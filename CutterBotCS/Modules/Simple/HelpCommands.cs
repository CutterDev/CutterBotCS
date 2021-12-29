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
        public static string GetHelpList()
        {
            var prefix = Properties.Settings.Default.CommandPrefix;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(prefix + "Leaderboard - Pearlsayah Leaderboard for Solo Ranked");
            sb.AppendLine(prefix + "Riot Mastery [Players Name] - Gets Top 10 Champions for Player");
            sb.AppendLine(prefix + "RiotEUW History [Players Name] - Gets 10 recent games of a player in EUW");
            sb.AppendLine(prefix + "RiotEUNE History [Players Name] - Gets 10 recent games of a player in EUNE");
            sb.AppendLine(prefix + "RiotNA History [Players Name] - Gets 10 recent games of a player in NA");
            sb.AppendLine(prefix + "registereuw [Players Name] - Register Summoner to use for Leaderboard and info from euw");
            sb.AppendLine(prefix + "registereune [Players Name] - Register Summoner to use for Leaderboard and info from eune");
            sb.AppendLine(prefix + "registerna [Players Name] - Register Summoner to use for Leaderboard and info from na");
            sb.AppendLine(prefix + "Mastery - Gets Top 10 Champions of a Summoner registed");
            sb.AppendLine(prefix + "remove - Remove Summoner tied to the user");
            sb.AppendLine(prefix + "Hello - Umaru says Hello!");

            return sb.ToString();
        }
    }
}
