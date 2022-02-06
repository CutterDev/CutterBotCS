using System.Text.Json.Serialization;

namespace CutterDragon.Champions
{
    /// <summary>
    /// Champion Info
    /// </summary>
    public class ChampionInfo
    {
        /// <summary>
        /// Champion Id
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Olaf
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Alias
        /// </summary>
        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// Short Bio
        /// </summary>
        [JsonPropertyName("shortBio")]
        public string ShortBio { get; set; }

        /// <summary>
        /// Tractical Info
        /// </summary>
        [JsonPropertyName("tacticalInfo")]
        public TacticalInfo TacticalInfo { get; set; }

        /// <summary>
        /// Play Style Info
        /// </summary>
        [JsonPropertyName("playstyleInfo")]
        public PlayStyleInfo PlayStyleInfo { get; set; }

        /// <summary>
        /// Square Portrait Path File .png
        /// </summary>
        [JsonPropertyName("squarePortraitPath")]
        public string SquarePortraitPath { get; set; }

        /// <summary>
        /// Stingers SFX Sound File Path .ogg
        /// </summary>
        [JsonPropertyName("stingersSfxPath")]
        public string StingersSfxPath { get; set; }

        /// <summary>
        /// Choose Voice Over Sound File Path .ogg
        /// </summary>
        [JsonPropertyName("chooseVoPath")]
        public string ChooseVOPath { get; set; }

        /// <summary>
        /// Ban Voice Over Sound File Path .ogg
        /// </summary>
        [JsonPropertyName("banVoPath")]
        public string BanVoPath { get; set; }

        /// <summary>
        /// Roles
        /// </summary>
        [JsonPropertyName("roles")]
        public string[] Roles { get; set; }

        public string[] RecommendedItemDefaults { get; set; }

        /// <summary>
        /// Recommended Item Defaults
        /// </summary>
        public string[] recommendedItemDefaults { get; set; }

        /// <summary>
        /// Skins
        /// </summary>
        [JsonPropertyName("skins")]
        public Skin[] Skins { get; set; }

        /// <summary>
        /// Passive
        /// </summary>
        [JsonPropertyName("passive")]
        public Passive Passive { get; set; }

        /// <summary>
        /// Spells
        /// </summary>
        [JsonPropertyName("spells")]
        public Spell[] Spells { get; set; }
    }
}
