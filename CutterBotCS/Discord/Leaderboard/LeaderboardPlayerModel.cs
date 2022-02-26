namespace CutterBotCS.Discord
{
    /// <summary>
    /// Player Model
    /// </summary>
    public class LeaderboardPlayerModel
    {
        /// <summary>
        /// Player's League of Legend Summoner Name
        /// </summary>
        public string SummonerName { get; set; }

        /// <summary>
        /// Player's League of Legend Puuid
        /// </summary>
        public string Puuid { get; set; }

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
