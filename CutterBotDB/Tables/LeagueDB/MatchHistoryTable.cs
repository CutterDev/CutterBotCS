using CutterDB.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace CutterDB.Tables
{
    /// <summary>
    /// Match History Table
    /// </summary>
    public class MatchHistoryTable : ITable<MatchEntity>
    {
        MySqlConnection m_SqlConnection { get; set; }

        #region Query Strings
        const string EXISTS_MATCHHISTORY = "SELECT COUNT(*) FROM matchhistory{0} WHERE (matchid = @param1)";

        const string INSERT_MATCHHISTORY = "INSERT INTO matchhistory{0} " +
                                   "VALUES (@param0, @param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9, @param10," +
                                          "@param11, @param12, @param13, @param14, @param15, @param16, @param17, @param18, @param19, @param20," +
                                          "@param21, @param22, @param23, @param24, @param25)";

        const string SELECT_MATCHHISTORY = "SELECT * FROM matchhistory{0} WHERE matchid = @param1";
        #endregion

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
        /// Entity Exists
        /// </summary>
        public bool EntityExists(string matchid, string region, out string message)
        {
            int rowsfound = 0;
            message = string.Empty;
            string error = string.Empty;
            string commandtext = string.Empty;

            if (m_SqlConnection.State == ConnectionState.Open)
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(string.Format(EXISTS_MATCHHISTORY, region), m_SqlConnection))
                    {
                        cmd.Parameters.Add("@param0", MySqlDbType.String).Value = matchid;

                        commandtext = cmd.CommandText;
                        cmd.CommandType = CommandType.Text;
                        rowsfound = (int)cmd.ExecuteScalar();
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    message += string.Format("Command{0}\r\n Error: {1}", commandtext, error);
                }
            }
            else
            {
                message = "Not Connected to SQL.";
            }

            return rowsfound > 0;
        }

        /// <summary>
        /// Insert Entity to Table
        /// </summary>
        public bool InsertEntity(MatchEntity entity, string region, out string message)
        {
            bool rowinserted = false;
            message = string.Empty;
            string error = string.Empty;
            string commandtext = string.Empty;
            if (m_SqlConnection.State == ConnectionState.Open)
            {
                int rowseffected = 0;

                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(string.Format(INSERT_MATCHHISTORY, region), m_SqlConnection))
                    {
                        cmd.Parameters.Add("@param0", MySqlDbType.String).Value = entity.MatchId;
                        cmd.Parameters.Add("@param1", MySqlDbType.Int64).Value = entity.GameCreation;
                        cmd.Parameters.Add("@param2", MySqlDbType.Int64).Value = entity.GameDuration;
                        cmd.Parameters.Add("@param3", MySqlDbType.Int64).Value = entity.GameEndTimestamp;
                        cmd.Parameters.Add("@param4", MySqlDbType.Int64).Value = entity.GameId;
                        cmd.Parameters.Add("@param5", MySqlDbType.String).Value = entity.GameModeEnum;
                        cmd.Parameters.Add("@param6", MySqlDbType.String).Value = entity.GameName;
                        cmd.Parameters.Add("@param7", MySqlDbType.Int64).Value = entity.GameStartTimestamp;
                        cmd.Parameters.Add("@param8", MySqlDbType.String).Value = entity.GameTypeEnum;
                        cmd.Parameters.Add("@param9", MySqlDbType.String).Value = entity.GameVersion;
                        cmd.Parameters.Add("@param10", MySqlDbType.String).Value = entity.MapIdEnum;
                        cmd.Parameters.Add("@param11", MySqlDbType.String).Value = entity.PlatformId;
                        cmd.Parameters.Add("@param12", MySqlDbType.String).Value = entity.QueueIdEnum;
                        cmd.Parameters.Add("@param13", MySqlDbType.Int32).Value = entity.Participant1Id;
                        cmd.Parameters.Add("@param14", MySqlDbType.Int32).Value = entity.Participant2Id;
                        cmd.Parameters.Add("@param15", MySqlDbType.Int32).Value = entity.Participant3Id;
                        cmd.Parameters.Add("@param16", MySqlDbType.Int32).Value = entity.Participant4Id;
                        cmd.Parameters.Add("@param17", MySqlDbType.Int32).Value = entity.Participant5Id;
                        cmd.Parameters.Add("@param18", MySqlDbType.Int32).Value = entity.Participant6Id;
                        cmd.Parameters.Add("@param19", MySqlDbType.Int32).Value = entity.Participant7Id;
                        cmd.Parameters.Add("@param20", MySqlDbType.Int32).Value = entity.Participant8Id;
                        cmd.Parameters.Add("@param21", MySqlDbType.Int32).Value = entity.Participant9Id;
                        cmd.Parameters.Add("@param22", MySqlDbType.Int32).Value = entity.Participant10Id;
                        cmd.Parameters.Add("@param23", MySqlDbType.String).Value = entity.TeamJson1;
                        cmd.Parameters.Add("@param24", MySqlDbType.String).Value = entity.TeamJson2;
                        cmd.Parameters.Add("@param25", MySqlDbType.String).Value = entity.DataVersion;
                        commandtext = cmd.CommandText;
                        cmd.CommandType = CommandType.Text;
                        rowseffected = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                }

                rowinserted = rowseffected == 1;

                message = rowinserted ? string.Empty : "MatchHistoryEntity was not inserted.\n\r";

                if (!string.IsNullOrWhiteSpace(error))
                {
                    message += string.Format("Command{0}\r\n Error: {1}", commandtext, error);
                }
            }
            else
            {
                message = "Not Connected to SQL.";
            }


            return rowinserted;
        }

        /// <summary>
        /// Get Entity from Table
        /// </summary>
        public bool TryGetEntity(string key, string region, out MatchEntity entity, out string message)
        {
            bool result = false;
            message = string.Empty;
            entity = null;

            try
            {
                int[] participantids = new int[10];
                using (MySqlCommand command = new MySqlCommand(string.Format(SELECT_MATCHHISTORY, region), m_SqlConnection))
                {
                    command.Parameters.AddWithValue("@param1", key);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity = new MatchEntity();
                            result = true;
                            entity.MatchId = reader.GetString("matchid");
                            entity.DataVersion = reader.GetString("dataversion");
                            entity.GameCreation = reader.GetInt64("gamecreation");
                            entity.GameDuration = reader.GetInt64("gameduration");
                            entity.GameEndTimestamp = reader.GetInt64("gameendtimestamp");
                            entity.GameId = reader.GetInt64("gameid");
                            entity.GameModeEnum = reader.GetString("gamemodeenum");
                            entity.GameName = reader.GetString("gamename");
                            entity.GameStartTimestamp = reader.GetInt64("gamestarttimestamp");
                            entity.GameTypeEnum = reader.GetString("gametypeenum");
                            entity.GameVersion = reader.GetString("gameversion");
                            entity.MapIdEnum = reader.GetString("mapidenum");
                            entity.PlatformId = reader.GetString("platformid");
                            entity.QueueIdEnum = reader.GetString("queueidenum");
                            entity.Participant1Id = reader.GetInt32("participant1id");
                            entity.TeamJson1 = reader.GetString("teamjson1");
                            entity.TeamJson2 = reader.GetString("teamjson2");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                message = string.Format("Error Getting Match History in SQL: {0}", e.Message);
            }

            return result;
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
