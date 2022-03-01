using Camille.Enums;
using Discord;
using Discord.WebSocket;
using System;

namespace CutterBotCS.Discord
{
    /// <summary>
    /// Match History Embed Model
    /// </summary>
    public class MatchHistoryEmbedModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MatchHistoryEmbedModel()
        {
            MatchIds = new string[10];
        }

        /// <summary>
        /// Embed Builder for Match History
        /// </summary>
        public EmbedBuilder Embed { get; set; }

        /// <summary>
        /// League of Legend Match History Ids
        /// </summary>
        public string[] MatchIds { get; set; }

        /// <summary>
        /// Timestamp when Matches were requested
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Guild Id the message came from
        /// </summary>
        public ulong GuildId { get; set; }
 

        /// <summary>
        /// Keep Track of the History Message Id to delete
        /// </summary>
        public ulong HistoryMessageId { get; set; }

        /// <summary>
        /// The Channel the user used to look at history
        /// </summary>
        public ulong HistoryTextChannelId { get; set; }

        /// <summary>
        /// Regional Route
        /// </summary>
        public RegionalRoute RegionalRoute { get; set; }
    }
}
