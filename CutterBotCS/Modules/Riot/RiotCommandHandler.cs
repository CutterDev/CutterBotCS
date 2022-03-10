using Camille.Enums;
using Camille.RiotGames.MatchV5;
using Camille.RiotGames.SummonerV4;
using CutterBotCS.Discord;
using CutterBotCS.Modules.Riot.History;
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
        /// History Match Image Creator
        /// </summary>
        private HistoryCreator m_HistoryMatchCreator { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RiotCommandHandler(RiotAPIHandler handler)
        {
            m_RiotAPIHandler = handler;
            m_HistoryMatchCreator = new HistoryCreator();
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
        public async Task<bool> GetMatch(string matchid, RegionalRoute rr, string path)
        {
            bool result = false;

            Match match = await m_RiotAPIHandler.GetMatchFromMatchIdAsync(rr, matchid);

            if (match != null)
            {
                try
                {
                    HistoryMatchModel model = new HistoryMatchModel();

                    model.GameTime = match.Info.GameEndTimestamp.HasValue ? TimeSpan.FromSeconds(match.Info.GameDuration) : TimeSpan.FromMilliseconds(match.Info.GameDuration);
                    model.GameDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(match.Info.GameCreation).ToLocalTime();

                    model.RankType = match.Info.QueueId == Queue.SUMMONERS_RIFT_5V5_RANKED_SOLO ? "Ranked Solo/Duo" : "Ranked Flex";

                    foreach (var p in match.Info.Participants)
                    {
                        if (p.TeamId == Camille.RiotGames.Enums.Team.Blue)
                        {
                            model.Team1[GetLaneNumber(p.TeamPosition)] = p;
                        }
                        else if (p.TeamId == Camille.RiotGames.Enums.Team.Red)
                        {
                            model.Team2[GetLaneNumber(p.TeamPosition)] = p;
                        }
                    }

                    foreach (var t in match.Info.Teams)
                    {
                        if (t.TeamId == Camille.RiotGames.Enums.Team.Blue)
                        {
                            model.Team1Info = t;
                            if (t.Win)
                            {
                                model.VictoryTeam = t.TeamId;
                            }
                        }
                        else if (t.TeamId == Camille.RiotGames.Enums.Team.Red)
                        {
                            model.Team2Info = t;
                            if (t.Win)
                            {
                                model.VictoryTeam = t.TeamId;
                            }
                        }


                    }

                    result = m_HistoryMatchCreator.TryCreateMatchHistoryImage(model, path);
                }
                catch (Exception e)
                {
                    DiscordWorker.Log(string.Format("Error Creating History Model: {0}", e.Message), LogType.Error);
                }
            }

            return result;
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
                    model.Embed.AddField("Match " + (count + 1), match.Value);

                    count++;
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

            string playerserror;
            PlayersTable pt = new PlayersTable();
            pt.OpenConnection(Properties.Settings.Default.BotDBConn, out playerserror);
            DiscordWorker.Log(playerserror, LogType.Error);

            // Check if player already exists in database
            if (!pt.PlayerExists(guildid, discordid, out playerserror))
            {
                // Make sure no error occured before proceeding
                if (string.IsNullOrWhiteSpace(playerserror))
                {
                    Summoner summoner = null;

                    // Get Summoner
                    try
                    {
                        summoner = await m_RiotAPIHandler.GetSummonerBySummonerNameAsync(pr, name);
                    }
                    catch (Exception e)
                    {
                        DiscordWorker.Log(string.Format("Error getting summoner: {0}", e.Message), LogType.Error);
                    }

                    if (summoner != null)
                    {
                        // Create Entity for Database
                        PlayerEntity entity = new PlayerEntity()
                        {
                            DiscordId = discordid,
                            GuildId = guildid,
                            SummonerName = summoner.Name,
                            PlatformRoute = (int)pr,
                            RegionalRoute = (int)rr
                        };

                        // Insert Player into Database
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
                }
                else
                {
                    message = "Error has occured Contact CutterHealer#0001";
                }
            }
            else
            {
                DiscordWorker.Log(playerserror, LogType.Error);
                message = "You already have a player registered!";

                if (!string.IsNullOrWhiteSpace(playerserror))
                {
                    message += " ERROR OCCURED! Contact CutterHealer#0001";
                }
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

            string result = pt.RemovePlayer(guildid, discordid, out playerserror) ? "Summoner has been removed!" : "You do not have a player registed!";
            DiscordWorker.Log(playerserror, LogType.Error);

            if (!string.IsNullOrWhiteSpace(playerserror))
            {
                result += " ERROR OCCURED! Contact CutterHealer#0001";
            }

            pt.CloseConnection(out playerserror);
            DiscordWorker.Log(playerserror, LogType.Error);

            return result;
        }
    }
}
