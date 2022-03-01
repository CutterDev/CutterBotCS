using Camille.RiotGames.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutterBotCS.Modules.Riot.History
{
    /// <summary>
    /// History Match Model
    /// </summary>
    public class HistoryMatchModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HistoryMatchModel()
        {
            WinningTeam = Team.Other;
        }

        /// <summary>
        /// Winning Team
        /// </summary>
        public Team WinningTeam { get; set; }
    }
}
