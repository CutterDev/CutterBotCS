using CutterBotCS.Config;
using CutterBotCS.Discord;
using CutterBotCS.Helpers;
using System;
using System.IO;

namespace CutterBotCS.ConsoleApp
{
    public class Program
    {
        public static DiscordBot m_DiscordBot;

        static void Main(string[] args) => new Program().Start();

        /// <summary>
        /// Start
        /// </summary>
        public async void Start()
        {
            m_DiscordBot = new DiscordBot(DiscordBot.CONFIG_DIR, DiscordBot.CONFIG_NAME);
            InitializeSettings();

            await m_DiscordBot.Initialize();
            string userinput = string.Empty;
            while (true)
            {
                
            }
        }

        /// <summary>
        /// Initialize Settings
        /// </summary>
        public void InitializeSettings()
        {
            var configdir = DiscordBot.CONFIG_DIR;
            var configname = DiscordBot.CONFIG_NAME;
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
        }
    }
}
