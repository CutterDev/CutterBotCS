using System.Collections.Generic;

namespace CutterBotCS.Discord
{
    /// <summary>
    /// Guild Leaderboard Model
    /// </summary>
    public class GuildLeaderboardModel
    {
        /// <summary>
        /// Guild Id
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// Channel Id
        /// </summary>
        public ulong ChannelId { get; set; }

        /// <summary>
        /// Players for Leaderboard
        /// </summary>
        public List<LeaderboardPlayerModel> Players { get; set; }
    }
}
