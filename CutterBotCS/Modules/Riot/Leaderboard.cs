using CutterBotCS.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutterBotCS.Modules.Riot
{
    public class Leaderboard
    {
        /// <summary>
        /// Players SummonerName, EncryptedSummonerId
        /// </summary>
        public List<Player> Players { get; set; }

        /// <summary>
        /// m_FilePath
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public Leaderboard(string path)
        {
            FilePath = path;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            List<Player> players;

            if (File.Exists(FilePath))
            {
                if (JsonHelper.DeserializeFromFile(FilePath, out players))
                {
                    Players = players;
                }
            }
            else
            {
                Players = new List<Player>();

                JsonHelper.SerializeToFile(Players, FilePath);
            }

        }

        /// <summary>
        /// Save
        /// </summary>
        public void Save()
        {
            JsonHelper.SerializeToFile(Players, FilePath);
        }
    }

    /// <summary>
    /// Player
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Summoner Name
        /// </summary>
        public string SummonerName { get; set; }

        /// <summary>
        /// Account Id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Encrypted Summoner Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Profile Icon id
        /// </summary>
        public int ProfileIconId { get; set; }

        /// <summary>
        /// Puuid
        /// </summary>
        public string Puuid { get; set; }

        /// <summary>
        /// Revision Date
        /// </summary>
        public long RevisionDate { get; set; }

        /// <summary>
        /// Summoner Level
        /// </summary>
        public long SummonerLevel { get; set; }

        /// <summary>
        /// Player
        /// </summary>
        public Player()
        {
            SummonerName = string.Empty;
            AccountId = string.Empty;
            Id = string.Empty;
            Puuid = string.Empty;
        }
    }
}
