namespace CutterDB.Entities
{
    /// <summary>
    /// Perk stats for Participants
    /// </summary>
    public class ParticipantPerkStatsEntity
    {
        /// <summary>
        /// Match Id
        /// </summary>
        public string MatchId { get; set; }

        /// <summary>
        /// Participant Id
        /// </summary>
        public int ParticipantId { get; set; }

        /// <summary>
        /// Perks overall Defence Stat
        /// </summary>
        public int PerksStatDefence { get; set; }

        /// <summary>
        /// Perks Overall Flex Stat
        /// </summary>
        public int PerksStatFlex { get; set; }

        /// <summary>
        /// Perk Overall Offence Stat
        /// </summary>
        public int PerksStatOffence { get; set; }

        /// <summary>
        /// Perk Style 1 Json
        /// </summary>
        public string PerkStyle1Json { get; set; }

        /// <summary>
        /// Perk Style 2 Json
        /// </summary>
        public string PerkStyle2Json { get; set; }
    }
}
