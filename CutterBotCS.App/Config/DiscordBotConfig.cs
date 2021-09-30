using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutterBotCS.Config
{
    /// <summary>
    /// Discord Bot Config
    /// </summary>
    public class DiscordBotConfig
    {
        /// <summary>
        /// API Token
        /// </summary>
        public string APIToken { get; set; }

        /// <summary>
        /// Prefix
        /// </summary>
        public char Prefix { get; set; }

        /// <summary>
        /// Riot API
        /// </summary>
        public string RiotAPI { get; set; }
    }
}
