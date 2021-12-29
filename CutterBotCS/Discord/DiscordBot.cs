using CutterBotCS.Modules;
using CutterBotCS.RiotAPI;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public const long ETHAN = 217599819202560000;
        public const string BOYZ = "Boyz";
        public static APIHandler RiotHandler;
        private CommandHandler m_CommandHandler;
        private DiscordSocketClient m_Client;
        private CommandService m_Commands;
        private string m_Token;

        static string LEADERBOARD_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/CutterBot/players.json";

        public event ConnectedEventHandler connected;

        /// <summary>
        /// Ctor
        /// </summary>
        public DiscordBot()
        {
        }

        /// <summary>
        /// Main Async
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            m_Token = Properties.Settings.Default.DiscordToken;
            RiotHandler = new APIHandler(LEADERBOARD_PATH);
            m_Commands = new CommandService();

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
            m_CommandHandler = new CommandHandler(m_Client, m_Commands);
            await m_CommandHandler.InstallCommandsAsync();

            // Block this task until the program is closed
            await Task.Delay(-1);
        }

        /// <summary>
        /// Connected Event
        /// </summary>
        private Task Connected()
        {
            connected();

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
