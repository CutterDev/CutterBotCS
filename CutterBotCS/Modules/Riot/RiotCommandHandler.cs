using Camille.Enums;
using Camille.RiotGames.MatchV5;
using Camille.RiotGames.SummonerV4;
using CutterBotCS.Discord;
using CutterBotCS.RiotAPI;
using CutterBotCS.Worker;
using CutterDB.Entities;
using CutterDB.Tables;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CutterBotCS.Modules.Riot
{
    /// <summary>
    /// Riot Commands Helper
    /// </summary>
    public class RiotCommandHandler
    {
        private RiotAPIHandler m_RiotAPIHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public RiotCommandHandler(RiotAPIHandler handler)
        {
            m_RiotAPIHandler = handler;
        }

        /// <summary>
        /// Get registered player's 10 most recent ranked games
        /// </summary>
        public async Task<KeyValuePair<ulong, MatchHistoryEmbedModel>> GetRegisteredPlayerHistoryAsync(ulong discordid, ulong guildid)
        {
            MatchHistoryEmbedModel model = null;


            PlayerEntity entity;
            string playerserror;

            PlayersTable pt = new PlayersTable();
            pt.OpenConnection(Properties.Settings.Default.BotDBConn, out playerserror);
            DiscordWorker.Log(playerserror, LogType.Error);

            if (pt.TryGetPlayer(guildid, discordid, out entity, out playerserror))
            {
                model = await GetEmbedMatchModel(entity.SummonerName, (PlatformRoute)entity.PlatformRoute, (RegionalRoute)entity.RegionalRoute);
            }

            DiscordWorker.Log(playerserror, LogType.Error);

            pt.CloseConnection(out playerserror);
            DiscordWorker.Log(playerserror, LogType.Error);
            return new KeyValuePair<ulong, MatchHistoryEmbedModel>(discordid, model);
        }

        /// <summary>
        /// Get Match as Embed
        /// </summary>
        public async Task<EmbedBuilder> GetMatch(string matchid, RegionalRoute rr)
        {
            EmbedBuilder builder = new EmbedBuilder();

            Match match = await m_RiotAPIHandler.GetMatchFromMatchIdAsync(rr, matchid);

            if (match != null)
            {
                var team1 = new List<Participant>();    
                var team2 = new List<Participant>();
                var team1id = match.Info.Teams[0].TeamId;

                foreach(var p in match.Info.Participants)
                {
                    if (p.TeamId == team1id)
                    {
                        team1.Add(p);
                    }
                    else
                    {
                        team2.Add(p);
                    }
                }

                string[] matchstats = new string[5];
                foreach(var p in team1)
                {
                    int teampos = GetLaneNumber(p.TeamPosition);

                    foreach(var p2 in team2)
                    {
                        if (GetLaneNumber(p2.TeamPosition) == teampos)
                        {
                            matchstats[teampos] = string.Format("{0} | {1} {2}/{3}/{4} ", p.SummonerName, p.ChampionName, p.Kills, p.Deaths, p.Assists) +
                                                  string.Format("{0}/{1}/{2} {3} | {4}", p2.Kills, p2.Deaths, p2.Assists, p2.ChampionName, p2.SummonerName);
                        }
                    }
                }

                for (int i = 0; i < matchstats.Length; i++)
                {
                    builder.AddField(string.Format("Match {0}" + (i + 1)), matchstats[i]);  
                }
            }
            else
            {
                builder.AddField("ERROR", "Error getting match. Please contact CutterHealer#0001");
            }

            return builder;
        }

        private int GetLaneNumber(string teamposition)
        {
            int lane;

            switch (teamposition.ToUpper())
            {
                case "TOP":
                    lane = 0;
                    break;
                case "JUNGLE":
                    lane = 1;
                    break;
                case "MIDDLE":
                    lane = 2;
                    break;
                case "BOTTOM":
                    lane = 3;
                    break;
                case "UTILITY":
                    lane = 4;
                    break;
                default:
                    lane = -1;
                    break;
            }

            return lane;
        }

        /// <summary>
        /// Get Embed Match Model
        /// </summary>
        public async Task<MatchHistoryEmbedModel> GetEmbedMatchModel(string summonername, PlatformRoute pr, RegionalRoute rr)
        {
            MatchHistoryEmbedModel model = new MatchHistoryEmbedModel()
            {
                Embed = new EmbedBuilder()
            };

            model.Embed.Title = summonername;
            model.Embed.Description = "Top 10 most recent ranked games";
            Dictionary<string, string> matches = await m_RiotAPIHandler.GetRankedHistoryByNameAsync(summonername, pr, rr);

            int count = 0;

            if (matches != null && matches.Count > 0)
            {
                foreach (KeyValuePair<string, string> match in matches)
                {
                    model.MatchIds[count] = match.Key;
                    model.Embed.AddField("Match " + (++count), match.Value);

      
                }

                model.RegionalRoute = rr;
                model.Embed.Footer = new EmbedFooterBuilder() { Text = "Select a match to view! e.g. !selectmatch 1" };
            }
            else
            {
                model.Embed.AddField("ERROR GETTING MATCH", "ERROR");
            }

            return model;
        }

        /// <summary>
        /// Get Registered Players Top 10 Masteries
        /// </summary>
        public async Task<string> GetRegisteredPlayerMasteryAsync(ulong discordid, ulong guildid)
        {
            string message = string.Empty;

            string playerserror;
            PlayersTable pt = new PlayersTable();
            pt.OpenConnection(Properties.Settings.Default.BotDBConn, out playerserror);

            PlayerEntity entity;
            if (pt.TryGetPlayer(guildid, discordid, out entity, out playerserror))
            {
                var platformroute = (PlatformRoute)entity.PlatformRoute;

                List<string> champions = await m_RiotAPIHandler.GetTop10MasteriesByNameAsync(entity.SummonerName, platformroute);
                message = ChampionMasteries(champions, entity.SummonerName);
            }
            else
            {
                message = "Player does not have a Summoner Registered";
            }

            pt.CloseConnection(out playerserror);

            return message;
        }

        /// <summary>
        /// Get Top 10 Masterys for a summoner
        /// </summary>
        public async Task<string> MasteryAsync(string summonername, PlatformRoute pr)
        {
            string message = string.Empty;

            if (string.IsNullOrWhiteSpace(summonername))
            {
                message = "Summoner ID empty";
            }
            else
            {
                List<string> champions = await m_RiotAPIHandler.GetTop10MasteriesByNameAsync(summonername, pr);
                message = ChampionMasteries(champions, summonername);
            }

            return message;
        }

        /// <summary>
        /// ChampionMasteriesAsync
        /// </summary>
        public string ChampionMasteries(List<string> champions, string summonername)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("Error Getting Masteries");

            if (champions != null && champions.Count > 0)
            {
                message.Clear();
                message.AppendLine(string.Format("== {0} Top 10 Champion Masteries ==", summonername));
                foreach (string champion in champions)
                {
                    message.AppendLine(champion);
                }

            }

            return message.ToString();
        }

        /// <summary>
        /// Register Player to PlayerManager for RiotAPI
        /// </summary>
        public async Task<string> RegisterPlayerAsync(string name, RegionalRoute rr, PlatformRoute pr, ulong discordid, ulong guildid)
        {
            string message = string.Empty;

            Summoner summoner = null;
            try
            {
                summoner = await m_RiotAPIHandler.GetSummonerBySummonerNameAsync(pr, name);
            }
            catch(Exception e)
            {
                DiscordWorker.Log(string.Format("Error getting summoner: {0}", e.Message), LogType.Error);
            }

            if (summoner != null)
            {
                PlayerEntity entity = new PlayerEntity()
                {
                    DiscordId = discordid,
                    GuildId = guildid,
                    SummonerName = summoner.Name,
                    PlatformRoute = (int)pr,
                    RegionalRoute = (int)rr
                };

                string playerserror;
                PlayersTable pt = new PlayersTable();
                pt.OpenConnection(Properties.Settings.Default.BotDBConn, out playerserror);
                DiscordWorker.Log(playerserror, LogType.Error);
                if (pt.InsertPlayer(entity, out playerserror))
                {
                    message = string.Format("Registered {0}!", summoner.Name);
                }
                else
                {
                    message = "Error has occured Contact CutterHealer#0001";
                }
                DiscordWorker.Log(playerserror, LogType.Error);

                pt.CloseConnection(out playerserror);
                DiscordWorker.Log(playerserror, LogType.Error);
            }
            else
            {
                message = "Summoner could not be found.";
            }
            

            return message;
        }

        /// <summary>
        /// Remove Player
        /// </summary>
        public string RemovePlayer(ulong discordid, ulong guildid)
        {
            string playerserror;
            PlayersTable pt = new PlayersTable();
            pt.OpenConnection(Properties.Settings.Default.BotDBConn, out playerserror);
            DiscordWorker.Log(playerserror, LogType.Error);

            string result = pt.RemovePlayer(guildid, discordid, out playerserror) ? "Summoner has been removed!" : "Error has occured Contact CutterHealer#0001";
            DiscordWorker.Log(playerserror, LogType.Error);

            pt.CloseConnection(out playerserror);
            DiscordWorker.Log(playerserror, LogType.Error);

            return result;
        }
    }
}
