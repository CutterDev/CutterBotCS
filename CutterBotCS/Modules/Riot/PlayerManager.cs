using Camille.Enums;
using CutterBotCS.Helpers;
using System.Collections.Generic;
using System.IO;

namespace CutterBotCS.Modules.Riot
{
    public class PlayerManager
    {
        /// <summary>
        /// Players SummonerName, EncryptedSummonerId
        /// </summary>
        private List<Player> m_Players { get; set; }

        /// <summary>
        /// m_FilePath
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public PlayerManager(string path)
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
                    m_Players = players;
                }
            }
            else
            {
                m_Players = new List<Player>();

                JsonHelper.SerializeToFile(m_Players, FilePath);
            }

        }

        /// <summary>
        /// Get Players
        /// </summary>
        /// <returns></returns>
        public List<Player> GetPlayers()
        {
            return m_Players;
        }

        /// <summary>
        /// Player Exists
        /// </summary>
        public bool PlayerExists(ulong discordid)
        {
            bool result = false;
            foreach(Player player in m_Players)
            {
                if (player.DiscordId == discordid)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Try Get Player
        /// </summary>
        public bool TryGetPlayer(ulong discordid, out Player player)
        {
            bool result = false;
            player = null;

            foreach(Player p in m_Players)
            {
                if (p.DiscordId == discordid)
                {
                    player = p;
                    result = true;
                    break;
                }
            }
            
            return result;
        }

        /// <summary>
        /// Add new Player
        /// </summary>
        public void AddPlayer(Player player)
        {
            m_Players.Add(player);
            Save();
        }

        /// <summary>
        /// Remove Player
        /// </summary>
        public void RemovePlayer(Player player)
        {
            m_Players.Remove(player);
            Save();
        }

        /// <summary>
        /// Save
        /// </summary>
        public void Save()
        {
            JsonHelper.SerializeToFile(m_Players, FilePath);
        }
    }

    /// <summary>
    /// Player
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Discord Id
        /// </summary>
        public ulong DiscordId { get; set; }

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
        /// Platform Route
        /// </summary>
        public PlatformRoute PlatformRoute { get; set; }

        /// <summary>
        /// Regional Route
        /// </summary>
        public RegionalRoute RegionalRoute { get; set; }

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
