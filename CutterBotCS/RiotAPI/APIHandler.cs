using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camille.Enums;
using Camille.RiotGames;
using Camille.RiotGames.LeagueV4;
using Camille.RiotGames.SummonerV4;
using CutterBotCS.Modules.Riot;

namespace CutterBotCS.RiotAPI
{
    public class APIHandler
    {
        string m_PlayersPath;
        RiotGamesApi m_RiotInstance;
        public PlayerManager PManager;

        // ENCRYPTED SUMMONER IDS ARE ENCRYPTED TO THE API KEY.
        // WHEN REQUESTING SUMMONER IDS USE THE SAME KEY FOR REQUEST WITH THOSE IDS
        private List<string> m_Boyz_Ids = new List<string>()
        {
            // CutterHealer
            "TQ5lPY8EqMbXK0XZiRiwuPTakusJJZGcBNQfh4YGULuIt9U",
            // Viperino
            "aJ_dZCp7tf_YPT2UXPk77W-dl6mIoq1Vtpk5ztDMnBwQDdo",
            // Beyn
            "Olby6WI9UHFkUgi5egfpt-pLWb1ntSKYbFAyehoU5NiHqNE",
            // Logyra
            "l6klspuZY9a3RXqoSlQqbzRndeWCQbkPvn-2PYjXBmuuVA0",
            // Raging
            "KHP-no_MKNViwnxFesnYl59rqxub7AN0CJ7gYJeR1ms_eCvn",
            // Sherveen
            "c9h0UDjonKZRWYKIpVnSjR_mMesE9P0ZySf-1vx-TYQnPWQ",
            // Tommy 
            "Ntd7VqWbXTeO9hNtSBErjESDOoppyE_cV7sugmEpIUX1BkJk",
            // Pearl
            "x8osa-diL3np9WwnQjS-xIcAHRh537dayU09Itf3fhrrrRcm",
            // Fletch
            "zJKDFPrC9ejrOCHbqhJvJo080JtcKnyJ1sy4yK4pcPRD9V77",
            // Bluka
            "kignUG9EDBBg-CvUUQwWoRm5C-jMAx8RvaP_sjW-S6Is2dc",
            // Shahrooz
            "KArgj_cLuhu7UduodL2n8Q7LO-FbPU8dtI3Q4ex-m_On3EI",
            // The Liin 
            "wHNIK1k621gbGTZuLELbIeq-M-lmjSNtCpO8pcbtanClStdm",
            // GhostLust
            "uAzAsqLPneRFjLMM1QyOoL4s79JvXVEmy9AiM3C5vyT9kpCYK1daD4OgnQ",
            // Nix0712
            "9W6Ks02Ja9bMcnli3bQ0vQTp7r0piPpMgRT_wyLdr8osRKJ1g8yyMNmEew"
        };

        /// <summary>
        /// API Handler
        /// </summary>
        public APIHandler(string playerspath)
        {
            m_PlayersPath = playerspath;
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
        /// <param name="summonername"></param>
        /// <returns></returns>
        public async Task<Summoner> GetSummonerAsync(PlatformRoute pr, string summonername)
        {
            return await m_RiotInstance.SummonerV4().GetBySummonerNameAsync(pr, summonername);
        }

        /// <summary>
        /// Get Summoner 
        /// </summary>
        /// <param name="summonername"></param>
        /// <returns></returns>
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
        /// <param name="summonername"></param>
        /// <returns></returns>
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
        /// <returns></returns>
        public async Task<List<LeagueEntry>> GetBoyzLeagueAsync()
        {
            List<LeagueEntry> boys = new List<LeagueEntry>();

            foreach (Player player in PManager.Players)
            {
                if(string.IsNullOrWhiteSpace(player.Id))
                {
                    Summoner summoner = await m_RiotInstance.SummonerV4().GetBySummonerNameAsync(player.Route, player.SummonerName);

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
                    LeagueEntry[] entries = await m_RiotInstance.LeagueV4().GetLeagueEntriesForSummonerAsync(player.Route, player.Id);

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

                    if (soloranked != null)
                    {
                        boys.Add(soloranked);
                    }
                }

                boys = boys.OrderBy(b => b.Tier).ThenBy(b => b.Rank).ThenByDescending(b => b.LeaguePoints).ToList();
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
