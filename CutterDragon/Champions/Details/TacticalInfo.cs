using System.Text.Json.Serialization;

namespace CutterDragon.Champions
{
    public class TacticalInfo
    {
        [JsonPropertyName("style")]
        public int Style { get; set; }

        [JsonPropertyName("difficulty")]
        public int Difficulty { get; set; }

        [JsonPropertyName("damageType")]
        public string DamageType { get; set; }
    }
}
