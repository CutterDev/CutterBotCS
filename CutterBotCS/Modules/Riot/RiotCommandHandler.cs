using Camille.Enums;
using Camille.RiotGames.SummonerV4;
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
        public async Task<EmbedBuilder> GetRegisteredPlayerHistoryAsync(ulong discordid, ulong guildid)
        {
            var embed = new EmbedBuilder();

            PlayerEntity entity;
            string playerserror;

            PlayersTable pt = new PlayersTable();
            pt.OpenConnection(Properties.Settings.Default.BotDBConn, out playerserror);

            if (pt.TryGetPlayer(guildid, discordid, out entity, out playerserror))
            {
                embed.Title = entity.SummonerName;
                embed.Description = "Top 10 most recent ranked games";
                List<string> matches = await m_RiotAPIHandler.GetRankedHistoryByNameAsync(entity.SummonerName, (PlatformRoute)entity.PlatformRoute, (RegionalRoute)entity.RegionalRoute);

                int count = 1;
                
                if (matches != null && matches.Count > 0)
                {
                    foreach (string match in matches)
                    {
                        embed.AddField("Match " + count, match);

                        count++;
                    }
                }
                else
                {
                    embed.AddField("ERROR GETTING MATCH", "ERROR");
                }
            }
            else
            {
                embed.Description = "Player does not have a Summoner Registered";
            }
            pt.CloseConnection(out playerserror);

            return embed;
        }

        /// <summary>
        /// Get 10 most recent ranked games for a Summoner
        /// </summary>
        public async Task<List<string>> GetMatchHistoryEmbedAsync(string name, PlatformRoute pr, RegionalRoute rr)
        {
            List<string> matches = new List<string>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                List<string> history = await m_RiotAPIHandler.GetRankedHistoryByNameAsync(name, pr, rr);
                if (history != null && history.Count > 0)
                {
                    foreach (string match in history)
                    {
                        matches.Add(match);
                    }

                }
            }
            return matches;
        }

        /// <summary>
        /// Get 10 most recent ranked games for a Summoner
        /// </summary>
        public async Task<string> GetMatchHistoryAsync(string name, PlatformRoute pr, RegionalRoute rr)
        {
            StringBuilder message = new StringBuilder();

            message.Append("No Match History for this Summoner found try again.");

            if (!string.IsNullOrWhiteSpace(name))
            {
                List<string> history = await m_RiotAPIHandler.GetRankedHistoryByNameAsync(name, pr, rr);
                if (history != null && history.Count > 0)
                {
                    message.Clear();
                    message.AppendLine(string.Format("== {0} 10 Most Recent Ranked Games ==", name));
                    foreach (string match in history)
                    {
                        message.AppendLine(match);
                    }

                }
            }
            return message.ToString();
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
                summoner = await m_RiotAPIHandler.GetSummonerAsync(pr, name);
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
