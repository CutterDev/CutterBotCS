using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CutterDragon.Assets
{
    public class ItemSummary
    {
        /// <summary>
        /// Id of Item
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Name of Item
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Description of Item
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Is Item Activatable
        /// </summary>
        [JsonPropertyName("active")]
        public bool Activatable { get; set; }

        /// <summary>
        /// Item is in store
        /// </summary>
        [JsonPropertyName("instore")]
        public bool InStore { get; set; }

        /// <summary>
        /// Items the item is made from
        /// </summary>
        [JsonPropertyName("from")]
        public int[] FromItems { get; set; }

        /// <summary>
        /// Items the item can be made into
        /// </summary>
        [JsonPropertyName("to")]
        public int[] ToItems { get; set; }

        /// <summary>
        /// Categories the Item comes under
        /// </summary>
        [JsonPropertyName("categories")]
        public string[] Categories { get; set; }

        /// <summary>
        /// Max amount of this item you can stack
        /// </summary>
        [JsonPropertyName("maxStacks")]
        public int MaxStacks { get; set; }

        /// <summary>
        /// Some Items require a champion to be played
        /// </summary>
        [JsonPropertyName("requiredChampion")]
        public string RequiredChampion { get; set; }


        /// <summary>
        /// Requires ally to use
        /// </summary>
        [JsonPropertyName("requiredAlly")]
        public string RequiredAlly { get; set; }

        /// <summary>
        /// Currency Needed to acquire this Item
        /// </summary>
        [JsonPropertyName("requiredBuffCurrencyName")]
        public string RequiredCurrencyName { get; set; }

        /// <summary>
        /// Required Currency Cost to acquire item
        /// </summary>
        [JsonPropertyName("requiredBuffCurrencyCost")]
        public int RequiredCurrencyCost { get; set; }

        /// <summary>
        /// Special Recipe Item
        /// </summary>
        [JsonPropertyName("specialRecipe")]
        public int SpecialRecipe { get; set; }

        /// <summary>
        /// Is Item Enchantment
        /// </summary>
        [JsonPropertyName("isEnchantment")]
        public bool IsEnchantment { get; set; }

        /// <summary>
        /// Price in Gold to acquire item with the other from items already bought
        /// </summary>
        [JsonPropertyName("price")]
        public int Price { get; set; }

        /// <summary>
        /// Price Total to acquire item
        /// </summary>
        [JsonPropertyName("priceTotal")]
        public int PriceTotal { get; set; }

        [JsonPropertyName("iconPath")]
        public string IconPath { get; set; }
    }
}
