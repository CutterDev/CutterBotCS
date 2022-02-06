using System.Text.Json.Serialization;

namespace CutterDragon.Champions
{
    public class ChampionSummary
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Alias
        /// </summary>
        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Square Portait Path 
        /// /lol-game-data/assets/v1/champion-icons/[id].png
        /// /lol-game-data-assets/{path} mapped to
        /// Paths to Communitydragon as https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/{path}
        /// https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-icons/[id].png
        /// </summary>
        [JsonPropertyName("squarePortraitPath")]
        public string SquarePortraitPath { get; set; }

        /// <summary>
        /// Champion Roles
        /// </summary>
        [JsonPropertyName("roles")]
        public string[] Roles { get; set; }

        /// <summary>
        /// Get Icon URL for Data Dragon
        /// </summary>
        public string GetIconURL()
        {
            string lolpath = SquarePortraitPath.Replace(CutterDragonWorker.LOL_GAME_PATH, string.Empty);
            string cdatapath = CutterDragonWorker.CDATA_ASSET_PATH + lolpath;

            return cdatapath;
        }
    }
}
