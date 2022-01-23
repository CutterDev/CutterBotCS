using CutterBotCS.Modules;
using CutterBotCS.Modules.Leaderboard;
using CutterBotCS.RiotAPI;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
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
        private CommandHandler m_BotCommandHandler;
        private DiscordSocketClient m_Client;
        private Leaderboard m_Leaderboard;
        private string m_Token;
        private string m_PlayersPath;

        public static RiotAPIHandler RiotHandler;
        public static CommandService BotCommandService;
        public static string CONFIG_DIR;
        public static string CONFIG_FILENAME;
        public const long ETHAN = 217599819202560000;
        public const string BOYZ = "Boyz";

        public event ConnectedEventHandler connected;

        /// <summary>
        /// Ctor
        /// </summary>
        public DiscordBot()
        {
            m_PlayersPath = AppDomain.CurrentDomain.BaseDirectory + "/Configuration/players.json";
            CONFIG_DIR = AppDomain.CurrentDomain.BaseDirectory + "/Configuration";
            CONFIG_FILENAME = "config.json";
        }

        /// <summary>
        /// Main Async
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            m_Token = Properties.Settings.Default.DiscordToken;
            RiotHandler = new RiotAPIHandler(m_PlayersPath);
            BotCommandService = new CommandService();

            //
            // Init Riot Handler
            //
            RiotHandler.Initialize();

            //
            // Init Client
            //
            var config = new DiscordSocketConfig
            {
                AlwaysDownloadUsers = false,
                MessageCacheSize = 100
            };
            m_Client = new DiscordSocketClient();

            m_Client.Log += Log;
            m_Client.Connected += Connected;

            await m_Client.LoginAsync(TokenType.Bot, m_Token);
            await m_Client.StartAsync();

            // 
            // Init CommandHandler
            //
            m_BotCommandHandler = new CommandHandler(m_Client, BotCommandService);
            await m_BotCommandHandler.InstallCommandsAsync();

            //
            // Init Leaderboard
            //
            m_Leaderboard = new Leaderboard(RiotHandler, m_Client);
            m_Leaderboard.Initialize();
        }

        /// <summary>
        /// Connected Event
        /// </summary>
        private Task Connected()
        {
            if (connected != null)
            {
                connected();
            }


            return Task.CompletedTask;
        }

        /// <summary>
        /// Log
        /// </summary>
        private Task Log(LogMessage arg)
        {
            // TODO: Log Something

            return Task.CompletedTask;
        }

        /// <summary>
        /// Is Id Ethan
        /// </summary>
        public static bool IsEthan(ulong id)
        {
            return id == ETHAN;
        }
    }
}
