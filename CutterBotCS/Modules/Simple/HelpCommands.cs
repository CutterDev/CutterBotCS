using CutterBotCS.Config;
using CutterBotCS.Discord;
using CutterBotCS.Helpers;
using Discord.Commands;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CutterBotCS.Modules.Simple
{
    /// <summary>
    /// Help Commands
    /// </summary>
    public static class HelpCommands
    {
        /// <summary>
        /// Get Help List
        /// </summary>
        public static string GetHelpList(IEnumerable<CommandInfo> commandinfos)
        {
            var prefix = Properties.Settings.Default.CommandPrefix;
            StringBuilder sb = new StringBuilder();
            if (commandinfos != null)
            {
                foreach(CommandInfo ci in commandinfos)
                {
                    if (!string.IsNullOrWhiteSpace(ci.Name))
                    {
                        sb.AppendLine(prefix + ci.Name + ": " + (ci.Summary ?? "no description given"));
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Save Config and Json Helper
        /// </summary>
        public static void Save(string configdir, string configname)
        {
            DiscordBotConfig config = new DiscordBotConfig();

            config.GetProperties();

            JsonHelper.SerializeToFile(config, Path.Combine(configdir, configname));

            DiscordBot.RiotHandler.PManager.Save();
        }
    }
}
