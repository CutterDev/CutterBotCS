using Camille.Enums;

namespace CutterBotCS.Discord
{
    /// <summary>
    /// Player
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Summoner Name
        /// </summary>
        public string SummonerName { get; set; }

        /// <summary>
        /// Platform Route
        /// </summary>
        public PlatformRoute PlatformRoute { get; set; }

        /// <summary>
        /// Regional Route
        /// </summary>
        public RegionalRoute RegionalRoute { get; set; }

        /// <summary>
        /// Player
        /// </summary>
        public Player()
        {
            SummonerName = string.Empty;
        }
    }
}
