namespace CutterBotCS.Discord
{
    /// <summary>
    /// Guild Item
    /// </summary>
    public class GuildItem
    {
        public const char DEFAULT_PREFIX = '!';

        /// <summary>
        /// Constructor
        /// </summary>
        public GuildItem()
        {
            Prefix = DEFAULT_PREFIX;
            LeaderboardTitle = "Leaderboard";
        }
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Guild Id
        /// </summary>
        public ulong GuildId { get; set; }  

        /// <summary>
        /// Leaderboard Channel Id
        /// </summary>
        public ulong LeaderboardChannelId { get; set; }    

        /// <summary>
        /// Leaderboard Message Id
        /// </summary>
        public ulong LeaderboardMessageId { get; set; }

        /// <summary>
        /// Leaderboard Title
        /// </summary>
        public string LeaderboardTitle { get; set; }

        /// <summary>
        /// Command Prefix
        /// </summary>
        public char Prefix { get; set; }
    }
}
