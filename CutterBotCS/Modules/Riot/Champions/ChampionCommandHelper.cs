using CutterDragon.Champions;
using Discord;
using System;
using System.Text.RegularExpressions;

namespace CutterDragon
{
    /// <summary>
    /// Champion Helper Class
    /// </summary>
    public class ChampionCommandHelper
    {
        /// <summary>
        /// Cutter Dragon
        /// </summary>
        private static CutterDragonWorker CutterDragon { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ChampionCommandHelper()
        {
            CutterDragon = new CutterDragonWorker();

            CutterDragon.Initialize();
        }

        /// <summary>
        /// Get Discord Embed Message for Champion Info
        /// </summary>
        public EmbedBuilder GetChampionInfo(string championname)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.Title = "ERROR";
            builder.Description = "Champion could not be found";

            ChampionModel model;
            if (CutterDragon.TryGetChampionModel(championname, out model))
            {
                builder.Author = new EmbedAuthorBuilder() { Name = "Umaru Bot [BETA]", IconUrl = "https://avatarfiles.alphacoders.com/140/140066.png" };
                // Name
                builder.Title = model.ChampionInfo.Name;

                // Title
                builder.Description = model.ChampionInfo.Title;

                // DataDragon URL Icon
                builder.ThumbnailUrl = model.ChampionSummary.GetIconURL();

                // Short Bio
                builder.AddField("Bio", model.ChampionInfo.ShortBio);

                // Passive
                builder.AddField(string.Format("Passive - {0}", model.ChampionInfo.Passive.Name), HTMLTagRemoval(model.ChampionInfo.Passive.Description));

                // Abilities Q W E R
                if (model.ChampionInfo.Spells != null)
                {
                    foreach (Spell spell in model.ChampionInfo.Spells)
                    {
                        var spelldesc = string.IsNullOrWhiteSpace(spell.DynamicDescription) ? spell.Description : spell.DynamicDescription;
                        builder.AddField(string.Format("{0} - {1}", spell.SpellKey.ToUpper(), spell.Name), HTMLTagRemoval(spelldesc));
                    }
                }

                builder.Footer = new EmbedFooterBuilder() { Text = "[BETA] STILL UNDER MAJOR CONSTRUCTION [BETA]" };
            }

            return builder;
        }

        /// <summary>
        /// Take Message add newlines for messages and remove all other html tags
        /// </summary>
        private string HTMLTagRemoval(string message)
        {
            string result = string.IsNullOrEmpty(message) ? string.Empty : Regex.Replace(message.Replace("<br>", Environment.NewLine), @"<.*?>", "").Trim();
            return result; 
        }
    }
}
