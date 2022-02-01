using Camille.Enums;
using Camille.RiotGames.SummonerV4;
using CutterBotCS.Discord;
using CutterBotCS.RiotAPI;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutterBotCS.Modules.Riot
{
    /// <summary>
    /// Riot Commands Helper
    /// </summary>
    public static class RiotCommandHelper
    {
        /// <summary>
        /// Get registered player's 10 most recent ranked games
        /// </summary>
        public static async Task<EmbedBuilder> GetRegisteredPlayerHistoryAsync(ulong discordid)
        {
            var embed = new EmbedBuilder();

            Player player;
            if (DiscordBot.RiotHandler.PManager.TryGetPlayer(discordid, out player))
            {
                embed.Title = player.SummonerName;
                embed.Description = "Top 10 most recent ranked games";
                List<string> matches = await GetMatchHistoryEmbedAsync(player.SummonerName, player.PlatformRoute, player.RegionalRoute);

                int count = 1;
                foreach(string match in matches)
                {
                    embed.AddField("Match " + count, match);

                    count++;
                }
            }
            else
            {
                embed.Description = "Player does not have a Summoner Registered";
            }

            return embed;
        }

        /// <summary>
        /// Get 10 most recent ranked games for a Summoner
        /// </summary>
        public static async Task<List<string>> GetMatchHistoryEmbedAsync(string name, PlatformRoute pr, RegionalRoute rr)
        {
            List<string> matches = new List<string>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                List<string> history = await DiscordBot.RiotHandler.GetRankedHistoryByNameAsync(name, pr, rr);
                if (history.Count > 0)
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
        public static async Task<string> GetMatchHistoryAsync(string name, PlatformRoute pr, RegionalRoute rr)
        {
            StringBuilder message = new StringBuilder();

            message.Append("No Match History for this Summoner found try again.");

            if (!string.IsNullOrWhiteSpace(name))
            {
                List<string> history = await DiscordBot.RiotHandler.GetRankedHistoryByNameAsync(name, pr, rr);
                if (history.Count > 0)
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
        public static async Task<string> GetRegisteredPlayerMasteryAsync(ulong discordid)
        {
            string message = string.Empty;

            Player player;
            if (DiscordBot.RiotHandler.PManager.TryGetPlayer(discordid, out player))
            {
                List<string> champions = await DiscordBot.RiotHandler.GetTop10MasteriesByIdAsync(player.Id, player.PlatformRoute);
                message = ChampionMasteries(champions, player.SummonerName);
            }
            else
            {
                message = "Player does not have a Summoner Registered";
            }

            return message;
        }

        /// <summary>
        /// Get Top 10 Masterys for a summoner
        /// </summary>
        public static async Task<string> MasteryAsync(string summonername, PlatformRoute pr)
        {
            string message = string.Empty;

            if (string.IsNullOrWhiteSpace(summonername))
            {
                message = "Summoner ID empty";
            }
            else
            {
                List<string> champions = await DiscordBot.RiotHandler.GetTopSummonerMasteriesAsync(summonername, pr);
                message = ChampionMasteries(champions, summonername);
            }

            return message;
        }

        /// <summary>
        /// ChampionMasteriesAsync
        /// </summary>
        public static string ChampionMasteries(List<string> champions, string summonername)
        {
            StringBuilder message = new StringBuilder();

            if (champions.Count > 0)
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
        /// Super User Register Account
        /// </summary>
        public static async Task<string> SuperUserRegister(string name, RegionalRoute rr, PlatformRoute pr, ulong user, ulong discordid)
        {
            string message = string.Empty;

            if(DiscordBot.IsEthan(discordid))
            {
                message = await RegisterPlayerAsync(name, rr, pr, user);
            }
            else
            {
                message = "You are not Ethan feck off.";
            }

            return message;
        }

        /// <summary>
        /// Register Player to PlayerManager for RiotAPI
        /// </summary>
        public static async Task<string> RegisterPlayerAsync(string name, RegionalRoute rr, PlatformRoute pr, ulong user)
        {
            string message = string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                message = "Please Submit a name.";
            }
            else if (DiscordBot.RiotHandler.PManager.PlayerExists(user))
            {
                message = "You already registered an Account";
            }
            else
            {
                Summoner summoner = await DiscordBot.RiotHandler.GetSummonerAsync(pr, name);

                if (summoner != null)
                {
                    Player player = new Player()
                    {
                        DiscordId = user,
                        SummonerName = summoner.Name,
                        AccountId = summoner.AccountId,
                        Id = summoner.Id,
                        ProfileIconId = summoner.ProfileIconId,
                        PlatformRoute = pr,
                        RegionalRoute = rr,
                        Puuid = summoner.Puuid,
                        RevisionDate = summoner.RevisionDate
                    };
                    DiscordBot.RiotHandler.PManager.AddPlayer(player);
                    message = string.Format("Summoner {0} has been registered!", player.SummonerName);
                }
                else
                {
                    message = "Player does not exist, register another.";
                }
            }

            return message;
        }

        /// <summary>
        /// Super User Remove Player
        /// </summary>
        public static string SuperUserRemovePlayer(ulong id, ulong discordid)
        {
            string message = String.Empty;

            if (DiscordBot.IsEthan(discordid))
            {
                message = RemovePlayer(id);
            }
            else
            {
                message = "You are not Ethan. Feck off.";
            }

            return message;
        }

        /// <summary>
        /// Remove Player
        /// </summary>
        public static string RemovePlayer(ulong id)
        {
            string result = string.Empty;

            List<Player> players = DiscordBot.RiotHandler.PManager.GetPlayers().Where(p => p.DiscordId == id).ToList();

            if (players.Count > 0)
            {
                DiscordBot.RiotHandler.PManager.RemovePlayer(players[0]);
                result = string.Format("Summoner {0} has been removed!", players[0].SummonerName);
            }
            else
            {
                result = "You do not have a Summoner Registered";
            }

            return result;
        }
    }
}
