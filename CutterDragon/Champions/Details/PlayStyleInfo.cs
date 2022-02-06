using System.Text.Json.Serialization;

namespace CutterDragon.Champions
{
    public class PlayStyleInfo
    {
        [JsonPropertyName("damage")]
        public int Damage { get; set; }

        [JsonPropertyName("crowdControl")]
        public int CrowdControl { get; set; }

        [JsonPropertyName("mobility")]
        public int Mobility { get; set; }

        [JsonPropertyName("utility")]
        public int Utility { get; set; }
    }
}
