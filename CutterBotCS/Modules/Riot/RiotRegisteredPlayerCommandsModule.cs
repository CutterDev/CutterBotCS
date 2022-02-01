using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace CutterBotCS.Modules.Riot
{
    /// <summary>
    /// Riot Registed Player Commands Module
    /// </summary>
    public class RiotRegisteredPlayerCommandsModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Leaderboard no longer is draw from command. Leaderboard.cs handles it
        /// </summary>
        /// <returns></returns>
        [Command("leaderboard")]
        [Summary("Due to too much spam inbetween convos of said bot. Use #Leaderboard to see the stats.")]
        [RequireUserPermission(ChannelPermission.SendMessages, Group = "boyz")]
        public async Task LeaderboardCommandAsync()
        {
            string channel = Properties.Settings.Default.DiscordLeaderboardChannelId > 0 ? string.Format("<#{0}>", Properties.Settings.Default.DiscordLeaderboardChannelId)
                                                                                         : "Unknown";

            await ReplyAsync(string.Format("Due to too much spam inbetween convos of said bot. Use {0} to see the stats.", channel));
        }

        /// <summary>
        /// Get Masteries for Registed Player
        /// </summary>      
        [Command("Mastery")]
        [Summary("Gets top 10 Champion Masteries for Registered Player")]
        public async Task RegisteredMasteries()
        {
            string message = await RiotCommandHelper.GetRegisteredPlayerMasteryAsync(Context.User.Id);

            await ReplyAsync(message);
        }   

        /// <summary>
        /// Get History for Registered Player
        /// </summary>
        /// <returns></returns>
        [Command("History")]
        [Summary("Get most recent 10 games for registered Player")]
        public async Task RegisteredMatchHistoryAsync()
        {
            EmbedBuilder message = await RiotCommandHelper.GetRegisteredPlayerHistoryAsync(Context.User.Id);

            await ReplyAsync(embed: message.Build());
        }
    }
}
