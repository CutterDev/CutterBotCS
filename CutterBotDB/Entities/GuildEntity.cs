using System;

namespace CutterDB.Entities
{
    /// <summary>
    /// Guild Entity
    /// </summary>
    public class GuildEntity
    {
        /// <summary>
        /// Bot's Guid to the Guild
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Guild Id
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// Date Inserted
        /// </summary>
        public DateTime DateInserted { get; set; }

        /// <summary>
        /// Prefix
        /// </summary>
        public char Prefix { get; set; }

        /// <summary>
        /// Text Channel Leaderboard Id
        /// </summary>
        public ulong TCLeaderboardId { get; set; }

        /// <summary>
        /// Latest Message Id for Leaderboard
        /// </summary>
        public ulong LeaderboardLatestMessageId { get; set; }

        /// <summary>
        /// Leaderboard Title
        /// </summary>
        public string LeaderboardTitle { get; set; }

        /// <summary>
        /// Last Modified
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}
