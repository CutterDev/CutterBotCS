using CutterDB.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace CutterDB.Tables
{
    public class PlayersTable : ITable<PlayerEntity>
    {
        MySqlConnection m_SqlConnection { get; set; }

        const string INSERT_PLAYER = "INSERT INTO players VALUES (@param1, @param2, @param3, @param4, @param5)";

        const string SELECT_ALL_PLAYERS = "SELECT * FROM players WHERE guildid = @param1";

        const string SELECT_PLAYER = "SELECT * FROM players WHERE guildid = @param1 AND discordid = @param2";

        const string DELETE_PLAYER = "DELETE FROM players WHERE guildid = @param1 AND discordid = @param2";

        /// <summary>
        /// Open Connection to Database
        /// </summary>
        public void OpenConnection(string connstring, out string message)
        {
            message = string.Empty;
            try
            {
                m_SqlConnection = new MySqlConnection(connstring);
                m_SqlConnection.Open();
            }
            catch (Exception e)
            {
                message = string.Format("Error Connecting to Match History Table SQL: {0}", e.Message);
            }
        }

        /// <summary>
        /// Try Get Player
        /// </summary>
        public bool TryGetPlayer(ulong guildid, ulong discordid, out PlayerEntity entity, out string message)
        {
            bool result = false;
            entity = null;
            message = string.Empty;

            if (m_SqlConnection.State == ConnectionState.Open)
            {
                try
                {
                    using (MySqlCommand command = new MySqlCommand(SELECT_PLAYER, m_SqlConnection))
                    {
                        command.Parameters.AddWithValue("@param1", guildid);
                        command.Parameters.AddWithValue("@param2", discordid);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                entity = new PlayerEntity();
                                entity.DiscordId = reader.GetUInt64("discordid");
                                entity.GuildId = reader.GetUInt64("guildid");
                                entity.SummonerName = reader.GetString("summonername");
                                entity.RegionalRoute = reader.GetInt32("regionalroute");
                                entity.PlatformRoute = reader.GetInt32("platformroute");
                                result = true;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    message = string.Format("Error getting Player Entity: {0}", e.Message);
                    entity = null;
                }
            }


            return result;
        }

        /// <summary>
        /// Try Get Player
        /// </summary>
        public bool TryGetGuildPlayers(ulong guildid, out List<PlayerEntity> entities, out string message)
        {
            entities = new List<PlayerEntity>();
            message = string.Empty;

            if (m_SqlConnection.State == ConnectionState.Open)
            {
                try
                {
                    using (MySqlCommand command = new MySqlCommand(SELECT_ALL_PLAYERS, m_SqlConnection))
                    {
                        command.Parameters.AddWithValue("@param1", guildid);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var entity = new PlayerEntity();
                                entity.DiscordId = reader.GetUInt64("discordid");
                                entity.GuildId = reader.GetUInt64("guildid");
                                entity.SummonerName = reader.GetString("summonername");
                                entity.RegionalRoute = reader.GetInt32("regionalroute");
                                entity.PlatformRoute = reader.GetInt32("platformroute");

                                entities.Add(entity);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    message = string.Format("Error getting Player Entity: {0}", e.Message);
                }
            }

            return entities.Count > 0;
        }

        /// <summary>
        /// Insert Player
        /// </summary>
        public bool InsertPlayer(PlayerEntity entity, out string message)
        {
            bool rowinserted = false;
            message = string.Empty;
            string error = string.Empty;

            if (m_SqlConnection.State == ConnectionState.Open)
            {
                int rowseffected = 0;

                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(INSERT_PLAYER, m_SqlConnection))
                    {
                        cmd.Parameters.Add("@param1", MySqlDbType.UInt64).Value = entity.GuildId;
                        cmd.Parameters.Add("@param2", MySqlDbType.UInt64).Value = entity.DiscordId;
                        cmd.Parameters.Add("@param3", MySqlDbType.VarChar).Value = entity.SummonerName;
                        cmd.Parameters.Add("@param4", MySqlDbType.UInt64).Value = entity.RegionalRoute;
                        cmd.Parameters.Add("@param5", MySqlDbType.UInt64).Value = entity.PlatformRoute;

                        cmd.CommandType = CommandType.Text;
                        rowseffected = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                }

                rowinserted = rowseffected == 1;

                message = rowinserted ? string.Empty : "Player Entity was not inserted.";

                if (!string.IsNullOrWhiteSpace(error))
                {
                    message += string.Format(" Error inserting Player Entity Error: {0}", error);
                }
            }

            return rowinserted;
        }

        /// <summary>
        /// Insert Player
        /// </summary>
        public bool RemovePlayer(ulong guildid, ulong discordid, out string message)
        {
            bool rowinserted = false;
            message = string.Empty;
            string error = string.Empty;

            if (m_SqlConnection.State == ConnectionState.Open)
            {
                int rowseffected = 0;

                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(DELETE_PLAYER, m_SqlConnection))
                    {
                        cmd.Parameters.Add("@param1", MySqlDbType.UInt64).Value = guildid;
                        cmd.Parameters.Add("@param2", MySqlDbType.UInt64).Value = discordid;

                        cmd.CommandType = CommandType.Text;
                        rowseffected = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                }

                rowinserted = rowseffected == 1;

                message = rowinserted ? string.Empty : "Player was not deleted.";

                if (!string.IsNullOrWhiteSpace(error))
                {
                    message += string.Format(" Error deleting Player Error: {0}", error);
                }
            }

            return rowinserted;
        }


        /// <summary>
        /// Close Connection
        /// </summary>
        public void CloseConnection(out string message)
        {
            message = string.Empty;
            try
            {
                if (m_SqlConnection.State != ConnectionState.Closed)
                {
                    m_SqlConnection.Close();
                }
            }
            catch (Exception e)
            {
                message = string.Format("Error Closing SQL Connection: {0}", e.Message);
            }
        }
    }
}
