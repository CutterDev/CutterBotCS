using CutterDB.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace CutterDB.Tables
{
    /// <summary>
    /// Participant Perk Stats Table
    /// </summary>
    public class ParticipantPerkStatsTable : ITable<ParticipantPerkStatsEntity>
    {
        MySqlConnection m_SqlConnection { get; set; }

        #region Query Strings


        const string INSERT_PARTICIPANTSTATS = "INSERT INTO participantstats{0} " +
                                               "VALUES (@param1, @param2, @param3, @param4, @param5, @param6, @param7)";

        const string SELECT_PARTICIPANTSTATS = "SELECT * FROM participantstats{0} WHERE matchid = @param1 AND participantid = @param2";

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
                message = string.Format("Error Connecting to ParticipantPerkStats Table SQL: {0}", e.Message);
            }
        }

        /// <summary>
        /// Insert ParticipantPerkStatsEntity
        /// </summary>
        public bool InsertEntity(ParticipantPerkStatsEntity entity, string region, out string message)
        {
            bool rowinserted = false;
            message = string.Empty;
            string error = string.Empty;

            if (m_SqlConnection.State == ConnectionState.Open)
            {
                int rowseffected = 0;

                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(string.Format(INSERT_PARTICIPANTSTATS, region), m_SqlConnection))
                    {
                        cmd.Parameters.Add("@param1", MySqlDbType.String).Value = entity.MatchId;
                        cmd.Parameters.Add("@param2", MySqlDbType.Int32).Value = entity.ParticipantId;
                        cmd.Parameters.Add("@param3", MySqlDbType.Int32).Value = entity.PerksStatDefence;
                        cmd.Parameters.Add("@param4", MySqlDbType.Int32).Value = entity.PerksStatFlex;
                        cmd.Parameters.Add("@param5", MySqlDbType.Int32).Value = entity.PerksStatOffence;
                        cmd.Parameters.Add("@param6", MySqlDbType.String).Value = entity.PerkStyle1Json;
                        cmd.Parameters.Add("@param7", MySqlDbType.String).Value = entity.PerkStyle2Json;

                        cmd.CommandType = CommandType.Text;
                        rowseffected = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                }

                rowinserted = rowseffected == 1;

                message = rowinserted ? string.Empty : "PerkStats was not inserted.";

                if (!string.IsNullOrWhiteSpace(error))
                {
                    message += string.Format(" Error: {0}", error);
                }
            }
            else
            {
                message = "Not Connected to SQL.";
            }


            return rowinserted;
        }

        /// <summary>
        /// Get ParcitipantPerkStatsEntity from Table
        /// </summary>
        public bool TryGetEntity(string matchid, int participantid, string region, out ParticipantPerkStatsEntity entity, out string message)
        {
            bool result = false;
            message = string.Empty;
            entity = null;

            try
            {
                using (MySqlCommand command = new MySqlCommand(string.Format(SELECT_PARTICIPANTSTATS, region), m_SqlConnection))
                {
                    command.Parameters.AddWithValue("@param1", matchid);
                    command.Parameters.AddWithValue("@param2", participantid);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity = new ParticipantPerkStatsEntity();
                            result = true;
                            entity.MatchId = reader.GetString("matchid");
                            entity.ParticipantId = reader.GetInt32("participantid");
                            entity.PerksStatDefence = reader.GetInt32("perksstat_defence");
                            entity.PerksStatFlex = reader.GetInt32("perksstat_flex");
                            entity.PerksStatOffence = reader.GetInt32("perksstat_offence");
                            entity.PerkStyle1Json = reader.GetString("perkstyle1_json");
                            entity.PerkStyle2Json = reader.GetString("perkstyle2_json");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                message = string.Format("Error Getting Match Participant Stats in SQL: {0}", e.Message);
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
