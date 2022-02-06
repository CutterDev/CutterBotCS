namespace CutterDragon.Champions
{
    /// <summary>
    /// Champion Model used to display data to users
    /// </summary>
    public class ChampionModel
    {
        /// <summary>
        /// Champion Summary from champions.json file in CommunityDragon
        /// </summary>
        public ChampionSummary ChampionSummary { get; set; }

        /// <summary>
        /// Champion info from the [id].json file for each champion in Community Dragon
        /// </summary>
        public ChampionInfo ChampionInfo { get; set; }
    }
}
