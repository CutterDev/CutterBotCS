using Camille.RiotGames.MatchV5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CutterBotCS.Modules.Riot.History
{
    /// <summary>
    /// History Match Model
    /// </summary>
    public class HistoryMatchModel
    {
        public HistoryMatchModel()
        {
            VictoryTeam = Camille.RiotGames.Enums.Team.Other;
            Team1 = new Dictionary<int, Participant>();
            Team2 = new Dictionary<int, Participant>();
            RankType = string.Empty;
        }

        public string RankType { get; set; }

        /// <summary>
        /// GameTime
        /// </summary>
        public TimeSpan GameTime { get; set; }

        /// <summary>
        /// Date Time the game started (loading screen)
        /// </summary>
        public DateTime GameDate { get; set; }

        /// <summary>
        /// Winning Team
        /// </summary>
        public Camille.RiotGames.Enums.Team VictoryTeam { get; set; }

        /// <summary>
        /// Team 1
        /// </summary>
        public Team Team1Info { get; set; }

        /// <summary>
        /// Team 2 Info
        /// </summary>
        public Team Team2Info { get; set; }

        /// <summary>
        /// Team 1
        /// </summary>
        public Dictionary<int, Participant> Team1 { get; set; }

        /// <summary>
        /// Team 2
        /// </summary>
        public Dictionary<int, Participant> Team2 { get; set; }
    }
}
