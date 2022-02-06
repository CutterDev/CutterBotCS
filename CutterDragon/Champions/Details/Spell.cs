using System.Text.Json.Serialization;

namespace CutterDragon.Champions
{
    public class Spell
    {
        [JsonPropertyName("spellKey")]
        public string SpellKey { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("abilityIconPath")]
        public string AbilityIconPath { get; set; }

        [JsonPropertyName("abilityVideoPath")]
        public string AbilityVideoPath { get; set; }

        [JsonPropertyName("abilityVideoImagePath")]
        public string AbilityVideoImagePath { get; set; }

        [JsonPropertyName("cost")]
        public string Cost { get; set; }

        [JsonPropertyName("cooldown")]
        public string Cooldown { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("dynamicDescription")]
        public string DynamicDescription { get; set; }

        [JsonPropertyName("range")]
        public double[] Range { get; set; }

        [JsonPropertyName("costCoefficients")]
        public double[] CostCoefficients { get; set; }

        [JsonPropertyName("coolDownCoefficients")]
        public double[] CoolDownCoefficients { get; set; }

        [JsonPropertyName("coefficients")]
        public SpellCoefficient Coefficients { get; set; }

        [JsonPropertyName("effectAmounts")]
        public SpellEffectAmount EffectAmounts { get; set; }

        [JsonPropertyName("ammo")]
        public SpellAmmo Ammo { get; set; }

        [JsonPropertyName("maxLevel")]
        public int MaxLevel { get; set; }
    }

    public class SpellCoefficient
    {
        [JsonPropertyName("coefficient1")]
        public double Coefficient1 { get; set; }

        [JsonPropertyName("coefficient2")]
        public double Coefficient2 { get; set; }
    }

    public class SpellEffectAmount
    {
        [JsonPropertyName("Effect1Amount")]
        public double[] Effect1Amount { get; set; }

        [JsonPropertyName("Effect2Amount")]
        public double[] Effect2Amount { get; set; }

        [JsonPropertyName("Effect3Amount")]
        public double[] Effect3Amount { get; set; }

        [JsonPropertyName("Effect4Amount")]
        public double[] Effect4Amount { get; set; }

        [JsonPropertyName("Effect5Amount")]
        public double[] Effect5Amount { get; set; }

        [JsonPropertyName("Effect6Amount")]
        public double[] Effect6Amount { get; set; }

        [JsonPropertyName("Effect7Amount")]
        public double[] Effect7Amount { get; set; }

        [JsonPropertyName("Effect8Amount")]
        public double[] Effect8Amount { get; set; }

        [JsonPropertyName("Effect9Amount")]
        public double[] Effect9Amount { get; set; }

        [JsonPropertyName("Effect10Amount")]
        public double[] Effect10Amount { get; set; }


    }

    public class SpellAmmo
    {
        [JsonPropertyName("ammoRechargeTime")]
        public double[] AmmoRechargeTime { get; set; }

        [JsonPropertyName("maxAmmo")]
        public int[] MaxAmmo { get; set; }
    }
}
