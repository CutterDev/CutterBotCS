using System.Text.Json.Serialization;

namespace CutterDragon.Champions
{
    public class Skin
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("isBase")]
        public bool IsBase { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("splashPath")]
        public string SplashPath { get; set; }

        [JsonPropertyName("uncenteredSplashPath")]
        public string UncenteredSplashPath { get; set; }

        [JsonPropertyName("tilePath")]
        public string TilePath { get; set; }

        [JsonPropertyName("loadScreenPath")]
        public string LoadScreenPath { get; set; }

        [JsonPropertyName("skinType")]
        public string SkinType { get; set; }

        [JsonPropertyName("rarity")]
        public string Rarity { get; set; }

        [JsonPropertyName("isLegacy")]
        public bool IsLegacy { get; set; }

        [JsonPropertyName("splashVideoPath")]
        public string SplashVideoPath { get; set; }

        [JsonPropertyName("collectionSplashVideoPath")]
        public string CollectionSplashVideoPath { get; set; }

        [JsonPropertyName("featuresText")]
        public string FeaturesText { get; set; }

        [JsonPropertyName("chromaPath")]
        public string ChromaPath { get; set; }

        [JsonPropertyName("chromas")]
        public Chroma[] Chromas { get; set; }

        [JsonPropertyName("emblems")]
        public object Emblems { get; set; }

        [JsonPropertyName("regionRarityId")]
        public int RegionRarityId { get; set; }

        [JsonPropertyName("rarityGemPath")]
        public string RarityGemPath { get; set; }

        [JsonPropertyName("skinLines")]
        public SkinLine[] SkinLines { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class SkinLine
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class Chroma
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("chromaPath")]
        public string ChromaPath { get; set; }

        [JsonPropertyName("colors")]
        public string[] Colors { get; set; }

        [JsonPropertyName("descriptions")]
        public ChromaDescription[] Descriptions { get; set; }

        [JsonPropertyName("rarities")]
        public ChromaRarity[] Rarities { get; set; }
    }

    public class ChromaDescription
    {
        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class ChromaRarity
    {
        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("rarity")]
        public int Rarity { get; set; }
    }
}
