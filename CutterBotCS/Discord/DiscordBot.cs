using CutterBotCS.Modules;
using CutterBotCS.Worker;
using CutterDB.Entities;
using CutterDB.Tables;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CutterBotCS.Discord
{
    //custom delegate  
    public delegate void ConnectedEventHandler();

    /// <summary>
    /// Discord Bot
    /// </summary>
    public class DiscordBot
    {
        private readonly string m_DiscordToken;
        private DiscordSocketClient m_SocketClient;
        private CommandHandler m_BotCommandHandler; 
        private CommandService m_BotCommandService;
        private IServiceCollection m_ServiceProvider;
        private Leaderboard m_Leaderboard;
        private bool m_Initialized;

        public static MessageHandler MessageHandler;

        /// <summary>
        /// Ctor
        /// </summary>
        public DiscordBot(string discordbottoken, Leaderboard leaderboard)
        {
            m_DiscordToken = discordbottoken;
            m_Leaderboard = leaderboard;
            m_Initialized = false;

            MessageHandler = new MessageHandler();
        }

        /// <summary>
        /// Main Async
        /// </summary>
        /// <returns></returns>
        public async Task Initialize(IServiceCollection serviceprovider)
        {
            m_ServiceProvider = serviceprovider;

            DiscordWorker.Log(string.Format("Initializing Bot"), LogType.Info);
            m_BotCommandService = new CommandService(new CommandServiceConfig { DefaultRunMode = RunMode.Async });

            m_BotCommandService.Log += LogHandlerAsync;

            //
            // Init Client
            //
            var config = new DiscordSocketConfig
            {
                AlwaysDownloadUsers = false,
                MessageCacheSize = 100,                   
            };

            m_SocketClient = new DiscordSocketClient();

            m_SocketClient.Log += Log;
            m_SocketClient.Connected += Connected;
            m_SocketClient.JoinedGuild += BotJoinedGuild;  

            await m_SocketClient.LoginAsync(TokenType.Bot, m_DiscordToken);
            await m_SocketClient.StartAsync();

            //
            // Handles Logging Messages
            //
            MessageHandler.Initialise(m_SocketClient);
        }

        /// <summary>
        /// Connected Event
        /// </summary>
        private async Task Connected()
        {
            if (!m_Initialized)
            {
                m_Initialized = true;
                DiscordWorker.Log("CONNECTED BOT!", LogType.Info);
                await GetGuildsAsync();
            }
        }

        /// <summary>
        /// Discord Bot Log Event
        /// </summary>
        private Task Log(LogMessage arg)
        {
            DiscordWorker.Log(arg.Message, LogType.Info);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Bot Joined Guild
        /// </summary>
        private Task BotJoinedGuild(SocketGuild arg)
        {
            GuildTable gt = new GuildTable();
            string errormsg;
            gt.OpenConnection(Properties.Settings.Default.BotDBConn, out errormsg);
            DiscordWorker.Log(errormsg, LogType.Info);

            gt.InsertGuild(new GuildEntity() { GuildId = arg.Id }, out errormsg);
            DiscordWorker.Log(errormsg, LogType.Info);

            gt.CloseConnection(out errormsg);
            DiscordWorker.Log(errormsg, LogType.Info);

            m_BotCommandHandler.AddNewGuild(arg.Id, GuildItem.DEFAULT_PREFIX);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Get Guilds Async
        /// </summary>
        private async Task GetGuildsAsync()
        {

            GuildTable gt = new GuildTable();
            string guilderr;
            gt.OpenConnection(Properties.Settings.Default.BotDBConn, out guilderr);
            DiscordWorker.Log(guilderr, LogType.Error);

            List<GuildItem> guilds = new List<GuildItem>();
            Dictionary<ulong, char> guildprefixes = new Dictionary<ulong, char>();
            foreach (var guild in m_SocketClient.Guilds)
            {
                GuildEntity entity;
                GuildItem gi = new GuildItem();

                if (gt.TryGetGuild(guild.Id, out entity, out guilderr))
                {
                    gi.Id = entity.Id;
                    gi.GuildId = entity.GuildId;
                    gi.Prefix = entity.Prefix;
                    gi.LeaderboardChannelId = entity.TCLeaderboardId;
                    gi.LeaderboardMessageId = entity.LeaderboardLatestMessageId;
                    gi.LeaderboardTitle = entity.LeaderboardTitle;
                }
                else
                {
                    gi.GuildId = guild.Id;
                    gi.Prefix = GuildItem.DEFAULT_PREFIX;

                    // Check guild does NOT exist
                    if (!gt.GuildExists(guild.Id, out guilderr))
                    {
                        // enter a new guildentity into table
                        entity = new GuildEntity()
                        {
                            GuildId = guild.Id,
                            Prefix = GuildItem.DEFAULT_PREFIX
                        };

                        gt.InsertGuild(entity, out guilderr);
                    }
                }

                DiscordWorker.Log(guilderr, LogType.Info);

                guildprefixes.Add(gi.GuildId, gi.Prefix);
                guilds.Add(gi);
            }

            gt.CloseConnection(out guilderr);
            DiscordWorker.Log(guilderr, LogType.Error);

            m_Leaderboard.Initialize(guilds, this);

            // 
            // Init CommandHandler
            //
            m_BotCommandHandler = new CommandHandler(m_SocketClient, m_BotCommandService, m_ServiceProvider.BuildServiceProvider(), guildprefixes);
            await m_BotCommandHandler.InstallCommandsAsync();
        }

        /// <summary>
        /// Get SocketTextChannel
        /// </summary>
        public bool TryGetSocketTextChannel(ulong guildid, ulong socketchannelid, out SocketTextChannel stc)
        {
            stc = null;

            if (m_SocketClient.ConnectionState == ConnectionState.Connected)
            {
                var guild = m_SocketClient.GetGuild(guildid);

                if (guild != null)
                {
                    stc = guild.GetTextChannel(socketchannelid);
                }
            }

            return stc != null;
        }

        /// <summary>
        /// Log Handler
        /// </summary>
        public Task LogHandlerAsync(LogMessage logMessage)
        {
            if (logMessage.Exception is CommandException cmdEx)
            {
                DiscordWorker.Log($"{cmdEx.GetBaseException().GetType()} was thrown while executing {cmdEx.Command.Aliases.ToString()} in {cmdEx.Context.Channel} by {cmdEx.Context.User}.", LogType.Error);
            }

            return Task.CompletedTask;
        }
    }
}
