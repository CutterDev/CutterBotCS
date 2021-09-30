using System;
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
        RiotGamesApi m_RiotInstance;
        public Leaderboard Leaderboards;

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
            string riottoken = Properties.Settings.Default.RiotApiToken;
            m_RiotInstance = RiotGamesApi.NewInstance(riottoken);
            Leaderboards = new Leaderboard(playerspath);
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            Leaderboards.Initialize();
        }

        /// <summary>
        /// Get Top 10 Summoner Masteries
        /// </summary>
        /// <param name="summonername"></param>
        /// <returns></returns>
        public List<string> GetTopSummonerMasteries(string summonername)
        {
            List<string> champions = new List<string>();

            // Get summoners by name synchronously. (using async is faster).
            var summoners = new[]
            {
                m_RiotInstance.SummonerV4().GetBySummonerName(PlatformRoute.EUW1, summonername)
            };

            foreach (var summoner in summoners)
            {
                var masteries = m_RiotInstance.ChampionMasteryV4().GetAllChampionMasteries(PlatformRoute.EUW1, summoner.Id);

                for (var i = 0; i < 10; i++)
                {
                    var mastery = masteries[i];
                    // Get champion for this mastery.
                    var champ = (Champion)mastery.ChampionId;
                    // print i, champ id, champ mastery points, and champ level
                    champions.Add(string.Format("{0,3}) {1,-16} {2,10:N0} Level ({3})", i + 1, champ, mastery.ChampionPoints, mastery.ChampionLevel));
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

            foreach (Player player in Leaderboards.Players)
            {
                if(string.IsNullOrWhiteSpace(player.Id))
                {
                    Summoner summoner = await m_RiotInstance.SummonerV4().GetBySummonerNameAsync(PlatformRoute.EUW1, player.SummonerName);

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
                    LeagueEntry[] entries = await m_RiotInstance.LeagueV4().GetLeagueEntriesForSummonerAsync(PlatformRoute.EUW1, player.Id);

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
        /// Get Ranked History Asynchronous
        /// </summary>
        public async Task<List<string>> GetRankedHistoryAsync(string summonername)
        {
            List<string> matchhistory = new List<string>();
            // Get summoners data (blocking).
            var summonerData = await m_RiotInstance.SummonerV4().GetBySummonerNameAsync(PlatformRoute.EUW1, summonername);

            if (null == summonerData)
            {
                // If a summoner is not found, the response will be null.
                return null;
            }

            // Get 10 most recent matches (blocking)
            var matchlist = await m_RiotInstance.MatchV5().GetMatchIdsByPUUIDAsync(
               RegionalRoute.EUROPE, summonerData.Puuid, start: 0, count: 10, type: "ranked");

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
                    .First(pi => summonerData.Id.Equals(pi.SummonerId));
                // Find the corresponding participant.
                var participant = matchData.Info.Participants
                    .First(p => p.ParticipantId == participantIdData.ParticipantId);

                var win = participant.Win;
                var champ = (Champion)participant.ChampionId;
                var k = participant.Kills;
                var d = participant.Deaths;
                var a = participant.Assists;
                var kda = (k + a) / (float)d;

                // Win/Loss, Champion
                // Champion, K/D/A
                string matchhistorymsg = string.Format("{0,3}) {1,-4} ({2})", i + 1, win ? "Win" : "Loss", champ) +
                                      string.Format("     K/D/A {0}/{1}/{2} ({3:0.00})", k, d, a, kda);

                matchhistory.Add(matchhistorymsg);
            }

            return matchhistory;
        }
    }
}
