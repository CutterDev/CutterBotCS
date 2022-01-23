using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camille.Enums;
using Camille.RiotGames;
using Camille.RiotGames.LeagueV4;
using Camille.RiotGames.SummonerV4;
using CutterBotCS.Modules.Leaderboard;
using CutterBotCS.Modules.Riot;

namespace CutterBotCS.RiotAPI
{
    /// <summary>
    /// Riot API Handler
    /// </summary>
    public class RiotAPIHandler
    {
        RiotGamesApi m_RiotInstance;
        public PlayerManager PManager;

        /// <summary>
        /// API Handler
        /// </summary>
        public RiotAPIHandler(string playerspath)
        {
            PManager = new PlayerManager(playerspath);
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            string riottoken = Properties.Settings.Default.RiotApiToken;
            m_RiotInstance = RiotGamesApi.NewInstance(riottoken);
            PManager.Initialize();
        }

        /// <summary>
        /// Get Summoner 
        /// </summary>
        public async Task<Summoner> GetSummonerAsync(PlatformRoute pr, string summonername)
        {
            return await m_RiotInstance.SummonerV4().GetBySummonerNameAsync(pr, summonername);
        }

        /// <summary>
        /// Get Summoner 
        /// </summary>
        public async Task<Summoner> GetSummonerByAccountIdAsync(PlatformRoute pr, string accountid)
        {
            return await m_RiotInstance.SummonerV4().GetByAccountIdAsync(pr, accountid);
        }


        /// <summary>
        /// Get Top 10 Masteries using Summoner Id
        /// </summary>
        public async Task<List<string>> GetTop10MasteriesByIdAsync(string id, PlatformRoute pr)
        {
            List<string> champions = new List<string>();
            var masteries = await m_RiotInstance.ChampionMasteryV4().GetAllChampionMasteriesAsync(pr, id);

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


            return champions;
        }

        /// <summary>
        /// Get Top 10 Summoner Masteries
        /// </summary>
        public async Task<List<string>> GetTopSummonerMasteriesAsync(string summonername, PlatformRoute pr)
        {
            List<string> champions = new List<string>();

            // Get Summoners Async
            var summoner = await GetSummonerAsync(PlatformRoute.EUW1, summonername);

            if (summoner != null)
            {
                var masteries = await m_RiotInstance.ChampionMasteryV4().GetAllChampionMasteriesAsync(pr, summoner.Id);

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
        /// Boyz League
        /// </summary>
        public async Task<List<LeaderboardEntry>> GetBoyzLeagueAsync(EntryFilter filter)
        {
            List<LeaderboardEntry> boys = new List<LeaderboardEntry>();

            var players = PManager.GetPlayers();
            foreach (Player player in players)
            {
                LeaderboardEntry lentry = null;
                if(string.IsNullOrWhiteSpace(player.Id))
                {
                    Summoner summoner = await m_RiotInstance.SummonerV4().GetBySummonerNameAsync(player.PlatformRoute, player.SummonerName);

                    if (summoner != null)
                    {
                        player.AccountId = summoner.AccountId;
                        player.Id = summoner.Id;
                        player.ProfileIconId = summoner.ProfileIconId;
                        player.Puuid = summoner.Puuid;
                        player.RevisionDate = summoner.RevisionDate;
                        player.SummonerLevel = summoner.SummonerLevel;
                    }
                }

                if (!string.IsNullOrWhiteSpace(player.Id))
                {
                    lentry = new LeaderboardEntry()
                    {
                        DiscordId = player.DiscordId
                    };

                    LeagueEntry[] entries = await m_RiotInstance.LeagueV4().GetLeagueEntriesForSummonerAsync(player.PlatformRoute, player.Id);

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
        public async Task<List<string>> GetRankedHistoryByIdAsync(string id, PlatformRoute pr, RegionalRoute rr)
        {
            var summonerData = await m_RiotInstance.SummonerV4().GetBySummonerIdAsync(pr, id);

            return await GetRankedHistoryAsync(rr, summonerData);
        }

        /// <summary>
        /// Get Ranked History Asynchronous
        /// </summary>
        public async Task<List<string>> GetRankedHistoryByNameAsync(string summonername, PlatformRoute pr, RegionalRoute rr)
        {
            // Get summoners data (blocking).
            var summonerData = await m_RiotInstance.SummonerV4().GetBySummonerNameAsync(pr, summonername);

            return await GetRankedHistoryAsync(rr, summonerData);
        }

        /// <summary>
        /// Get Ranked History ASync
        /// </summary>
        async Task<List<string>> GetRankedHistoryAsync(RegionalRoute rr, Summoner summonerdata)
        {
            List<string> matchhistory = new List<string>();
            if (summonerdata != null)
            {
                // Get 10 most recent matches (blocking)
                var matchlist = await m_RiotInstance.MatchV5().GetMatchIdsByPUUIDAsync(
                   rr, summonerdata.Puuid, start: 0, count: 10, type: "ranked");

                // Get match results (done asynchronously -> not blocking -> fast).
                var matchDataTasks = matchlist.Select(
                       matchMetadata => m_RiotInstance.MatchV5().GetMatchAsync(RegionalRoute.EUROPE, matchMetadata)
                   ).ToArray();

                // Wait for all task requests to complete asynchronously.
                var matchDatas = await Task.WhenAll(matchDataTasks);

                for (var i = 0; i < matchDatas.Count(); i++)
                {
                    var matchData = matchDatas[i];
                    // Get this summoner's participant ID info.
                    var participantIdData = matchData.Info.Participants
                        .First(pi => summonerdata.Id.Equals(pi.SummonerId));
                    // Find the corresponding participant.
                    var participant = matchData.Info.Participants
                        .First(p => p.ParticipantId == participantIdData.ParticipantId);

                    var win = participant.Win;
                    var champ = (Champion)participant.ChampionId;
                    var kills = participant.Kills;
                    var deaths = participant.Deaths;
                    var assists = participant.Assists;

                    string gamemode;

                    switch (matchData.Info.QueueId)
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
                    string matchhistorymsg = string.Format("{0} - {1,3}) {2,-4} ({3})", gamemode, i + 1, win ? "Win" : "Loss", champ) +
                                          string.Format("     K/D/A {0}/{1}/{2} ({3:0.00})", kills, deaths, assists, kda);

                    matchhistory.Add(matchhistorymsg);
                }
            }
            else
            {
                matchhistory.Add("Cannot find Summoner. Please try again.");
            }

            return matchhistory;
        }
    }
}
