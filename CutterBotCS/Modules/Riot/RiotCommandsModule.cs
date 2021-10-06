using CutterBotCS.Discord;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using Camille.RiotGames.LeagueV4;
using CutterBotCS.Helpers;
using Camille.RiotGames.SummonerV4;
using Camille.Enums;

namespace CutterBotCS.Modules.Riot
{
    public class RiotCommandsModule : ModuleBase<SocketCommandContext>
    {

        [Command("Riot Mastery")]
        [Summary("Gets top 10 Champion Masteries of a player")]
        public async Task UnregisteredMasteries(string summonername)
        {
            string message = string.Empty;
            if (string.IsNullOrWhiteSpace(summonername))
            {
                message = "Summoner ID empty";
            }
            else
            {
                List<string> champions = await DiscordBot.RiotHandler.GetTopSummonerMasteriesAsync(summonername);
                message = ChampionMasteries(champions, summonername);
            }

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

        [Command("RiotEUW History")]
        [Summary("Get most recent 10 games of a player")]
        public async Task MatchHistoryEUWAsync(string summonername)
        {
            string message = await MatchHistoryAsync(summonername, PlatformRoute.EUW1, RegionalRoute.EUROPE);

            await ReplyAsync(message);
        }

        [Command("RiotEUNE History")]
        [Summary("Get most recent 10 games of a player")]
        public async Task MatchHistoryEUNEAsync(string summonername)
        {
            string message = await MatchHistoryAsync(summonername, PlatformRoute.EUN1, RegionalRoute.EUROPE);

            await ReplyAsync(message);
        }

        [Command("RiotNA History")]
        [Summary("Get most recent 10 games of a player")]
        public async Task MatchHistoryNAAsync(string summonername)
        {
            string message = await MatchHistoryAsync(summonername, PlatformRoute.NA1, RegionalRoute.EUROPE);

            await ReplyAsync(message);
        }

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

        [Command("registereuw")]
        [Summary("Register to leaderboard for Ranked Solo")]
        [RequireRoleAttribute(DiscordBot.BOYZ)]
        public async Task RegisterPlayerEUW(string name)
        {
            string message = await RegisterPlayerAsync(name, PlatformRoute.EUW1);

            await ReplyAsync(message);
        }

        [Command("registerna")]
        [Summary("Register to leaderboard for Ranked Solo")]
        [RequireRoleAttribute(DiscordBot.BOYZ)]
        public async Task RegisterPlayerNA(string name)
        {
            string message = await RegisterPlayerAsync(name, PlatformRoute.NA1);
            await ReplyAsync(message);
        }

        [Command("registereune")]
        [Summary("Register to leaderboard for Ranked Solo")]
        [RequireRoleAttribute(DiscordBot.BOYZ)]
        public async Task RegisterPlayerEUNE(string name)
        {
            string message = await RegisterPlayerAsync(name, PlatformRoute.EUN1);
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

        /// <summary>
        /// Register Player with Platform 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pr"></param>
        /// <returns></returns>
        public async Task<string> RegisterPlayerAsync(string name, PlatformRoute pr)
        {
            string message = string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                message = "Please Submit a name.";
            }
            else if (Discord.DiscordBot.RiotHandler.PManager.PlayerExists(Context.User.Id))
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
    }
}
