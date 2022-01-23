using Camille.Enums;
using System;

namespace CutterBotCS.Modules.Leaderboard
{
    /// <summary>
    /// Leaderboard Entry
    /// </summary>
    public class LeaderboardEntry
    {
        /// <summary>
        /// Discord Id
        /// </summary>
        public ulong DiscordId { get; set; }

        /// <summary>
        /// League Id
        /// </summary>
        public string LeagueId { get; set; }

        /// <summary>
        /// League Points
        /// </summary>
        public int LeaguePoints { get; set; }

        /// <summary>
        /// Division
        /// </summary>
        public Division? Division { get; set; }

        /// <summary>
        /// Tier
        /// </summary>
        public Tier? Tier { get; set; }

        /// <summary>
        /// Losses
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Wins
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Summoner Id
        /// </summary>
        public string SummonerId { get; set; }

        /// <summary>
        /// Summoner Name
        /// </summary>
        public string SummonerName { get; set; }

        /// <summary>
        /// WinRate as Percentage
        /// </summary>
        public float WinRate
        {
            get
            {
                return (Wins / (float)(Losses + Wins)) * 100.0f;
            }
        }

        /// <summary>
        /// Total Games
        /// </summary>
        public float TotalGames
        {
            get
            {
                return Wins + Losses;
            }
        }

        /// <summary>
        /// LogoPath for Entry
        /// </summary>
        public string LogoPath 
        { 
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + string.Format("Resources/Images/Leaderboard/{0}.png", DiscordId);
            } 
        }
    }

    /// <summary>
    /// Entry Filter
    /// </summary>
    public enum EntryFilter
    {
        None,
        MostWins,
        MostLosses,
        MostGames,
        LowestWinRate,
        HighestWinRate
    }
}
