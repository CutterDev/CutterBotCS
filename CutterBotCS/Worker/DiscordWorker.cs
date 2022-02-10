using CutterBotCS.Config;
using CutterBotCS.Discord;
using CutterBotCS.Helpers;
using CutterDragon;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CutterBotCS.Worker
{
    /// <summary>
    /// Discord Worker Background Service
    /// </summary>
    public class DiscordWorker : BackgroundService
    {
        public static DiscordBot DiscordBot;
        private CutterDragonWorker CutterDragon;
        private static ILogger<DiscordWorker> Logger;

        /// <summary>
        /// Construction
        /// </summary>
        public DiscordWorker(ILogger<DiscordWorker> logger)
        {
            Logger = logger;          
        }

        /// <summary>
        /// Start
        /// </summary>
        public async void Start()
        {
            CutterDragon = new CutterDragonWorker();
            Logger.LogInformation("Starting up Bot");
            DiscordBot = new DiscordBot();
            InitializeSettings();

            await DiscordBot.Initialize();
        }

        /// <summary>
        /// Initialize Settings
        /// </summary>
        public void InitializeSettings()
        {
            var configdir = DiscordBot.CONFIG_DIR;
            var configname = DiscordBot.CONFIG_FILENAME;
            string configpath = Path.Combine(configdir, configname);

            // Config Dir
            if (!Directory.Exists(configdir))
            {
                Directory.CreateDirectory(configdir);
            }

            DiscordBotConfig config;
            if (File.Exists(configpath))
            {
                if (JsonHelper.DeserializeFromFile(configpath, out config))
                {
                    config.SetProperties();
                }
            }
            else
            {
                config = new DiscordBotConfig();
                config.GetProperties();

                JsonHelper.SerializeToFile(config, configpath);
            }

            CutterDragon.Initialize();
        }

        /// <summary>
        /// Log
        /// </summary>
        public static void Log(string message, LogType type)
        {
            switch(type)
            {
                case LogType.Info:
                    Logger.LogInformation(message);
                    break;

                case LogType.Warning:
                    Logger.LogWarning(message);
                    break;

                case LogType.Error:
                    Logger.LogError(message);
                    break;
            }
        }

        /// <summary>
        /// Excute A Sync Discord Worker Job
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                CutterDragon.GetAssets();
                await Task.Delay(new TimeSpan(1, 0, 0, 0, 0));
            }
        }
    }
    
    public enum LogType
    {
        Info,
        Warning,
        Error
    }
}
