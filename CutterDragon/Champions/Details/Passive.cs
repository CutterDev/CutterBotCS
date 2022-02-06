using System.Text.Json.Serialization;

namespace CutterDragon.Champions
{
    public class Passive
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("abilityIconPath")]
        public string AbilityIconPath { get; set; }

        [JsonPropertyName("abilityVideoPath")]
        public string AbilityVideoPath { get; set; }

        [JsonPropertyName("abilityVideoImagePath")]
        public string AbilityVideoImagePath { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
