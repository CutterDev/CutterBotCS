using CutterBotCS.Discord;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camille.RiotGames.LeagueV4;
using CutterBotCS.Helpers;
using Camille.RiotGames.SummonerV4;
using Camille.Enums;

namespace CutterBotCS.Modules.Riot
{
    public class RiotCommandsModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Draw Leaderboard as a PNG and display on Discord as a message
        /// </summary>
        /// <returns></returns>
        [Command("leaderboard")]
        [Summary("Pearlsayah Leaderboard for Solo Ranked")]
        [RequireUserPermission(ChannelPermission.SendMessages, Group = "boyz")]
        public async Task LeaderboardAsync()
        {
            StringBuilder message = new StringBuilder();
            string image = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/CutterBot/leaderboard.png";
            message.Append("LEADERBOARD NOT FOUND WTF DID YOU BREAK ETHAN?");
            List<LeagueEntry> boys = await DiscordBot.RiotHandler.GetBoyzLeagueAsync();

            LeaderboardUICreator lc = new LeaderboardUICreator();
            lc.CreateLeaderboard(boys, image);

            await Context.Channel.SendFileAsync(image, string.Empty);
        }

        #region Mastery Commands

        /// <summary>
        /// Riot EUW Mastery for an Unregistered player, e.g. !RiotEUW Mastery [PlayerName]
        /// </summary>
        [Command("RiotEUW Mastery")]
        [Summary("Gets top 10 Champion Masteries of a player")]
        public async Task UnregisteredEUWMasteries(string summonername)
        {
            string message = await UnregisteredMastery(summonername, PlatformRoute.EUW1);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Riot EUNE Mastery for an Unregistered player, e.g. !RiotEUNE Mastery [PlayerName]
        /// </summary>
        [Command("RiotEUNE Mastery")]
        [Summary("Gets top 10 Champion Masteries of a player")]
        public async Task UnregisteredEUNEMasteries(string summonername)
        {
            string message = await UnregisteredMastery(summonername, PlatformRoute.EUN1);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Riot NA Mastery for an Unregistered player, e.g. !RiotNA Mastery [PlayerName]
        /// </summary>
        [Command("RiotNA Mastery")]
        [Summary("Gets top 10 Champion Masteries of a player")]
        public async Task UnregisteredNAMasteries(string summonername)
        {
            string message = await UnregisteredMastery(summonername, PlatformRoute.NA1);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Registered Masteries
        /// </summary>      
        [Command("mastery")]
        [Summary("Gets top 10 Champion Masteries for Registered Player")]
        public async Task RegisteredMasteries()
        {
            string message = string.Empty;
            Player player;
            if (DiscordBot.RiotHandler.PManager.TryGetPlayer(Context.User.Id, out player))
            {
                List<string> champions = await DiscordBot.RiotHandler.GetTop10MasteriesByIdAsync(player.Id, player.Route);
                message = ChampionMasteries(champions, player.SummonerName);
            }
            else
            {
                message = "Player does not have a Summoner Registered";
            }

            await ReplyAsync(message);
        }

        /// <summary>
        /// Unregisted Mastery
        /// </summary>
        async Task<string> UnregisteredMastery(string summonername, PlatformRoute pr)
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
        string ChampionMasteries(List<string> champions, string summonername)
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

        #endregion

        #region History Commands

        /// <summary>
        /// Riot NA History to an account, e.g. !RiotEUW History [playername]
        /// </summary>
        [Command("RiotEUW History")]
        [Summary("Get most recent 10 games of a player")]
        public async Task MatchHistoryEUWAsync(string summonername)
        {
            string message = await MatchHistoryAsync(summonername, PlatformRoute.EUW1, RegionalRoute.EUROPE);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Riot NA History to an account, e.g. !RiotEUNE History [playername]
        /// </summary>
        [Command("RiotEUNE History")]
        [Summary("Get most recent 10 games of a player")]
        public async Task MatchHistoryEUNEAsync(string summonername)
        {
            string message = await MatchHistoryAsync(summonername, PlatformRoute.EUN1, RegionalRoute.EUROPE);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Riot NA History to an account, e.g. !RiotNA History [playername]
        /// </summary>
        [Command("RiotNA History")]
        [Summary("Get most recent 10 games of a player")]
        public async Task MatchHistoryNAsync(string summonername)
        {
            string message = await MatchHistoryAsync(summonername, PlatformRoute.NA1, RegionalRoute.EUROPE);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Match History Asynchronous
        /// </summary>
        public async Task<string> MatchHistoryAsync(string name, PlatformRoute pr, RegionalRoute rr)
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

        #endregion

        #region Register Commands

        /// <summary>
        /// Register EUW Account to a Discord Player
        /// </summary>
        [Command("registereuw")]
        [Summary("Register to leaderboard for Ranked Solo")]
        [RequireRoleAttribute(DiscordBot.BOYZ)]
        public async Task RegisterPlayerEUW(string name)
        {
            string message = await RegisterPlayerAsync(name, PlatformRoute.EUW1, Context.User.Id);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Register NA Account to a Discord Player
        /// </summary>
        [Command("registerna")]
        [Summary("Register to leaderboard for Ranked Solo")]
        [RequireRoleAttribute(DiscordBot.BOYZ)]
        public async Task RegisterPlayerNA(string name)
        {
            string message = await RegisterPlayerAsync(name, PlatformRoute.NA1, Context.User.Id);
            await ReplyAsync(message);
        }

        /// <summary>
        /// Register EUNE Account to a Discord Player
        /// </summary>
        [Command("registereune")]
        [Summary("Register to leaderboard for Ranked Solo")]
        [RequireRoleAttribute(DiscordBot.BOYZ)]
        public async Task RegisterPlayerEUNE(string name)
        {
            string message = await RegisterPlayerAsync(name, PlatformRoute.EUN1, Context.User.Id);
            await ReplyAsync(message);
        }

        /// <summary>
        /// Add Register Discord Player EUW Manually to leaderboards
        /// </summary>
        [Command("playeraddeuw")]
        [Summary("Register Player to Leaderboard with their id also")]
        public async Task PlayerAddEUW(string name, ulong id)
        {
            string message = await AddPlayer(name, PlatformRoute.EUW1, id);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Add Register Discord Player EUW Manually to leaderboards
        /// </summary>
        [Command("playeraddeune")]
        [Summary("Register Player to Leaderboard with their id also")]
        public async Task PlayerAddEUNE(string name, ulong id)
        {
            string message = await AddPlayer(name, PlatformRoute.EUN1, id);
            await ReplyAsync(message);
        }

        /// <summary>
        /// Add Register Discord Player EUW Manually to leaderboards
        /// </summary>
        [Command("playeraddna")]
        [Summary("Register Player to Leaderboard with their id also")]
        public async Task PlayerAddNA(string name, ulong id)
        {
            string message = await AddPlayer(name, PlatformRoute.NA1, id);
            await ReplyAsync(message);
        }

        /// <summary>
        /// Add Player to Registry
        /// </summary>
        public async Task<string> AddPlayer(string name, PlatformRoute pr, ulong id)
        {
            string message = "You are not Ethan. Feck off.";

            if (DiscordBot.IsEthan(Context.User.Id))
            {
                message = await RegisterPlayerAsync(name, pr, id);
            }

            return message;
        }

        /// <summary>
        /// Register Player with Platform 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pr"></param>
        /// <returns></returns>
        public async Task<string> RegisterPlayerAsync(string name, PlatformRoute pr, ulong user)
        {
            string message = string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                message = "Please Submit a name.";
            }
            else if (Discord.DiscordBot.RiotHandler.PManager.PlayerExists(user))
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
                        DiscordId = Context.User.Id,
                        SummonerName = summoner.Name,
                        AccountId = summoner.AccountId,
                        Id = summoner.Id,
                        ProfileIconId = summoner.ProfileIconId,
                        Route = pr,
                        Puuid = summoner.Puuid,
                        RevisionDate = summoner.RevisionDate
                    };
                    DiscordBot.RiotHandler.PManager.Players.Add(player);
                    await ReplyAsync(string.Format("Summoner {0} has been registered!", player.SummonerName));
                }
                else
                {
                    message = "Player does not exist, register another.";
                }
            }

            return message;
        }

        /// <summary>
        /// Remove Player
        /// </summary>
        [Command("remove")]
        [Summary("Remove User who called the command's Summoner.")]
        public async Task RemovePlayer()
        {
            List<Player> players = DiscordBot.RiotHandler.PManager.Players.Where(p => p.DiscordId == Context.User.Id).ToList();
            if (players.Count > 0)
            {
                DiscordBot.RiotHandler.PManager.Players.Remove(players[0]);
                await ReplyAsync(string.Format("Summoner {0} has been removed!", players[0].SummonerName));
            }
            else
            {
                await ReplyAsync("You do not have a Summoner Registered");


            }
        }

        #endregion
    }
}
