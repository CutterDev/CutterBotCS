namespace CutterDB.Entities
{
    /// <summary>
    /// Player Entity
    /// </summary>
    public class PlayerEntity
    {
        /// <summary>
        /// Guild Id Player is from
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// Players Discord Id
        /// </summary>
        public ulong DiscordId { get; set; }

        /// <summary>
        /// Player's League of Legend Summoner Name
        /// </summary>
        public string SummonerName { get; set; }

        /// <summary>
        /// Regiuonal Route of League of Legends player
        /// </summary>
        public int RegionalRoute { get; set; }

        /// <summary>
        /// Platform Route of Legend of Legends player
        /// </summary>
        public int PlatformRoute { get; set; }
    }
}
