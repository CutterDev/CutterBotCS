using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camille.Enums;
using Camille.RiotGames;
using Camille.RiotGames.LeagueV4;
using Camille.RiotGames.MatchV5;
using Camille.RiotGames.SummonerV4;
using CutterBotCS.Helpers;
using CutterBotCS.Modules.Leaderboard;
using CutterBotCS.SQL;
using CutterBotCS.Worker;

namespace CutterBotCS.RiotAPI
{
    /// <summary>
    /// Riot API Handler
    /// </summary>
    public class RiotAPIHandler
    {
        RiotGamesApi m_APIInstance;
        public PlayerManager PManager;
        SQLClient m_SQLClient;

        /// <summary>
        /// API Handler
        /// </summary>
        public RiotAPIHandler(string playerspath)
        {
            PManager = new PlayerManager(playerspath);

            m_SQLClient = new SQLClient();
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            string riottoken = Properties.Settings.Default.RiotApiToken;
            m_APIInstance = RiotGamesApi.NewInstance(riottoken);
            PManager.Initialize();
        }

        /// <summary>
        /// Get Summoner 
        /// </summary>
        public async Task<Summoner> GetSummonerAsync(PlatformRoute pr, string summonername)
        {
            return await m_APIInstance.SummonerV4().GetBySummonerNameAsync(pr, summonername);
        }

        /// <summary>
        /// Get Summoner 
        /// </summary>
        public async Task<Summoner> GetSummonerByAccountIdAsync(PlatformRoute pr, string accountid)
        {
            return await m_APIInstance.SummonerV4().GetByAccountIdAsync(pr, accountid);
        }

        /// <summary>
        /// Get Top 10 Masteries using Summoner Id
        /// </summary>
        public async Task<List<string>> GetTop10MasteriesByIdAsync(string id, PlatformRoute pr)
        {
            List<string> champions = new List<string>();
            var masteries = await m_APIInstance.ChampionMasteryV4().GetAllChampionMasteriesAsync(pr, id);

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
                var masteries = await m_APIInstance.ChampionMasteryV4().GetAllChampionMasteriesAsync(pr, summoner.Id);

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
                    Summoner summoner = await m_APIInstance.SummonerV4().GetBySummonerNameAsync(player.PlatformRoute, player.SummonerName);

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

                    LeagueEntry[] entries = await m_APIInstance.LeagueV4().GetLeagueEntriesForSummonerAsync(player.PlatformRoute, player.Id);

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
            var summonerData = await m_APIInstance.SummonerV4().GetBySummonerIdAsync(pr, id);

            return await GetRankedHistoryAsync(rr, summonerData);
        }

        /// <summary>
        /// Get Ranked History Asynchronous
        /// </summary>
        public async Task<List<string>> GetRankedHistoryByNameAsync(string summonername, PlatformRoute pr, RegionalRoute rr)
        {
            // Get summoners data (blocking).
            var summonerData = await m_APIInstance.SummonerV4().GetBySummonerNameAsync(pr, summonername);

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
                var matchlist = await m_APIInstance.MatchV5().GetMatchIdsByPUUIDAsync(
                   rr, summonerdata.Puuid, start: 0, count: 10, type: "ranked");

                m_SQLClient.Open();

                List<Match> matches;
                try
                {
                    matches = await GetMatches(matchlist, rr);

                }
                catch(Exception e)
                {
                    matches = new List<Match>();

                    DiscordWorker.Log(String.Format("Getting Ranked Matches: {0}", e.Message), LogType.Error);
                }

      

                int count = 1;
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

                    matchhistory.Add(matchhistorymsg);

                    count++;
                }
            }
            else
            {
                matchhistory.Add("Cannot find Summoner. Please try again.");
            }


            m_SQLClient.Close();

            return matchhistory;
        }

        /// <summary>
        /// Get Matches
        /// </summary>
        private async Task<List<Match>> GetMatches(string[] matchlist, RegionalRoute rr)
        {

            string region = string.Empty;

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

            List<Match> matches = new List<Match>();
            foreach (string matchid in matchlist)
            {
                Match match;
                if (!m_SQLClient.SelectFromMatchHistory(region, matchid, out match))
                {
                    match = await m_APIInstance.MatchV5().GetMatchAsync(RegionalRoute.EUROPE, matchid);

                    string team1json = string.Empty;
                    string team2json = string.Empty;
                    JsonHelper.TrySerialize(match.Info.Teams[0], out team1json);
                    JsonHelper.TrySerialize(match.Info.Teams[1], out team2json);

                    string message;
                    m_SQLClient.InsertToMatchHistory(region, match.Metadata.MatchId, match.Metadata.DataVersion, match.Info.GameCreation, match.Info.GameDuration, match.Info.GameEndTimestamp,
                                                    match.Info.GameId, match.Info.GameMode.ToString(), match.Info.GameName, match.Info.GameStartTimestamp, match.Info.GameType.ToString(),
                                                    match.Info.GameVersion, match.Info.MapId.ToString(), match.Info.PlatformId, match.Info.QueueId.ToString(), match.Info.Participants[0].ParticipantId, match.Info.Participants[1].ParticipantId,
                                                    match.Info.Participants[2].ParticipantId, match.Info.Participants[3].ParticipantId, match.Info.Participants[4].ParticipantId, match.Info.Participants[5].ParticipantId, match.Info.Participants[6].ParticipantId,
                                                    match.Info.Participants[7].ParticipantId, match.Info.Participants[8].ParticipantId, match.Info.Participants[9].ParticipantId, team1json, team2json, out message);                   
                    for (int i = 0; i < 10; i++)
                    {
                        Participant p = match.Info.Participants[i];
                        m_SQLClient.InsertToMatchParticipants(region, match.Metadata.MatchId, p.ParticipantId,
                            p.BaronKills,
                            p.BountyLevel,
                            p.ChampExperience,
                            (int)p.ChampionId,
                            p.ChampionName,
                            p.ChampionTransform,
                            p.ChampLevel,
                            p.ConsumablesPurchased,
                            p.DamageDealtToBuildings,
                            p.DamageDealtToObjectives,
                            p.DamageDealtToTurrets,
                            p.DamageSelfMitigated,
                            p.Deaths,
                            p.DetectorWardsPlaced,
                            p.DoubleKills,
                            p.DragonKills,
                            p.FirstBloodAssist,
                            p.FirstBloodKill,
                            p.FirstTowerAssist,
                            p.FirstTowerKill,
                            p.GameEndedInEarlySurrender,
                            p.GameEndedInSurrender,
                            p.GoldEarned,
                            p.GoldSpent,
                            p.IndividualPosition,
                            p.InhibitorKills,
                            p.InhibitorsLost,
                            p.InhibitorTakedowns,
                            p.Item0,
                            p.Item1,
                            p.Item2,
                            p.Item3,
                            p.Item4,
                            p.Item5,
                            p.Item6,
                            p.ItemsPurchased,
                            p.KillingSprees,
                            p.Kills,
                            p.Lane,
                            p.LargestCriticalStrike,
                            p.LargestKillingSpree,
                            p.LargestMultiKill,
                            p.LongestTimeSpentLiving,
                            p.MagicDamageDealt,
                            p.MagicDamageDealtToChampions,
                            p.MagicDamageTaken,
                            p.NeutralMinionsKilled,
                            p.NexusKills,
                            p.NexusLost,
                            p.NexusTakedowns,
                            p.ObjectivesStolen,
                            p.ObjectivesStolenAssists,
                            p.PentaKills,
                            p.PhysicalDamageDealt,
                            p.PhysicalDamageDealtToChampions,
                            p.PhysicalDamageTaken,
                            p.ProfileIcon,
                            p.Puuid,
                            p.QuadraKills,
                            p.RiotIdTagline,
                            p.Role,
                            p.SightWardsBoughtInGame,
                            p.Spell1Casts,
                            p.Spell2Casts,
                            p.Spell3Casts,
                            p.Spell4Casts,
                            p.Summoner1Casts,
                            p.Summoner1Id,
                            p.Summoner2Casts,
                            p.Summoner2Id,
                            p.SummonerId,
                            p.SummonerLevel,
                            p.SummonerName,
                            p.TeamEarlySurrendered,
                            p.TeamId.ToString(),
                            p.TeamPosition,
                            p.TimeCCingOthers,
                            p.TimePlayed,
                            p.TotalDamageDealt,
                            p.TotalDamageDealtToChampions,
                            p.TotalDamageShieldedOnTeammates,
                            p.TotalDamageTaken,
                            p.TotalHeal,
                            p.TotalHealsOnTeammates,
                            p.TotalMinionsKilled,
                            p.TotalTimeCCDealt,
                            p.TotalTimeSpentDead,
                            p.TotalUnitsHealed,
                            p.TripleKills,
                            p.TrueDamageDealt,
                            p.TrueDamageDealtToChampions,
                            p.TrueDamageTaken,
                            p.TurretKills,
                            p.TurretsLost,
                            p.TurretTakedowns,
                            p.UnrealKills,
                            p.VisionScore,
                            p.VisionWardsBoughtInGame,
                            p.WardsKilled,
                            p.WardsPlaced,
                            p.Win,
                            out message);

                        string perks1json;
                        string perks2json;

                        JsonHelper.TrySerialize(p.Perks.Styles[0], out perks1json);
                        JsonHelper.TrySerialize(p.Perks.Styles[1], out perks2json);

                        m_SQLClient.InsertParticipantsStats(region, matchid, p.ParticipantId, p.Perks.StatPerks.Defense, p.Perks.StatPerks.Flex,
                                                            p.Perks.StatPerks.Offense, perks1json, perks2json, out message);
                    }
                }

                matches.Add(match);
            }
            return matches;
        }
    }
}
