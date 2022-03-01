using CutterBotCS.Discord;
using CutterBotCS.RiotAPI;
using CutterBotCS.Worker;
using Discord.Commands;
using System;
using System.Collections.Generic;
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
            var discordid = Context.User.Id;

            DiscordWorker.Log(string.Format("User: {0} used match History", discordid), LogType.Info);
            if (DiscordBot.MessageHandler.ContainsMessage(discordid))
            {
                await ReplyAsync("You already used match history in the last 1 minute! Either select a match or wait.");
            }
            else
            {
                KeyValuePair<ulong, MatchHistoryEmbedModel> model = await m_RiotCommandHandler.GetRegisteredPlayerHistoryAsync(discordid, Context.Guild.Id);

                if (model.Value != null)
                {
                    var usermessage = await ReplyAsync(embed: model.Value.Embed.Build());

                    // Get Client, Guild Id, TextChannel Id, Message Id
                    // where the message came from
                    model.Value.GuildId = Context.Guild.Id;
                    model.Value.HistoryTextChannelId = Context.Channel.Id;
                    model.Value.HistoryMessageId = usermessage.Id;
                    model.Value.Timestamp = DateTime.Now;

                    DiscordBot.MessageHandler.AddNewMessageModel(model.Key, model.Value);
                }
                else
                {
                    await ReplyAsync("Error occured please contact CutterHealer#001");
                }
            }

        }

        [Command("SelectMatch")]
        [Summary("Select Match History from the recent games displayed")]
        public async Task SelectMatchAsync(int matchnumber)
        {
            if (matchnumber > 0 && matchnumber < 11)
            {
                MatchHistoryEmbedModel model;
                if (DiscordBot.MessageHandler.TryGetModel(Context.User.Id, out model))
                {
                    try
                    {
                        var embedbuilder = await m_RiotCommandHandler.GetMatch(model.MatchIds[matchnumber - 1], model.RegionalRoute);
       
                        await ReplyAsync(embed: embedbuilder.Build());
                    }
                    catch (Exception e)
                    {
                        DiscordWorker.Log(string.Format("Error Making Match Model: {0}",e.Message), LogType.Error);
                    }
                }
                else
                {
                    await ReplyAsync("Please use !history first before selecting a match");
                }
            }
            else
            {
                await ReplyAsync("Please select a match between 1 and 10");
            }
        }
    }
}
