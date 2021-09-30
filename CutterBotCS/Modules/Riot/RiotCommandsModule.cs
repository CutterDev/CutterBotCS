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

namespace CutterBotCS.Modules.Riot
{
    public class RiotCommandsModule : ModuleBase<SocketCommandContext>
    {

        [Command("Riot Mastery")]
        [Summary("Gets top 10 Champion Masteries of a player")]
        public Task ChampionsAsync(string summonerid)
        {
            StringBuilder message = new StringBuilder();
            message.Append("No Champions for this Summoner found try again.");
            if (!string.IsNullOrWhiteSpace(summonerid))
            {
                List<string> champions = DiscordBot.RiotHandler.GetTopSummonerMasteries(summonerid);
                if (champions.Count > 0)
                {
                    message.Clear();
                    message.AppendLine(string.Format("== {0} Top 10 Champion Masteries ==", summonerid));
                    foreach (string champion in champions)
                    {
                        message.AppendLine(champion);
                    }

                }
            }
            return ReplyAsync(message.ToString());
        }

        [Command("Riot History")]
        [Summary("Get most recent 10 games of a player")]
        public async Task MatchHistoryAsync(string summonerid)
        {
            StringBuilder message = new StringBuilder();
            message.Append("No Match History for this Summoner found try again.");
            if (!string.IsNullOrWhiteSpace(summonerid))
            {
                List<string> history = await DiscordBot.RiotHandler.GetRankedHistoryAsync(summonerid);
                if (history.Count > 0)
                {
                    message.Clear();
                    message.AppendLine(string.Format("== {0} 10 Most Recent Ranked Games ==", summonerid));
                    foreach (string match in history)
                    {
                        message.AppendLine(match);
                    }

                }
            }
            await ReplyAsync(message.ToString());
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

        [Command("register")]
        [Summary("Register to leaderboard for Ranked Solo")]
        [RequireRoleAttribute(DiscordBot.BOYZ)]
        public async Task RegisterLeaderboard(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                await ReplyAsync("Please Submit a name.");
            }
            else if (DiscordBot.RiotHandler.Leaderboards.Players.Where(p => p.SummonerName.ToLower() == name.ToLower()).Count() > 0)
            {
                await ReplyAsync("Summoner already Exists");
            }
            else
            {
                DiscordBot.RiotHandler.Leaderboards.Players.Add(new Player() { SummonerName = name });

                await ReplyAsync(string.Format("Summoner {0} has been registered! Welcome to the leaderboard", name));
            }
        }

        [Command("remove")]
        [Summary("Remove Summoner from leaderboard for Ranked Solo")]
        [RequireRole("Mod")]
        public async Task RemoveLeaderboard(string name)
        {
            if (Context.User.Id == DiscordBot.ETHAN)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    await ReplyAsync("Please pick a name to remove");
                }
                else
                {
                    DiscordBot.RiotHandler.Leaderboards.Players.Remove(DiscordBot.RiotHandler.Leaderboards.Players.Where(p => p.SummonerName.ToLower() == name.ToLower()).First());

                    await ReplyAsync(string.Format("Summoner {0} has been removed!", name));
                }
            }
            else
            {
                await ReplyAsync("YOU ARE NOT ETHAN. FECK OFF!");
            }
        }
    }
}
