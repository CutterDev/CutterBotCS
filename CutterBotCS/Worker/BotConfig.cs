using System.Runtime.Serialization;

namespace CutterBotCS.Worker
{
    [DataContract]
    public class BotConfig
    {
        /// <summary>
        /// Discord Bot Token
        /// </summary>
        [DataMember]
        public string DiscordBotToken { get; set; }

        /// <summary>
        /// Riot Api Token
        /// </summary>
        [DataMember]
        public string RiotApiToken { get; set; }

        /// <summary>
        /// Bot Database Connection
        /// </summary>
        [DataMember]
        public string BotDBConn { get; set; }

        /// <summary>
        /// League Database Connection
        /// </summary>
        [DataMember]
        public string LeagueDBConn { get; set; }
    }
}
