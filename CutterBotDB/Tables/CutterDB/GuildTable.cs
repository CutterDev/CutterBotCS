using CutterDB.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutterDB.Tables
{
    public class GuildTable : ITable<GuildEntity>
    {
        MySqlConnection m_SqlConnection { get; set; }

        const string GUILD_EXISTS = "SELECT COUNT(*) FROM guilds where guildid = @param1";

        const string INSERT_GUILD = "INSERT INTO guilds VALUES (@param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8)";

        const string SELECT_ALL_GUILDS = "SELECT * FROM guilds";

        const string SELECT_ONE_GUILD = "SELECT * FROM guilds WHERE guildid = @param1";

        const string UPDATE_GUILD = "UPDATE guilds SET prefix = @param2, tcleaderboard = @param3, lastmodified = @param4 WHERE guildid = @param1";

        const string UPDATE_LEADERBOARDMESSAGE_GUILD = "UPDATE guilds SET leaderboardmessage = @param2, lastmodified = @param3 WHERE guildid = @param1";

        const string UPDATE_LEADERBOARDTITLE_GUILD = "UPDATE guilds SET leaderboardtitle = @param2, lastmodified = @param3 WHERE guildid = @param1";

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
                message = string.Format("Error Connecting to guilds Table SQL: {0}", e.Message);
            }
        }

        /// <summary>
        /// Try get Guild with Guild id
        /// </summary>
        public bool TryGetGuild(ulong guildid, out GuildEntity entity, out string message)
        {
            bool result = false;
            entity = null;
            message = string.Empty;

            if (m_SqlConnection.State == ConnectionState.Open)
            {
                try
                {
                    using (MySqlCommand command = new MySqlCommand(SELECT_ONE_GUILD, m_SqlConnection))
                    {
                        command.Parameters.AddWithValue("@param1", guildid);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entity = GetEntityFromReader(reader);
                                result = true;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    message = string.Format("Error getting All Guild Entities: {0}", e.Message);
                }
            }


            return result;
        }

        /// <summary>
        /// Try Get All Guilds
        /// </summary>
        public bool TryGetAllGuilds(out List<GuildEntity> entities, out string message)
        {
            bool result = false;
            entities = new List<GuildEntity>();
            message = string.Empty;

            if (m_SqlConnection.State == ConnectionState.Open)
            {
                try
                {
                    using (MySqlCommand command = new MySqlCommand(SELECT_ALL_GUILDS, m_SqlConnection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entities.Add(GetEntityFromReader(reader));

                            }
                            result = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    message = string.Format("Error getting All Guild Entities: {0}", e.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// Check Guild Exists in table
        /// </summary>
        public bool GuildExists(ulong guildid, out string message)
        {
            bool result = false;
            message = string.Empty;

            if (m_SqlConnection.State == ConnectionState.Open)
            {
                try
                {
                    using (MySqlCommand command = new MySqlCommand(GUILD_EXISTS, m_SqlConnection))
                    {
                        command.Parameters.AddWithValue("@param1", guildid);

                        long rows = (long)command.ExecuteScalar();

                        result = rows > 0;
                    }
                }
                catch (Exception e)
                {
                    message = string.Format("Error checking guild exists: {0}", e.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// Insert Guild
        /// </summary>
        public bool InsertGuild(GuildEntity entity, out string message)
        {
            bool rowinserted = false;
            message = string.Empty;
            string error = string.Empty;

            if (m_SqlConnection.State == ConnectionState.Open)
            {
                int rowseffected = 0;

                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(INSERT_GUILD, m_SqlConnection))
                    {
                        cmd.Parameters.Add("@param1", MySqlDbType.VarChar).Value = Guid.NewGuid().ToString();
                        cmd.Parameters.Add("@param2", MySqlDbType.UInt64).Value = entity.GuildId;
                        cmd.Parameters.Add("@param3", MySqlDbType.DateTime).Value = DateTime.Now;
                        cmd.Parameters.Add("@param4", MySqlDbType.VarChar).Value = entity.Prefix;
                        cmd.Parameters.Add("@param5", MySqlDbType.UInt64).Value = entity.TCLeaderboardId;
                        cmd.Parameters.Add("@param6", MySqlDbType.UInt64).Value = entity.LeaderboardLatestMessageId;
                        cmd.Parameters.Add("@param7", MySqlDbType.VarChar).Value = entity.LeaderboardTitle.Length > 50 ?
                                                                                   entity.LeaderboardTitle.Substring(0, 50) : entity.LeaderboardTitle;

                        cmd.Parameters.Add("@param8", MySqlDbType.DateTime).Value = DateTime.Now;

                        cmd.CommandType = CommandType.Text;
                        rowseffected = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                }

                rowinserted = rowseffected == 1;

                message = rowinserted ? string.Empty : "Guild Entity was not inserted.";

                if (!string.IsNullOrWhiteSpace(error))
                {
                    message += string.Format(" Error inserting Guild Entity Error: {0}", error);
                }
            }
            else
            {
                message = "Not Connected to SQL.";
            }


            return rowinserted;
        }

        /// <summary>
        /// Update Guild
        /// </summary>
        public bool UpdateGuild(GuildEntity entity, out string message)
        {
            bool rowupdated = false;
            message = string.Empty;
            string error = string.Empty;
            if (m_SqlConnection.State == ConnectionState.Open)
            {
                int rowseffected = 0;

                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(UPDATE_GUILD, m_SqlConnection))
                    {
                        cmd.Parameters.Add("@param1", MySqlDbType.UInt64).Value = entity.GuildId;
                        cmd.Parameters.Add("@param2", MySqlDbType.VarChar).Value = entity.Prefix;
                        cmd.Parameters.Add("@param3", MySqlDbType.UInt64).Value = entity.TCLeaderboardId;
                        cmd.Parameters.Add("@param4", MySqlDbType.DateTime).Value = DateTime.Now;

                        cmd.CommandType = CommandType.Text;
                        rowseffected = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                }

                rowupdated = rowseffected == 1;

                message = rowupdated ? string.Empty : "Guild Row was not modified.";

                if (!string.IsNullOrWhiteSpace(error))
                {
                    message += string.Format(" Error Modifying Guild Row Error: {0}", error);
                }
            }
            else
            {
                message = "Not Connected to SQL.";
            }


            return rowupdated;
        }

        /// <summary>
        /// Leaderboard Message Id for Guild Entity
        /// </summary>
        public bool UpdateLeaderboardMessageGuild(ulong guildid, ulong messageid, out string message)
        {
            bool rowupdated = false;
            message = string.Empty;
            string error = string.Empty;
            if (m_SqlConnection.State == ConnectionState.Open)
            {
                int rowseffected = 0;

                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(UPDATE_LEADERBOARDMESSAGE_GUILD, m_SqlConnection))
                    {
                        cmd.Parameters.Add("@param1", MySqlDbType.UInt64).Value = guildid;
                        cmd.Parameters.Add("@param2", MySqlDbType.UInt64).Value = messageid;
                        cmd.Parameters.Add("@param3", MySqlDbType.DateTime).Value = DateTime.Now;

                        cmd.CommandType = CommandType.Text;
                        rowseffected = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                }

                rowupdated = rowseffected == 1;

                message = rowupdated ? string.Empty : "Guild Row was not modified.";

                if (!string.IsNullOrWhiteSpace(error))
                {
                    message += string.Format(" Error Modifying Guild Row Error: {0}", error);
                }
            }
            else
            {
                message = "Not Connected to SQL.";
            }


            return rowupdated;
        }


        /// <summary>
        /// Leaderboard Message Id for Guild Entity
        /// </summary>
        public bool UpdateLeaderboardTitleGuild(ulong guildid, string leaderboardtitle, out string message)
        {
            bool rowupdated = false;
            message = string.Empty;
            string error = string.Empty;
            if (m_SqlConnection.State == ConnectionState.Open)
            {
                int rowseffected = 0;

                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(UPDATE_LEADERBOARDTITLE_GUILD, m_SqlConnection))
                    {
                        cmd.Parameters.Add("@param1", MySqlDbType.UInt64).Value = guildid;
                        cmd.Parameters.Add("@param2", MySqlDbType.VarChar).Value = leaderboardtitle;
                        cmd.Parameters.Add("@param3", MySqlDbType.DateTime).Value = DateTime.Now;

                        cmd.CommandType = CommandType.Text;
                        rowseffected = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                }

                rowupdated = rowseffected == 1;

                message = rowupdated ? string.Empty : "Guild Row was not modified.";

                if (!string.IsNullOrWhiteSpace(error))
                {
                    message += string.Format(" Error Modifying Guild Row Error: {0}", error);
                }
            }
            else
            {
                message = "Not Connected to SQL.";
            }


            return rowupdated;
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

        private GuildEntity GetEntityFromReader(MySqlDataReader reader)
        {
            GuildEntity entity = new GuildEntity();

            int i = 0;

            entity.Id = reader.GetString(i++);
            entity.GuildId = reader.GetUInt64(i++);
            entity.DateInserted = reader.GetDateTime(i++);
            entity.Prefix = reader.GetChar(i++);
            entity.TCLeaderboardId = reader.GetUInt64(i++);
            entity.LeaderboardLatestMessageId = reader.GetUInt64(i++);
            entity.LeaderboardTitle = reader.GetString(i++);
            entity.LastModified = reader.GetDateTime(i++);

            return entity;
        }
    }
}
