using CutterBotCS.Discord;
using CutterBotCS.Helpers;
using CutterDragon;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace CutterBotCS.Worker
{
    /// <summary>
    /// Discord Worker Background Service
    /// </summary>
    public class DiscordWorker : BackgroundService
    {
        private DiscordBotManager m_DiscordBotManager;
        private CutterDragonWorker CutterDragon;
        private static ILogger<DiscordWorker> Logger;
        private static ConcurrentQueue<LogItem> m_LogQueue;
        private System.Timers.Timer m_LogTimer;

        /// <summary>
        /// Construction
        /// </summary>
        public DiscordWorker(ILogger<DiscordWorker> logger)
        {
            Logger = logger;
            m_LogQueue = new ConcurrentQueue<LogItem>();

            m_LogTimer = new System.Timers.Timer();
            m_LogTimer.Interval = 10000;
            m_LogTimer.Elapsed += LogTimer_Elapsed;
        }

        /// <summary>
        /// Start
        /// </summary>
        public void Start()
        {
            string discordbottoken;
            string riotapitoken;
            if (TryGetConfig(out discordbottoken, out riotapitoken))
            {
                CutterDragon = new CutterDragonWorker();
                CutterDragon.Initialize();

                m_DiscordBotManager = new DiscordBotManager(CutterDragon);
                m_DiscordBotManager.Initialise(discordbottoken, riotapitoken);
            }

            m_LogTimer.Start();

            // Allow the log timer to call once to see if any config issues occured
            LogTimer_Elapsed(null, null);
        }

        /// <summary>
        /// Try Get Bot Config
        /// </summary>
        public bool TryGetConfig(out string discordbottoken, out string riotapitoken)
        {
            bool result = false;
            discordbottoken = string.Empty;
            riotapitoken = string.Empty;

            BotConfig config;
            if (JsonHelper.DeserializeFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"), out config))
            {
                result = true;
                discordbottoken = config.DiscordBotToken;
                riotapitoken = config.RiotApiToken;
                Properties.Settings.Default.LeagueDBConn = config.LeagueDBConn;
                Properties.Settings.Default.BotDBConn = config.BotDBConn;
            }
            else
            {
                Log("ERROR GETTING CONFIG. BOT DID NOT START.", LogType.Error);
            }

            return result;
        }

        /// <summary>
        /// Log Timer Elapsed Event
        /// </summary>
        private void LogTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_LogTimer.Stop();

            LogItem li;
            while (m_LogQueue.TryDequeue(out li))
            {
                Log(li);
            }

            m_LogTimer.Start();
        }

        /// <summary>
        /// Log
        /// </summary>
        private void Log(LogItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Message))
            {
                switch (item.LogType)
                {
                    case LogType.Info:
                        Logger.LogInformation(item.ToString());
                        break;

                    case LogType.Warning:
                        Logger.LogWarning(item.ToString());
                        break;

                    case LogType.Error:
                        Logger.LogError(item.ToString());
                        break;
                }
            }
        }

        /// <summary>
        /// Excute A Sync Discord Worker Job
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(new TimeSpan(1, 0, 0, 0, 0));
            }
        }

        /// <summary>
        /// Log
        /// </summary>
        public static void Log(string message, LogType type)
        {
            LogItem logitem = new LogItem(message, type);
            m_LogQueue.Enqueue(logitem);
        }
    }
}
