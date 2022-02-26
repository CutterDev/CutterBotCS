using CutterBotCS.RiotAPI;
using CutterBotCS.Worker;
using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace CutterBotCS.Modules.Riot
{
    /// <summary>
    /// Riot Registed Player Commands Module
    /// </summary>
    public class RiotRegisteredPlayerCommandsModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Riot Command Handler
        /// </summary>
        private RiotCommandHandler m_RiotCommandHandler { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RiotRegisteredPlayerCommandsModule(RiotAPIHandler handler)
        {
            m_RiotCommandHandler = new RiotCommandHandler(handler);
        }

        /// <summary>
        /// Leaderboard no longer is draw from command. Leaderboard.cs handles it
        /// </summary>
        /// <returns></returns>
        [Command("leaderboard")]
        [Summary("Due to too much spam inbetween convos of said bot. Use #Leaderboard to see the stats.")]
        public async Task LeaderboardCommandAsync()
        {
            await ReplyAsync("See leaderboardchannel to see the stats.");
        }

        /// <summary>
        /// Get Masteries for Registed Player
        /// </summary>      
        [Command("Mastery")]
        [Summary("Gets top 10 Champion Masteries for Registered Player")]
        public async Task RegisteredMasteries()
        {
            string message = "Cannot find masteries";

            try
            {
                message = await m_RiotCommandHandler.GetRegisteredPlayerMasteryAsync(Context.User.Id, Context.Guild.Id);
            }
            catch (Exception e)
            {
                DiscordWorker.Log(string.Format("Error getting mastery: {0}", e.Message), LogType.Error);
                message = "Error occured. Contact CutterHealer#0001";
            }

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
            EmbedBuilder message = await m_RiotCommandHandler.GetRegisteredPlayerHistoryAsync(Context.User.Id, Context.Guild.Id);

            await ReplyAsync(embed: message.Build());
        }
    }
}
