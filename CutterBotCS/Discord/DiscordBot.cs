using CutterBotCS.Config;
using CutterBotCS.Helpers;
using CutterBotCS.Leaderboard;
using CutterBotCS.Modules;
using CutterBotCS.RiotAPI;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
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
        System.Timers.Timer LeaderboardTimer = new System.Timers.Timer();

        public const long ETHAN = 217599819202560000;
        public const string BOYZ = "Boyz";
        public static APIHandler RiotHandler;
        public static CommandService BotCommandService;
        CommandHandler m_BotCommandHandler;
        private DiscordSocketClient m_Client;
        private string m_Token;
        private string m_ConfigDir;
        private string m_ConfigName;
        private ulong m_CurrentLeaderboardMessage;
        private LeaderboardUICreator m_Leaderboard;

        static string LEADERBOARD_PATH = AppDomain.CurrentDomain.BaseDirectory + "/Configuration/players.json";
        public static string CONFIG_DIR = AppDomain.CurrentDomain.BaseDirectory + "/Configuration";
        public static string CONFIG_NAME = "config.json";

        public event ConnectedEventHandler connected;

        /// <summary>
        /// Ctor
        /// </summary>
        public DiscordBot(string configdir, string config)
        {
            m_ConfigDir = configdir;
            m_ConfigName = config;
        }

        /// <summary>
        /// Main Async
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            m_Token = Properties.Settings.Default.DiscordToken;
            RiotHandler = new APIHandler(LEADERBOARD_PATH);
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

            LeaderboardTimer.Elapsed += LeaderboardTimer_Elapsed;

            // 15 minutes
            LeaderboardTimer.Interval = 60000 * 10;
            LeaderboardTimer.Enabled = true;

            LeaderboardTimer_Elapsed(null, null);
        }

        private async void LeaderboardTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<LeaderboardEntry> boys = await DiscordBot.RiotHandler.GetBoyzLeagueAsync(EntryFilter.None);

            // change this shite.
            SocketGuild sg = m_Client.GetGuild(Properties.Settings.Default.DiscordGuildId);

            if (sg != null)
            {
                SocketTextChannel stc = sg.GetTextChannel(Properties.Settings.Default.DiscordLeaderboardChannelId);
                if (stc != null)
                {
                    await SendLeaderboard(boys, stc, false);
                }
            }
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

        public async Task SendLeaderboard(List<LeaderboardEntry> entries, SocketTextChannel channel, bool usepodium)
        {
            string image = AppDomain.CurrentDomain.BaseDirectory + "/Resources/Images/leaderboard.png";

            if (m_CurrentLeaderboardMessage > 0)
            {
                await channel.DeleteMessageAsync(m_CurrentLeaderboardMessage);
            }

            if (m_Leaderboard == null)
            {
                m_Leaderboard = new LeaderboardUICreator();
            }

            string errmessage;
            m_Leaderboard.Initialize();
            m_Leaderboard.CreateLeaderboard(entries, image, out errmessage, usepodium);
            RestUserMessage rest;
            if (string.IsNullOrWhiteSpace(errmessage))
            {
                rest = await channel.SendFileAsync(image, string.Empty);
            }
            else
            {
                rest = await channel.SendMessageAsync(errmessage);

            }

            m_CurrentLeaderboardMessage = rest.Id;
        }
    }
}
