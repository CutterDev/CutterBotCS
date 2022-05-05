using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camille.Enums;
using Camille.RiotGames;
using Camille.RiotGames.ChampionMasteryV4;
using Camille.RiotGames.LeagueV4;
using Camille.RiotGames.MatchV5;
using Camille.RiotGames.SummonerV4;
using CutterBotCS.Discord;
using CutterBotCS.Modules.Riot;
using CutterBotCS.Worker;
using CutterDB.Tables;

namespace CutterBotCS.RiotAPI
{
    /// <summary>
    /// Riot API Handler
    /// </summary>
    public class RiotAPIHandler
    {
        RiotGamesApi m_APIInstance;
        public string Token { private set; get; }

        /// <summary>
        /// API Handler
        /// </summary>
        public RiotAPIHandler()
        {
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize(string riottoken)
        {
            m_APIInstance = RiotGamesApi.NewInstance(riottoken);
            Token = riottoken;
        }

        /// <summary>
        /// Get Summoner 
        /// </summary>
        public async Task<Summoner> GetSummonerBySummonerNameAsync(PlatformRoute pr, string summonername)
        {
            Summoner summ  = null;
            try
            {
               summ = await m_APIInstance.SummonerV4().GetBySummonerNameAsync(pr, summonername);
            }
            catch (Exception e)
            {
                DiscordWorker.Log(string.Format("Error Getting Summoner: {0}", e.Message), LogType.Error);
            }

            return summ;
        }

        /// <summary>
        /// Get Summoner 
        /// </summary>
        public async Task<Summoner> GetSummonerByAccountIdAsync(PlatformRoute pr, string accountid)
        {
            Summoner summ = null;
            try
            {
                summ = await m_APIInstance.SummonerV4().GetByAccountIdAsync(pr, accountid);
            }
            catch (Exception e)
            {
                DiscordWorker.Log(string.Format("Error Getting Summoner: {0}", e.Message), LogType.Error);
            }

            return summ;
        }

        /// <summary>
        /// Get Match From MatchId Async
        /// </summary>
        public async Task<Match> GetMatchFromMatchIdAsync(RegionalRoute rr, string matchid)
        {
            Match match = null;
            try
            {
                match = await m_APIInstance.MatchV5().GetMatchAsync(rr, matchid);
            }
            catch (Exception e)
            {
                DiscordWorker.Log(string.Format("Error Getting Match5V:\r\n " +
                                                "Error: {0} \r\n" +
                                                "Region: {1} \r\n" +
                                                "MatchId: {2} \r\n", e.Message, rr, matchid), LogType.Error);
            }

            return match;
        }

        /// <summary>
        /// Get Top 10 Masteries using Summoner Id
        /// </summary>
        public async Task<List<string>> GetTop10MasteriesByNameAsync(string summonername, PlatformRoute pr)
        {
            List<string> champions = new List<string>();

            var summoner = await GetSummonerBySummonerNameAsync(pr, summonername);

            if (summoner != null)
            {
                ChampionMastery[] masteries = null;

                try
                {
                    masteries = await m_APIInstance.ChampionMasteryV4().GetAllChampionMasteriesAsync(pr, summoner.Id);
                }
                catch (Exception e)
                {
                    DiscordWorker.Log(String.Format("Cannot get Champion Masteries Id: {0}\r\nError: {1}", summoner.Id, e.Message), LogType.Error);
                }

                if (masteries != null)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        // Get champion for this mastery.
                        var champ = (Champion)masteries[i].ChampionId;
                        // print i, champ id, champ mastery points, and champ level
                        champions.Add(string.Format("{0,3}) {1,-16} {2,10:N0} Level ({3})", i + 1, champ, masteries[i].ChampionPoints, masteries[i].ChampionLevel));
                    }
                }
            }



            return champions;
        }

        /// <summary>
        /// Get Players Leaderboard Entries
        /// </summary>
        public async Task<List<LeaderboardEntry>> GetPlayersLeaderboardEntries(List<Player> players, EntryFilter filter)
        {
            List<LeaderboardEntry> boys = new List<LeaderboardEntry>();
        
            foreach (Player player in players)
            {
                LeaderboardEntry lentry = null;
                if(!string.IsNullOrWhiteSpace(player.SummonerName))
                {
                    Summoner summoner = await GetSummonerBySummonerNameAsync(player.PlatformRoute, player.SummonerName);

                    if (summoner != null)
                    {
                        lentry = new LeaderboardEntry();

                        LeagueEntry[] entries = null;

                        try
                        {
                            entries = await m_APIInstance.LeagueV4().GetLeagueEntriesForSummonerAsync(player.PlatformRoute, summoner.Id);

                        }
                        catch (Exception e)
                        {
                            DiscordWorker.Log(string.Format("Error getting League Entries \r\n" +
                                                            "Id: {0} SummonerName: {1} \r\n" +
                                                            "Error: {2}", summoner.Name, summoner.Id, e.Message), LogType.Error);
                        }

                        LeagueEntry soloranked = null;

                        if (entries != null && entries.Length > 0)
                        {
                            foreach (LeagueEntry entry in entries)
                            {
                                if (entry.QueueType == QueueType.RANKED_SOLO_5x5)
                                {
                                    soloranked = entry;
                                }
                            }
                        }

                        if (lentry != null && soloranked != null)
                        {
                            lentry.LeagueId = soloranked.LeagueId;
                            lentry.LeaguePoints = soloranked.LeaguePoints;
                            lentry.Division = soloranked.Rank;
                            lentry.Tier = soloranked.Tier;
                            lentry.Losses = soloranked.Losses;
                            lentry.Wins = soloranked.Wins;
                            lentry.SummonerId = soloranked.SummonerId;
                            lentry.SummonerName = soloranked.SummonerName;
                            boys.Add(lentry);
                        }
                    }                
                }

                switch(filter)
                {
                    case EntryFilter.None:
                        boys = boys.OrderBy(b => b.Tier).ThenBy(b => b.Division)
                                    .ThenByDescending(b => b.LeaguePoints).ToList();
                        break;

                    case EntryFilter.MostWins:
                        boys = boys.OrderByDescending(b => b.Wins).ToList();
                        break;
                    case EntryFilter.MostLosses:
                        boys = boys.OrderByDescending(b => b.Losses).ToList();
                        break;

                    case EntryFilter.MostGames:
                        boys = boys.OrderByDescending(b => b.TotalGames).ToList();
                        break;
                    case EntryFilter.HighestWinRate:
                        boys = boys.OrderByDescending(b => b.WinRate).ToList();
                        break;
                    case EntryFilter.LowestWinRate:
                        boys = boys.OrderBy(b => b.WinRate).ToList();
                        break;
                }
            }

            return boys;
        }

        /// <summary>
        /// Get Ranked History By ID Async
        /// </summary>
        public async Task<Dictionary<string, string>> GetRankedHistoryByIdAsync(string id, PlatformRoute pr, RegionalRoute rr)
        {
            Dictionary<string, string> history = null;
            Summoner summonerData = null;

            summonerData = await GetSummonerByAccountIdAsync(pr, id);

            if (summonerData != null)
            {
                history = await GetRankedHistoryAsync(rr, summonerData);
            }

            return history;
        }

        /// <summary>
        /// Get Ranked History Asynchronous
        /// </summary>
        public async Task<Dictionary<string, string>> GetRankedHistoryByNameAsync(string summonername, PlatformRoute pr, RegionalRoute rr)
        {
            Dictionary<string, string> history = null;

            Summoner summonerData = await GetSummonerBySummonerNameAsync(pr, summonername);   

            if (summonerData != null)
            {
                history = await GetRankedHistoryAsync(rr, summonerData);
            }

            return history;
        }

        /// <summary>
        /// Get Ranked History ASync
        /// </summary>
        async Task<Dictionary<string, string>> GetRankedHistoryAsync(RegionalRoute rr, Summoner summonerdata)
        {
            Dictionary<string, string> matchhistory = null;
            if (summonerdata != null)
            {
                // Get 10 most recent matches (blocking)
                string[] matchlist = null;

                try
                {
                   matchlist  = await m_APIInstance.MatchV5().GetMatchIdsByPUUIDAsync(
                                            rr, summonerdata.Puuid, start: 0, count: 10, type: "ranked");
                }
                catch (Exception e)
                {
                    DiscordWorker.Log(string.Format("Getting API Ranked Matches: {0}", e.Message), LogType.Error);
                }

                if (matchlist != null)
                {
                    List<Match> matches;
                    try
                    {
                        matches = await GetMatches(matchlist, rr);

                    }
                    catch (Exception e)
                    {
                        matches = new List<Match>();

                        DiscordWorker.Log(string.Format("Getting Ranked Matches: {0}", e.Message), LogType.Error);
                    }

                    int count = 1;
                    if (matches != null)
                    {
                        matchhistory = new Dictionary<string, string>();

                        foreach (Match match in matches)
                        {
                            // Get this summoner's participant ID info.
                            var participantIdData = match.Info.Participants
                                .First(pi => summonerdata.Id.Equals(pi.SummonerId));
                            // Find the corresponding participant.
                            var participant = match.Info.Participants
                                .First(p => p.ParticipantId == participantIdData.ParticipantId);

                            var win = participant.Win;
                            var champ = (Champion)participant.ChampionId;
                            var kills = participant.Kills;
                            var deaths = participant.Deaths;
                            var assists = participant.Assists;

                            string gamemode;

                            switch (match.Info.QueueId)
                            {
                                case Queue.SUMMONERS_RIFT_5V5_RANKED_SOLO:
                                    gamemode = "SOLO";
                                    break;
                                case Queue.SUMMONERS_RIFT_5V5_RANKED_FLEX:
                                    gamemode = "FLEX";
                                    break;
                                default:
                                    gamemode = string.Empty;
                                    break;
                            }

                            var kda = deaths == 0 ? kills + assists : (kills + assists) / (float)deaths;

                            // GAMEMODE - WIN/LOSS - CHAMPION - KILLS/DEATHS/ASSISTS KDA
                            string matchhistorymsg = string.Format("{0} - {1,3}) {2,-4} ({3})", gamemode, count, win ? "Win" : "Loss", champ) +
                                                  string.Format("     K/D/A {0}/{1}/{2} ({3:0.00})", kills, deaths, assists, kda);

                            matchhistory.Add(match.Metadata.MatchId, matchhistorymsg);

                            count++;
                        }
                    }  
                }
            }

            return matchhistory;
        }

        /// <summary>
        /// Get Matches
        /// </summary>
        private async Task<List<Match>> GetMatches(string[] matchlist, RegionalRoute rr)
        {
            List<Match> matches = new List<Match>();

            foreach(var matchid in matchlist)
            {
                Match match = await GetMatchFromMatchIdAsync(rr, matchid);

                if (match != null)
                {
                    matches.Add(match);
                }
            }

            return matches;
        }

        /// <summary>
        /// Get Region Code
        /// </summary>
        private string GetRegionCode(RegionalRoute rr)
        {
            string region;

            switch (rr)
            {
                case RegionalRoute.ASIA:
                    region = "asia";
                    break;
                case RegionalRoute.AMERICAS:
                    region = "na";
                    break;
                case RegionalRoute.EUROPE:
                    region = "euw";
                    break;
                case RegionalRoute.SEA:
                    region = "oce";
                    break;
                default:
                    region = "euw";
                    break;
            }

            return region;
        }
    }
}
