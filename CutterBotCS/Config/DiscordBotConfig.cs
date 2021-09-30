using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CutterBotCS.Config
{
    /// <summary>
    /// Discord Bot Config
    /// </summary>
    [DataContract]
    public class DiscordBotConfig
    {
        /// <summary>
        /// Token
        /// </summary>
        [DataMember]
        public string DiscordToken { get; set; }

        /// <summary>
        /// Prefix
        /// </summary>
        [DataMember]
        public char Prefix { get; set; }
         
        /// <summary>
        /// Riot API Token
        /// </summary>
        [DataMember]
        public string RiotAPIToken { get; set; }

        /// <summary>
        /// Set Properties
        /// </summary>
        public void SetProperties()
        {
            Properties.Settings.Default.RiotApiToken = RiotAPIToken;
            Properties.Settings.Default.CommandPrefix = Prefix;
            Properties.Settings.Default.DiscordToken = DiscordToken;
        }

        /// <summary>
        /// Get Properties
        /// </summary>
        public void GetProperties()
        {
            RiotAPIToken = Properties.Settings.Default.RiotApiToken;
            Prefix = Properties.Settings.Default.CommandPrefix;
            DiscordToken = Properties.Settings.Default.DiscordToken;
        }
    }
}
