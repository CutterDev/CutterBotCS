using CutterDB.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace CutterDB.Tables
{
    /// <summary>
    /// Participant SQL Table
    /// </summary>
    public class ParticipantTable : ITable<ParticipantEntity>
    {
        MySqlConnection m_SqlConnection { get; set; }

        #region Query Strings


        const string INSERT_MATCHPARTICIPANTS = "INSERT INTO matchparticipants{0} " +
                                                "VALUES (@param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9, @param10," +
                                                        "@param11, @param12, @param13, @param14, @param15, @param16, @param17, @param18, @param19, @param20," +
                                                        "@param21, @param22, @param23, @param24, @param25, @param26, @param27, @param28, @param29, @param30," +
                                                        "@param31, @param32, @param33, @param34, @param35, @param36, @param37, @param38, @param39, @param40," +
                                                        "@param41, @param42, @param43, @param44, @param45, @param46, @param47, @param48, @param49, @param50," +
                                                        "@param51, @param52, @param53, @param54, @param55, @param56, @param57, @param58, @param59, @param60," +
                                                        "@param61, @param62, @param63, @param64, @param65, @param66, @param67, @param68, @param69, @param70," +
                                                        "@param71, @param72, @param73, @param74, @param75, @param76, @param77, @param78, @param79, @param80," +
                                                        "@param81, @param82, @param83, @param84, @param85, @param86, @param87, @param88, @param89, @param90," +
                                                        "@param91, @param92, @param93, @param94, @param95, @param96, @param97, @param98, @param99, @param100," +
                                                        "@param101, @param102, @param103)";

        const string SELECT_PARTICIPANT = "SELECT * FROM matchparticipants{0} WHERE matchid = @param1 AND participantid = @param2";

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
                message = string.Format("Error Connecting to Participant Table SQL: {0}", e.Message);
            }
        }

        /// <summary>
        /// Insert Participant Entity
        /// </summary>
        public bool InsertEntity(ParticipantEntity entity, string region, out string message)
        {
            bool rowinserted = false;
            message = string.Empty;
            string error = string.Empty;
            if (m_SqlConnection.State == ConnectionState.Open)
            {
                int rowseffected = 0;

                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(string.Format(INSERT_MATCHPARTICIPANTS, region), m_SqlConnection))
                    {
                        cmd.Parameters.Add("@param1", MySqlDbType.String).Value = entity.MatchId;
                        cmd.Parameters.Add("@param2", MySqlDbType.Int32).Value = entity.ParticipantId;
                        cmd.Parameters.Add("@param3", MySqlDbType.Int32).Value = entity.BaronKills;
                        cmd.Parameters.Add("@param4", MySqlDbType.Int32).Value = entity.BountyLevel;
                        cmd.Parameters.Add("@param5", MySqlDbType.Int32).Value = entity.ChampExperience;
                        cmd.Parameters.Add("@param6", MySqlDbType.Int32).Value = entity.ChampionIdEnum;
                        cmd.Parameters.Add("@param7", MySqlDbType.String).Value = entity.ChampionName;
                        cmd.Parameters.Add("@param8", MySqlDbType.Int32).Value = entity.ChampionTransform;
                        cmd.Parameters.Add("@param9", MySqlDbType.Int32).Value = entity.ChampLevel;
                        cmd.Parameters.Add("@param10", MySqlDbType.Int32).Value = entity.ConsumablesPurchased;
                        cmd.Parameters.Add("@param11", MySqlDbType.Int32).Value = entity.DamageDealtToBuildings;
                        cmd.Parameters.Add("@param12", MySqlDbType.Int32).Value = entity.DamageDealtToObjectives;
                        cmd.Parameters.Add("@param13", MySqlDbType.Int32).Value = entity.DamageDealtToTurrets;
                        cmd.Parameters.Add("@param14", MySqlDbType.Int32).Value = entity.DamageSelfMitigated;
                        cmd.Parameters.Add("@param15", MySqlDbType.Int32).Value = entity.Deaths;
                        cmd.Parameters.Add("@param16", MySqlDbType.Int32).Value = entity.DetectorWardsPlaced;
                        cmd.Parameters.Add("@param17", MySqlDbType.Int32).Value = entity.DoubleKills;
                        cmd.Parameters.Add("@param18", MySqlDbType.Int32).Value = entity.DragonKills;
                        cmd.Parameters.Add("@param19", MySqlDbType.Bit).Value = entity.FirstBloodAssist;
                        cmd.Parameters.Add("@param20", MySqlDbType.Bit).Value = entity.FirstBloodKill;
                        cmd.Parameters.Add("@param21", MySqlDbType.Bit).Value = entity.FirstTowerAssist;
                        cmd.Parameters.Add("@param22", MySqlDbType.Bit).Value = entity.FirstTowerKill;
                        cmd.Parameters.Add("@param23", MySqlDbType.Bit).Value = entity.GameEndedInEarlySurrender;
                        cmd.Parameters.Add("@param24", MySqlDbType.Bit).Value = entity.GameEndedInSurrender;
                        cmd.Parameters.Add("@param25", MySqlDbType.Int32).Value = entity.GoldEarned;
                        cmd.Parameters.Add("@param26", MySqlDbType.Int32).Value = entity.GoldSpent;
                        cmd.Parameters.Add("@param27", MySqlDbType.String).Value = entity.IndividualPosition;
                        cmd.Parameters.Add("@param28", MySqlDbType.Int32).Value = entity.InhibitorKills;
                        cmd.Parameters.Add("@param29", MySqlDbType.Int32).Value = entity.InhibitorsLost;
                        cmd.Parameters.Add("@param30", MySqlDbType.Int32).Value = entity.InhibitorTakedowns;
                        cmd.Parameters.Add("@param31", MySqlDbType.Int32).Value = entity.Item0;
                        cmd.Parameters.Add("@param32", MySqlDbType.Int32).Value = entity.Item1;
                        cmd.Parameters.Add("@param33", MySqlDbType.Int32).Value = entity.Item2;
                        cmd.Parameters.Add("@param34", MySqlDbType.Int32).Value = entity.Item3;
                        cmd.Parameters.Add("@param35", MySqlDbType.Int32).Value = entity.Item4;
                        cmd.Parameters.Add("@param36", MySqlDbType.Int32).Value = entity.Item5;
                        cmd.Parameters.Add("@param37", MySqlDbType.Int32).Value = entity.Item6;
                        cmd.Parameters.Add("@param38", MySqlDbType.Int32).Value = entity.ItemsPurchased;
                        cmd.Parameters.Add("@param39", MySqlDbType.Int32).Value = entity.KillingSprees;
                        cmd.Parameters.Add("@param40", MySqlDbType.Int32).Value = entity.Kills;
                        cmd.Parameters.Add("@param41", MySqlDbType.String).Value = entity.Lane;
                        cmd.Parameters.Add("@param42", MySqlDbType.Int32).Value = entity.LargestCriticalStrike;
                        cmd.Parameters.Add("@param43", MySqlDbType.Int32).Value = entity.LargestKillingSpree;
                        cmd.Parameters.Add("@param44", MySqlDbType.Int32).Value = entity.LargestMultikill;
                        cmd.Parameters.Add("@param45", MySqlDbType.Int32).Value = entity.LongestTimeSpentLiving;
                        cmd.Parameters.Add("@param46", MySqlDbType.Int32).Value = entity.MagicDamageDealt;
                        cmd.Parameters.Add("@param47", MySqlDbType.Int32).Value = entity.MagicDamageDealtToChampions;
                        cmd.Parameters.Add("@param48", MySqlDbType.Int32).Value = entity.MagicDamageTaken;
                        cmd.Parameters.Add("@param49", MySqlDbType.Int32).Value = entity.NeutralMinionsKilled;
                        cmd.Parameters.Add("@param50", MySqlDbType.Int32).Value = entity.NexusKills;
                        cmd.Parameters.Add("@param51", MySqlDbType.Int32).Value = entity.NexusLost;
                        cmd.Parameters.Add("@param52", MySqlDbType.Int32).Value = entity.NexusTakedowns;
                        cmd.Parameters.Add("@param53", MySqlDbType.Int32).Value = entity.ObjectivesStolenAssists;
                        cmd.Parameters.Add("@param54", MySqlDbType.Int32).Value = entity.ObjectivesStolenAssists;
                        cmd.Parameters.Add("@param55", MySqlDbType.Int32).Value = entity.PentaKills;
                        cmd.Parameters.Add("@param56", MySqlDbType.Int32).Value = entity.PhysicalDamageDealtToChampions;
                        cmd.Parameters.Add("@param57", MySqlDbType.Int32).Value = entity.PhysicalDamageDealtToChampions;
                        cmd.Parameters.Add("@param58", MySqlDbType.Int32).Value = entity.PhysicalDamageTaken;
                        cmd.Parameters.Add("@param59", MySqlDbType.Int32).Value = entity.ProfileIcon;
                        cmd.Parameters.Add("@param60", MySqlDbType.String).Value = entity.Puuid;
                        cmd.Parameters.Add("@param61", MySqlDbType.Int32).Value = entity.QuadraKills;
                        cmd.Parameters.Add("@param62", MySqlDbType.String).Value = entity.RiotIdTagline;
                        cmd.Parameters.Add("@param63", MySqlDbType.String).Value = entity.Role;
                        cmd.Parameters.Add("@param64", MySqlDbType.Int32).Value = entity.SightWardsBoughtInGame;
                        cmd.Parameters.Add("@param65", MySqlDbType.Int32).Value = entity.Spell1Casts;
                        cmd.Parameters.Add("@param66", MySqlDbType.Int32).Value = entity.Spell2Casts;
                        cmd.Parameters.Add("@param67", MySqlDbType.Int32).Value = entity.Spell3Casts;
                        cmd.Parameters.Add("@param68", MySqlDbType.Int32).Value = entity.Spell4Casts;
                        cmd.Parameters.Add("@param69", MySqlDbType.Int32).Value = entity.Summoner1Casts;
                        cmd.Parameters.Add("@param70", MySqlDbType.Int32).Value = entity.Summoner1Id;
                        cmd.Parameters.Add("@param71", MySqlDbType.Int32).Value = entity.Summoner2Casts;
                        cmd.Parameters.Add("@param72", MySqlDbType.Int32).Value = entity.Summoner2Id;
                        cmd.Parameters.Add("@param73", MySqlDbType.String).Value = entity.SummonerId;
                        cmd.Parameters.Add("@param74", MySqlDbType.Int32).Value = entity.SummonerLevel;
                        cmd.Parameters.Add("@param75", MySqlDbType.String).Value = entity.SummonerName;
                        cmd.Parameters.Add("@param76", MySqlDbType.Bit).Value = entity.TeamEarlySurrendered;
                        cmd.Parameters.Add("@param77", MySqlDbType.String).Value = entity.TeamIdEnum;
                        cmd.Parameters.Add("@param78", MySqlDbType.String).Value = entity.TeamPosition;
                        cmd.Parameters.Add("@param79", MySqlDbType.Int32).Value = entity.TimeCcingOthers;
                        cmd.Parameters.Add("@param80", MySqlDbType.Int32).Value = entity.TimePlayed;
                        cmd.Parameters.Add("@param81", MySqlDbType.Int32).Value = entity.TotalDamageDealtToChampions;
                        cmd.Parameters.Add("@param82", MySqlDbType.Int32).Value = entity.TotalDamageDealtToChampions;
                        cmd.Parameters.Add("@param83", MySqlDbType.Int32).Value = entity.TotalDamageShieldedOnTeammates;
                        cmd.Parameters.Add("@param84", MySqlDbType.Int32).Value = entity.TotalDamageTaken;
                        cmd.Parameters.Add("@param85", MySqlDbType.Int32).Value = entity.TotalHealsOnTeammates;
                        cmd.Parameters.Add("@param86", MySqlDbType.Int32).Value = entity.TotalHealsOnTeammates;
                        cmd.Parameters.Add("@param87", MySqlDbType.Int32).Value = entity.TotalMinionsKilled;
                        cmd.Parameters.Add("@param88", MySqlDbType.Int32).Value = entity.TotalTimeCcDealt;
                        cmd.Parameters.Add("@param89", MySqlDbType.Int32).Value = entity.TotalTimeSpentDead;
                        cmd.Parameters.Add("@param90", MySqlDbType.Int32).Value = entity.TotalUnitsHealed;
                        cmd.Parameters.Add("@param91", MySqlDbType.Int32).Value = entity.TripleKills;
                        cmd.Parameters.Add("@param92", MySqlDbType.Int32).Value = entity.TrueDamageDealtToChampions;
                        cmd.Parameters.Add("@param93", MySqlDbType.Int32).Value = entity.TrueDamageDealtToChampions;
                        cmd.Parameters.Add("@param94", MySqlDbType.Int32).Value = entity.TrueDamageTaken;
                        cmd.Parameters.Add("@param95", MySqlDbType.Int32).Value = entity.TurretKills;
                        cmd.Parameters.Add("@param96", MySqlDbType.Int32).Value = entity.TurretsLost;
                        cmd.Parameters.Add("@param97", MySqlDbType.Int32).Value = entity.TurretTakedowns;
                        cmd.Parameters.Add("@param98", MySqlDbType.Int32).Value = entity.UnrealKills;
                        cmd.Parameters.Add("@param99", MySqlDbType.Int32).Value = entity.VisionScore;
                        cmd.Parameters.Add("@param100", MySqlDbType.Int32).Value = entity.VisionWardsBoughtInGame;
                        cmd.Parameters.Add("@param101", MySqlDbType.Int32).Value = entity.WardsKilled;
                        cmd.Parameters.Add("@param102", MySqlDbType.Int32).Value = entity.DetectorWardsPlaced;
                        cmd.Parameters.Add("@param103", MySqlDbType.Bit).Value = entity.Win;

                        cmd.CommandType = CommandType.Text;
                        rowseffected = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                }

                rowinserted = rowseffected == 1;

                message = rowinserted ? string.Empty : "Participant was not inserted.";

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
        /// Get Participant Entity, key is {matchid-participantid}
        /// </summary>
        public bool TryGetEntity(string matchid, int participantid, string region, out ParticipantEntity entity, out string message)
        {
            bool result = false;
            message = string.Empty;
            entity = null;

            try
            {
                using (MySqlCommand command = new MySqlCommand(string.Format(SELECT_PARTICIPANT, region), m_SqlConnection))
                {
                    command.Parameters.AddWithValue("@param1", matchid);
                    command.Parameters.AddWithValue("@param2", participantid);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity = new ParticipantEntity();
                            entity.MatchId = reader.GetString("matchid");
                            entity.ParticipantId = reader.GetInt32("participantid");
                            entity.BaronKills = reader.GetInt32("baronkills");
                            entity.BountyLevel = reader.GetInt32("bountylevel");
                            entity.ChampExperience = reader.GetInt32("champexperience");
                            entity.ChampionIdEnum = reader.GetInt32("championidenum");
                            entity.ChampionName = reader.GetString("championname");
                            entity.ChampionTransform = reader.GetInt32("championtransform");
                            entity.ChampLevel = reader.GetInt32("champlevel");
                            entity.ConsumablesPurchased = reader.GetInt32("consumablespurchased");
                            entity.DamageDealtToBuildings = reader.GetInt32("damagedealttobuildings");
                            entity.DamageDealtToObjectives = reader.GetInt32("damagedealttoobjectives");
                            entity.DamageDealtToTurrets = reader.GetInt32("damagedealttoturrets");
                            entity.DamageSelfMitigated = reader.GetInt32("damageselfmitigated");
                            entity.Deaths = reader.GetInt32("deaths");
                            entity.DetectorWardsPlaced = reader.GetInt32("detectorwardsplaced");
                            entity.DoubleKills = reader.GetInt32("doublekills");
                            entity.DragonKills = reader.GetInt32("dragonkills");
                            entity.FirstBloodAssist = reader.GetBoolean("firstbloodassist");
                            entity.FirstBloodKill = reader.GetBoolean("firstbloodkill");
                            entity.FirstTowerAssist = reader.GetBoolean("firsttowerassist");
                            entity.FirstTowerKill = reader.GetBoolean("firsttowerkill");
                            entity.GameEndedInEarlySurrender = reader.GetBoolean("gameendedinearlysurrender");
                            entity.GameEndedInSurrender = reader.GetBoolean("gameendedinsurrender");
                            entity.GoldEarned = reader.GetInt32("goldearned");
                            entity.GoldSpent = reader.GetInt32("goldspent");
                            entity.IndividualPosition = reader.GetString("individualposition");
                            entity.InhibitorKills = reader.GetInt32("inhibitorkills");
                            entity.InhibitorsLost = reader.GetInt32("inhibitorslost");
                            entity.InhibitorTakedowns = reader["inhibitortakedowns"] == DBNull.Value ? null : reader.GetInt32("inhibitortakedowns");
                            entity.Item0 = reader.GetInt32("item0");
                            entity.Item1 = reader.GetInt32("item1");
                            entity.Item2 = reader.GetInt32("item2");
                            entity.Item3 = reader.GetInt32("item3");
                            entity.Item4 = reader.GetInt32("item4");
                            entity.Item5 = reader.GetInt32("item5");
                            entity.Item6 = reader.GetInt32("item6");
                            entity.ItemsPurchased = reader.GetInt32("itemspurchased");
                            entity.KillingSprees = reader.GetInt32("killingsprees");
                            entity.Kills = reader.GetInt32("kills");
                            entity.Lane = reader.GetString("lane");
                            entity.LargestCriticalStrike = reader.GetInt32("largestcriticalstrike");
                            entity.LargestKillingSpree = reader.GetInt32("largestkillingspree");
                            entity.LargestMultikill = reader.GetInt32("largestmultikill");
                            entity.LongestTimeSpentLiving = reader.GetInt32("longesttimespentliving");
                            entity.MagicDamageDealt = reader.GetInt32("magicdamagedealt");
                            entity.MagicDamageDealtToChampions = reader.GetInt32("magicdamagedealttochampions");
                            entity.MagicDamageTaken = reader.GetInt32("magicdamagetaken");
                            entity.NeutralMinionsKilled = reader.GetInt32("neutralminionskilled");
                            entity.NexusKills = reader.GetInt32("nexuskills");
                            entity.NexusLost = reader.GetInt32("nexuslost");
                            entity.NexusTakedowns = reader["nexustakedowns"] == DBNull.Value ? null : reader.GetInt32("nexustakedowns");
                            entity.ObjectivesStolen = reader.GetInt32("objectivesstolen");
                            entity.ObjectivesStolenAssists = reader.GetInt32("objectivesstolenassists");
                            entity.PentaKills = reader.GetInt32("pentakills");
                            entity.PhysicalDamageDealt = reader.GetInt32("physicaldamagedealt");
                            entity.PhysicalDamageDealtToChampions = reader.GetInt32("physicaldamagedealttochampions");
                            entity.PhysicalDamageTaken = reader.GetInt32("physicaldamagetaken");
                            entity.ProfileIcon = reader.GetInt32("profileicon");
                            entity.Puuid = reader.GetString("puuid");
                            entity.QuadraKills = reader.GetInt32("quadrakills");
                            entity.RiotIdTagline = reader.GetString("riotidtagline");
                            entity.Role = reader.GetString("role");
                            entity.SightWardsBoughtInGame = reader.GetInt32("sightwardsboughtingame");
                            entity.Spell1Casts = reader.GetInt32("spell1casts");
                            entity.Spell2Casts = reader.GetInt32("spell2casts");
                            entity.Spell3Casts = reader.GetInt32("spell3casts");
                            entity.Spell4Casts = reader.GetInt32("spell4casts");
                            entity.Summoner1Casts = reader.GetInt32("summoner1casts");
                            entity.Summoner1Id = reader.GetInt32("summoner1id");
                            entity.Summoner2Casts = reader.GetInt32("summoner2casts");
                            entity.Summoner2Id = reader.GetInt32("summoner2id");
                            entity.SummonerId = reader.GetString("summonerid");
                            entity.SummonerLevel = reader.GetInt32("summonerlevel");
                            entity.SummonerName = reader.GetString("summonername");
                            entity.TeamEarlySurrendered = reader.GetBoolean("teamearlysurrendered");
                            entity.TeamIdEnum = reader.GetString("teamidenum");
                            entity.TeamPosition = reader.GetString("teamposition");
                            entity.TimeCcingOthers = reader.GetInt32("timeccingothers");
                            entity.TimePlayed = reader.GetInt32("timeplayed");
                            entity.TotalDamageDealt = reader.GetInt32("totaldamagedealt");
                            entity.TotalDamageDealtToChampions = reader.GetInt32("totaldamagedealttochampions");
                            entity.TotalDamageShieldedOnTeammates = reader.GetInt32("totaldamageshieldedonteammates");
                            entity.TotalDamageTaken = reader.GetInt32("totaldamagetaken");
                            entity.TotalHeal = reader.GetInt32("totalheal");
                            entity.TotalHealsOnTeammates = reader.GetInt32("totalhealsonteammates");
                            entity.TotalMinionsKilled = reader.GetInt32("totalminionskilled");
                            entity.TotalTimeCcDealt = reader.GetInt32("totaltimeccdealt");
                            entity.TotalTimeSpentDead = reader.GetInt32("totaltimespentdead");
                            entity.TotalUnitsHealed = reader.GetInt32("totalunitshealed");
                            entity.TripleKills = reader.GetInt32("triplekills");
                            entity.TrueDamageDealt = reader.GetInt32("truedamagedealt");
                            entity.TrueDamageDealtToChampions = reader.GetInt32("truedamagedealttochampions");
                            entity.TrueDamageTaken = reader.GetInt32("truedamagetaken");
                            entity.TurretKills = reader.GetInt32("turretkills");
                            entity.TurretsLost = reader.GetInt32("turretslost");
                            entity.TurretTakedowns = reader["turrettakedowns"] == DBNull.Value ? null : reader.GetInt32("turrettakedowns");
                            entity.UnrealKills = reader.GetInt32("unrealkills");
                            entity.VisionScore = reader.GetInt32("visionscore");
                            entity.VisionWardsBoughtInGame = reader.GetInt32("visionwardsboughtingame");
                            entity.WardsKilled = reader.GetInt32("wardskilled");
                            entity.WardsPlaced = reader.GetInt32("wardsplaced");
                            entity.Win = reader.GetBoolean("win");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                message = string.Format("Error Getting Match Participant in SQL: {0}", e.Message);
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
