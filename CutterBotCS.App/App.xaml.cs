using System;
using System.IO;
using System.Windows;
using CutterBotCS.Config;
using CutterBotCS.Discord;
using CutterBotCS.Helpers;

namespace CutterBotCS.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string m_ConfigDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/CutterBot";
        private const string CONFIG_NAME = "config.json";

        /// <summary>
        /// Application Startup
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string configpath = Path.Combine(m_ConfigDir, CONFIG_NAME);

            // Config Dir
            if (!Directory.Exists(m_ConfigDir))
            {
                Directory.CreateDirectory(m_ConfigDir);
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

        /// <summary>
        /// Application Exit
        /// </summary>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            DiscordBotConfig config = new DiscordBotConfig();

            config.GetProperties();

            JsonHelper.SerializeToFile(config, Path.Combine(m_ConfigDir, CONFIG_NAME));
            
            DiscordBot.RiotHandler.PManager.Save();
        }
    }
}
