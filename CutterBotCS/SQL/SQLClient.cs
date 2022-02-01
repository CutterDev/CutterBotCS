using Camille.Enums;
using Camille.RiotGames.MatchV5;
using CutterBotCS.Helpers;
using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace CutterBotCS.SQL
{
    public class SQLClient
    {
        /// <summary>
        /// SQL Connection
        /// </summary>
        MySqlConnection m_Connection { get; set; }

        const string INSERT_MATCHHISTORY = "INSERT INTO MatchHistory{0} " +
                                           "VALUES (@param0, @param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9, @param10," +
                                                  "@param11, @param12, @param13, @param14, @param15, @param16, @param17, @param18, @param19, @param20," +
                                                  "@param21, @param22, @param23, @param24, @param25)";

        const string INSERT_MATCHPARTICIPANTS = "INSERT INTO MatchParticipants{0} " +
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

        const string INSERT_PARTICIPANTSTATS = "INSERT INTO ParticipantStats{0} " +
                                               "VALUES (@param1, @param2, @param3, @param4, @param5, @param6, @param7)";

        const string SELECT_MATCHHISTORY = "SELECT * FROM MatchHistory{0} WHERE matchid = @param1";

        const string SELECT_MATCHPARTICIPANT = "SELECT * FROM MatchParticipants{0} WHERE matchid = @param1 AND participantid = @param2";
        
        const string SELECT_PARTICIPANTSTATS = "SELECT * FROM ParticipantStats{0} WHERE matchid = @param1 AND participantid = @param2";

        public SQLClient()
        {
            try
            {
                string connectstring = "server=localhost;user=superuser;database=cutterlolgames;port=3306;password=CuntMuncher123";
                m_Connection = new MySqlConnection(connectstring);
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Insert To Match History
        /// </summary>
        public void InsertToMatchHistory(string region, string matchid, string dataversion, long gamecreation, long gameduration, long? gameendtimestamp, long gameid, string gamemodeenum, string gamename, long gamestarttimestamp,
                                        string gametypeenum, string gameversion, string mapidenum, string platformid, string queueidenum,
                                        int participant1id, int participant2id, int participant3id, int participant4id, int participant5id,
                                        int participant6id, int participant7id, int participant8id, int participant9id, int participant10id,
                                        string teamjson1, string teamjson2, out string message)
        {
            int rowseffected = 0;
            message = string.Empty;
            string error = string.Empty;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(string.Format(INSERT_MATCHHISTORY, region), m_Connection))
                {
                    cmd.Parameters.Add("@param0", MySqlDbType.String).Value = matchid;
                    cmd.Parameters.Add("@param1", MySqlDbType.Int64).Value = gamecreation;
                    cmd.Parameters.Add("@param2", MySqlDbType.Int64).Value = gameduration;
                    cmd.Parameters.Add("@param3", MySqlDbType.Int64).Value = gameendtimestamp;
                    cmd.Parameters.Add("@param4", MySqlDbType.Int64).Value = gameid;
                    cmd.Parameters.Add("@param5", MySqlDbType.String).Value = gamemodeenum;
                    cmd.Parameters.Add("@param6", MySqlDbType.String).Value = gamename;
                    cmd.Parameters.Add("@param7", MySqlDbType.Int64).Value = gamestarttimestamp;
                    cmd.Parameters.Add("@param8", MySqlDbType.String).Value = gametypeenum;
                    cmd.Parameters.Add("@param9", MySqlDbType.String).Value = gameversion;
                    cmd.Parameters.Add("@param10", MySqlDbType.String).Value = mapidenum;
                    cmd.Parameters.Add("@param11", MySqlDbType.String).Value = platformid;
                    cmd.Parameters.Add("@param12", MySqlDbType.String).Value = queueidenum;
                    cmd.Parameters.Add("@param13", MySqlDbType.Int32).Value = participant1id;
                    cmd.Parameters.Add("@param14", MySqlDbType.Int32).Value = participant2id;
                    cmd.Parameters.Add("@param15", MySqlDbType.Int32).Value = participant3id;
                    cmd.Parameters.Add("@param16", MySqlDbType.Int32).Value = participant4id;
                    cmd.Parameters.Add("@param17", MySqlDbType.Int32).Value = participant5id;
                    cmd.Parameters.Add("@param18", MySqlDbType.Int32).Value = participant6id;
                    cmd.Parameters.Add("@param19", MySqlDbType.Int32).Value = participant7id;
                    cmd.Parameters.Add("@param20", MySqlDbType.Int32).Value = participant8id;
                    cmd.Parameters.Add("@param21", MySqlDbType.Int32).Value = participant9id;
                    cmd.Parameters.Add("@param22", MySqlDbType.Int32).Value = participant10id;
                    cmd.Parameters.Add("@param23", MySqlDbType.String).Value = teamjson1;
                    cmd.Parameters.Add("@param24", MySqlDbType.String).Value = teamjson2;
                    cmd.Parameters.Add("@param25", MySqlDbType.String).Value = dataversion;

                    cmd.CommandType = CommandType.Text;
                    rowseffected = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            message = rowseffected == 1 ? string.Format("Match History Inserted: MatchId: {0}", gameid) : string.Format("No Match History Effected, Error: {0}", error);
        }

        /// <summary>
        /// Insert Match Participant
        /// </summary>
        public void InsertToMatchParticipants(string region, string matchid, int participantid, int baronkills, int bountylevel, int champexperience,
                                              int championidenum, string championname, int championtransform, int champlevel, int consumablespurchased,
                                              int damagedealttobuildings, int damagedealttoobjectives, int damagedealttoturrets, int damageselfmitigated, int deaths,
                                              int detectorwardsplaced, int doublekills, int dragonkills, bool firstbloodassist, bool firstbloodkill, bool firsttowerassist, 
                                              bool firsttowerkill, bool gameendedinearlysurrender, bool gameendedinsurrender, int goldearned, int goldspent,
                                              string individualposition, int inhibitorkills, int inhibitorslost, int? inhibitortakedowns,
                                              int item0, int item1, int item2, int item3, int item4, int item5, int item6,
                                              int itemspurchased, int killingsprees, int kills, string lane, int largestcriticalstrike, int largestkillingspree,
                                              int largestmultikill, int longesttimespentliving, int magicdamagedealt, int magicdamagedealttochampions,
                                              int magicdamagetaken, int neutralminionskilled, int nexuskills, int nexuslost, int? nexustakedowns,
                                              int objectivesstolen, int objectivesstolenassists, int pentakills, int physicaldamagedealt, int physicaldamagedealttochampions,
                                              int physicaldamagetaken, int profileicon, string puuid, int quadrakills, string riotidtagline, string role, int sightwardsboughtingame,
                                              int spell1casts, int spell2casts, int spell3casts, int spell4casts,
                                              int summoner1casts, int summoner1id, int summoner2casts, int summoner2id, string summonerid,
                                              int summonerlevel, string summonername, bool teamearlysurrendered, string teamidenum, string teamposition,
                                              int timeccingothers, int timeplayed, int totaldamagedealt, int totaldamagedealttochampions, int totaldamageshieldedonteammates,
                                              int totaldamagetaken, int totalheal, int totalhealsonteammates, int totalminionskilled, int totaltimeccdealt,
                                              int totaltimespentdead, int totalunitshealed, int triplekills, int truedamagedealt, int truedamagedealttochampions,
                                              int truedamagetaken, int turretkills, int turretslost, int? turrettakedowns, int unrealkills,
                                              int visionscore, int visionwardsboughtingame, int wardskilled, int wardsplaced, bool win, out string message)
                                              
        {
            int rowseffected = 0;
            message = string.Empty;
            string error = string.Empty;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(string.Format(INSERT_MATCHPARTICIPANTS, region), m_Connection))
                {
                    cmd.Parameters.Add("@param1", MySqlDbType.String).Value = matchid;
                    cmd.Parameters.Add("@param2", MySqlDbType.Int32).Value = participantid;
                    cmd.Parameters.Add("@param3", MySqlDbType.Int32).Value = baronkills;
                    cmd.Parameters.Add("@param4", MySqlDbType.Int32).Value = bountylevel;
                    cmd.Parameters.Add("@param5", MySqlDbType.Int32).Value = champexperience;
                    cmd.Parameters.Add("@param6", MySqlDbType.Int32).Value = championidenum;
                    cmd.Parameters.Add("@param7", MySqlDbType.String).Value = championname;
                    cmd.Parameters.Add("@param8", MySqlDbType.Int32).Value = championtransform;
                    cmd.Parameters.Add("@param9", MySqlDbType.Int32).Value = champlevel;
                    cmd.Parameters.Add("@param10", MySqlDbType.Int32).Value = consumablespurchased;
                    cmd.Parameters.Add("@param11", MySqlDbType.Int32).Value = damagedealttobuildings;
                    cmd.Parameters.Add("@param12", MySqlDbType.Int32).Value = damagedealttoobjectives;
                    cmd.Parameters.Add("@param13", MySqlDbType.Int32).Value = damagedealttoturrets;
                    cmd.Parameters.Add("@param14", MySqlDbType.Int32).Value = damageselfmitigated;
                    cmd.Parameters.Add("@param15", MySqlDbType.Int32).Value = deaths;
                    cmd.Parameters.Add("@param16", MySqlDbType.Int32).Value = detectorwardsplaced;
                    cmd.Parameters.Add("@param17", MySqlDbType.Int32).Value = doublekills;
                    cmd.Parameters.Add("@param18", MySqlDbType.Int32).Value = dragonkills;
                    cmd.Parameters.Add("@param19", MySqlDbType.Bit).Value = firstbloodassist;
                    cmd.Parameters.Add("@param20", MySqlDbType.Bit).Value = firstbloodkill;
                    cmd.Parameters.Add("@param21", MySqlDbType.Bit).Value = firsttowerassist;
                    cmd.Parameters.Add("@param22", MySqlDbType.Bit).Value = firsttowerkill;
                    cmd.Parameters.Add("@param23", MySqlDbType.Bit).Value = gameendedinearlysurrender;
                    cmd.Parameters.Add("@param24", MySqlDbType.Bit).Value = gameendedinsurrender;
                    cmd.Parameters.Add("@param25", MySqlDbType.Int32).Value = goldearned;
                    cmd.Parameters.Add("@param26", MySqlDbType.Int32).Value = goldspent;
                    cmd.Parameters.Add("@param27", MySqlDbType.String).Value = individualposition;
                    cmd.Parameters.Add("@param28", MySqlDbType.Int32).Value = inhibitorkills;
                    cmd.Parameters.Add("@param29", MySqlDbType.Int32).Value = inhibitorslost;
                    cmd.Parameters.Add("@param30", MySqlDbType.Int32).Value = inhibitortakedowns;
                    cmd.Parameters.Add("@param31", MySqlDbType.Int32).Value = item0;
                    cmd.Parameters.Add("@param32", MySqlDbType.Int32).Value = item1;
                    cmd.Parameters.Add("@param33", MySqlDbType.Int32).Value = item2;
                    cmd.Parameters.Add("@param34", MySqlDbType.Int32).Value = item3;
                    cmd.Parameters.Add("@param35", MySqlDbType.Int32).Value = item4;
                    cmd.Parameters.Add("@param36", MySqlDbType.Int32).Value = item5;
                    cmd.Parameters.Add("@param37", MySqlDbType.Int32).Value = item6;
                    cmd.Parameters.Add("@param38", MySqlDbType.Int32).Value = itemspurchased;
                    cmd.Parameters.Add("@param39", MySqlDbType.Int32).Value = killingsprees;
                    cmd.Parameters.Add("@param40", MySqlDbType.Int32).Value = kills;
                    cmd.Parameters.Add("@param41", MySqlDbType.String).Value = lane;
                    cmd.Parameters.Add("@param42", MySqlDbType.Int32).Value = largestcriticalstrike;
                    cmd.Parameters.Add("@param43", MySqlDbType.Int32).Value = largestkillingspree;
                    cmd.Parameters.Add("@param44", MySqlDbType.Int32).Value = largestmultikill;
                    cmd.Parameters.Add("@param45", MySqlDbType.Int32).Value = longesttimespentliving;
                    cmd.Parameters.Add("@param46", MySqlDbType.Int32).Value = magicdamagedealt;
                    cmd.Parameters.Add("@param47", MySqlDbType.Int32).Value = magicdamagedealttochampions;
                    cmd.Parameters.Add("@param48", MySqlDbType.Int32).Value = magicdamagetaken;
                    cmd.Parameters.Add("@param49", MySqlDbType.Int32).Value = neutralminionskilled;
                    cmd.Parameters.Add("@param50", MySqlDbType.Int32).Value = nexuskills;
                    cmd.Parameters.Add("@param51", MySqlDbType.Int32).Value = nexuslost;
                    cmd.Parameters.Add("@param52", MySqlDbType.Int32).Value = nexustakedowns;
                    cmd.Parameters.Add("@param53", MySqlDbType.Int32).Value = objectivesstolen;
                    cmd.Parameters.Add("@param54", MySqlDbType.Int32).Value = objectivesstolenassists;
                    cmd.Parameters.Add("@param55", MySqlDbType.Int32).Value = pentakills;
                    cmd.Parameters.Add("@param56", MySqlDbType.Int32).Value = physicaldamagedealt;
                    cmd.Parameters.Add("@param57", MySqlDbType.Int32).Value = physicaldamagedealttochampions;
                    cmd.Parameters.Add("@param58", MySqlDbType.Int32).Value = physicaldamagetaken;
                    cmd.Parameters.Add("@param59", MySqlDbType.Int32).Value = profileicon;
                    cmd.Parameters.Add("@param60", MySqlDbType.String).Value = puuid;
                    cmd.Parameters.Add("@param61", MySqlDbType.Int32).Value = quadrakills;
                    cmd.Parameters.Add("@param62", MySqlDbType.String).Value = riotidtagline;
                    cmd.Parameters.Add("@param63", MySqlDbType.String).Value = role;
                    cmd.Parameters.Add("@param64", MySqlDbType.Int32).Value = sightwardsboughtingame;
                    cmd.Parameters.Add("@param65", MySqlDbType.Int32).Value = spell1casts;
                    cmd.Parameters.Add("@param66", MySqlDbType.Int32).Value = spell2casts;
                    cmd.Parameters.Add("@param67", MySqlDbType.Int32).Value = spell3casts;
                    cmd.Parameters.Add("@param68", MySqlDbType.Int32).Value = spell4casts;
                    cmd.Parameters.Add("@param69", MySqlDbType.Int32).Value = summoner1casts;
                    cmd.Parameters.Add("@param70", MySqlDbType.Int32).Value = summoner1id;
                    cmd.Parameters.Add("@param71", MySqlDbType.Int32).Value = summoner2casts;
                    cmd.Parameters.Add("@param72", MySqlDbType.Int32).Value = summoner2id;
                    cmd.Parameters.Add("@param73", MySqlDbType.String).Value = summonerid;
                    cmd.Parameters.Add("@param74", MySqlDbType.Int32).Value = summonerlevel;
                    cmd.Parameters.Add("@param75", MySqlDbType.String).Value = summonername;
                    cmd.Parameters.Add("@param76", MySqlDbType.Bit).Value = teamearlysurrendered;
                    cmd.Parameters.Add("@param77", MySqlDbType.String).Value = teamidenum;
                    cmd.Parameters.Add("@param78", MySqlDbType.String).Value = teamposition;
                    cmd.Parameters.Add("@param79", MySqlDbType.Int32).Value = timeccingothers;
                    cmd.Parameters.Add("@param80", MySqlDbType.Int32).Value = timeplayed;
                    cmd.Parameters.Add("@param81", MySqlDbType.Int32).Value = totaldamagedealt;
                    cmd.Parameters.Add("@param82", MySqlDbType.Int32).Value = totaldamagedealttochampions;
                    cmd.Parameters.Add("@param83", MySqlDbType.Int32).Value = totaldamageshieldedonteammates;
                    cmd.Parameters.Add("@param84", MySqlDbType.Int32).Value = totaldamagetaken;
                    cmd.Parameters.Add("@param85", MySqlDbType.Int32).Value = totalheal;
                    cmd.Parameters.Add("@param86", MySqlDbType.Int32).Value = totalhealsonteammates;
                    cmd.Parameters.Add("@param87", MySqlDbType.Int32).Value = totalminionskilled;
                    cmd.Parameters.Add("@param88", MySqlDbType.Int32).Value = totaltimeccdealt;
                    cmd.Parameters.Add("@param89", MySqlDbType.Int32).Value = totaltimespentdead;
                    cmd.Parameters.Add("@param90", MySqlDbType.Int32).Value = totalunitshealed;
                    cmd.Parameters.Add("@param91", MySqlDbType.Int32).Value = triplekills;
                    cmd.Parameters.Add("@param92", MySqlDbType.Int32).Value = truedamagedealt;
                    cmd.Parameters.Add("@param93", MySqlDbType.Int32).Value = truedamagedealttochampions;
                    cmd.Parameters.Add("@param94", MySqlDbType.Int32).Value = truedamagetaken;
                    cmd.Parameters.Add("@param95", MySqlDbType.Int32).Value = turretkills;
                    cmd.Parameters.Add("@param96", MySqlDbType.Int32).Value = turretslost;
                    cmd.Parameters.Add("@param97", MySqlDbType.Int32).Value = turrettakedowns;
                    cmd.Parameters.Add("@param98", MySqlDbType.Int32).Value = unrealkills;
                    cmd.Parameters.Add("@param99", MySqlDbType.Int32).Value = visionscore;
                    cmd.Parameters.Add("@param100", MySqlDbType.Int32).Value = visionwardsboughtingame;
                    cmd.Parameters.Add("@param101", MySqlDbType.Int32).Value = wardskilled;
                    cmd.Parameters.Add("@param102", MySqlDbType.Int32).Value = wardsplaced;
                    cmd.Parameters.Add("@param103", MySqlDbType.Bit).Value = win;                                          

                    cmd.CommandType = CommandType.Text;
                    rowseffected = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            message = rowseffected == 1 ? string.Format("Match Participants: MatchId: {0}", matchid) : string.Format("No Match Participants Effected, Error: {0}", error);
        }

        /// <summary>
        /// Insert Participant Stats
        /// </summary>
        public void InsertParticipantsStats(string region, string matchid, int participantid, int perksstatdefence, int perksstatflex, 
                                            int perkstatoffence, string perkstyle1json, string perkstyle2json, out string message)
        {
            int rowseffected = 0;
            message = string.Empty;
            string error = string.Empty;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(string.Format(INSERT_PARTICIPANTSTATS, region), m_Connection))
                {
                    cmd.Parameters.Add("@param1", MySqlDbType.String).Value = matchid;
                    cmd.Parameters.Add("@param2", MySqlDbType.Int32).Value = participantid;
                    cmd.Parameters.Add("@param3", MySqlDbType.Int32).Value = perksstatdefence;
                    cmd.Parameters.Add("@param4", MySqlDbType.Int32).Value = perksstatflex;
                    cmd.Parameters.Add("@param5", MySqlDbType.Int32).Value = perkstatoffence;
                    cmd.Parameters.Add("@param6", MySqlDbType.String).Value = perkstyle1json;
                    cmd.Parameters.Add("@param7", MySqlDbType.String).Value = perkstyle2json;

                    cmd.CommandType = CommandType.Text;
                    rowseffected = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            message = rowseffected == 1 ? string.Format("Participants Stats Inserted: MatchId: {0}", matchid) : string.Format("No Participants Stats Effected, Error: {0}", error);
        }

        /// <summary>
        /// Select From Match History
        /// </summary>
        public bool SelectFromMatchHistory(string region, string matchid, out Match match)
        {        
            match = new Match();

            bool result = GetMatchHistoryMainInfo(region, matchid, match);

            return result;
        }

        /// <summary>
        /// Get Match History Main Info
        /// </summary>
        private bool GetMatchHistoryMainInfo(string region, string matchid, Match match)
        {
            bool result = false;

            try
            {
                int[] participantids = new int[10];
                using (MySqlCommand command = new MySqlCommand(string.Format(SELECT_MATCHHISTORY, region), m_Connection))
                {
                    command.Parameters.AddWithValue("@param1", matchid);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = true;
                            match.Metadata = new Metadata();
                            match.Info = new Info();
                            match.Metadata.MatchId = reader.GetString("matchid");
                            match.Metadata.DataVersion = reader.GetString("dataversion");
                            match.Info.GameCreation = reader.GetInt64("gamecreation");
                            match.Info.GameDuration = reader.GetInt64("gameduration");
                            match.Info.GameEndTimestamp = reader.GetInt64("gameendtimestamp");
                            match.Info.GameId = reader.GetInt64("gameid");
                            GameMode gamemode;
                            if (Enum.TryParse(reader.GetString("gamemodeenum"), out gamemode))
                            {
                                match.Info.GameMode = gamemode;
                            }
                            match.Info.GameName = reader.GetString("gamename");
                            match.Info.GameStartTimestamp = reader.GetInt64("gamestarttimestamp");

                            GameType gametype;
                            if (Enum.TryParse(reader.GetString("gametypeenum"), out gametype))
                            {
                                match.Info.GameType = gametype;
                            }
                            match.Info.GameVersion = reader.GetString("gameversion");

                            Map mapid;
                            if (Enum.TryParse(reader.GetString("mapidenum"), out mapid))
                            {
                                match.Info.MapId = mapid;
                            }

                            match.Info.PlatformId = reader.GetString("platformid");

                            Queue queueid;
                            if (Enum.TryParse(reader.GetString("queueidenum"), out queueid))
                            {
                                match.Info.QueueId = queueid;
                            }

                            for (int i = 0; i < participantids.Length; i++)
                            {
                               participantids[i] = reader.GetInt32(string.Format("participant{0}id", (i + 1)));
                            }

                            match.Info.Teams = new Team[2];
                            Team team;
                            if (JsonHelper.TryDeserialize(reader.GetString("teamjson1"), out team))
                            {
                                match.Info.Teams[0] = team;
                            }

                            if (JsonHelper.TryDeserialize(reader.GetString("teamjson2"), out team))
                            {
                                match.Info.Teams[1] = team;
                            }
                        }
                    }
                }

                
                match.Metadata.Participants = new string[10];
                match.Info.Participants = new Participant[10];

                for(int i = 0; i < participantids.Length; i++)
                {
                    match.Info.Participants[i] = GetParticipant(region, matchid, participantids[i]);
                    match.Metadata.Participants[i] = match.Info.Participants[i].Puuid;
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        private Participant GetParticipant(string region, string matchid, int participantid)
        {
            Participant result = new Participant();
            result.ParticipantId = participantid;
            try
            {
                using (MySqlCommand command = new MySqlCommand(string.Format(SELECT_MATCHPARTICIPANT, region), m_Connection))
                {
                    command.Parameters.AddWithValue("@param1", matchid);
                    command.Parameters.AddWithValue("@param2", participantid);


                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.BaronKills = reader.GetInt32("baronkills");
                            result.BountyLevel = reader.GetInt32("bountylevel");
                            result.ChampExperience = reader.GetInt32("champexperience");
                            result.ChampionId = (Champion)reader.GetInt32("championidenum");
                            result.ChampionName = reader.GetString("championname");
                            result.ChampionTransform = reader.GetInt32("championtransform");
                            result.ChampLevel = reader.GetInt32("champlevel");
                            result.ConsumablesPurchased = reader.GetInt32("consumablespurchased");
                            result.DamageDealtToBuildings = reader.GetInt32("damagedealttobuildings");
                            result.DamageDealtToObjectives = reader.GetInt32("damagedealttoobjectives");
                            result.DamageDealtToTurrets = reader.GetInt32("damagedealttoturrets");
                            result.DamageSelfMitigated = reader.GetInt32("damageselfmitigated");
                            result.Deaths = reader.GetInt32("deaths");
                            result.DetectorWardsPlaced = reader.GetInt32("detectorwardsplaced");
                            result.DoubleKills = reader.GetInt32("doublekills");
                            result.DragonKills = reader.GetInt32("dragonkills");
                            result.FirstBloodAssist = reader.GetBoolean("firstbloodassist");
                            result.FirstBloodKill = reader.GetBoolean("firstbloodkill");
                            result.FirstTowerAssist = reader.GetBoolean("firsttowerassist");
                            result.FirstTowerKill = reader.GetBoolean("firsttowerkill");
                            result.GameEndedInEarlySurrender = reader.GetBoolean("gameendedinearlysurrender");
                            result.GameEndedInSurrender = reader.GetBoolean("gameendedinsurrender");
                            result.GoldEarned = reader.GetInt32("goldearned");
                            result.GoldSpent = reader.GetInt32("goldspent");
                            result.IndividualPosition = reader.GetString("individualposition");
                            result.InhibitorKills = reader.GetInt32("inhibitorkills");
                            result.InhibitorsLost = reader.GetInt32("inhibitorslost");
                            result.InhibitorTakedowns = reader["inhibitortakedowns"] == DBNull.Value ? null : reader.GetInt32("inhibitortakedowns");
                            result.Item0 = reader.GetInt32("item0");
                            result.Item1 = reader.GetInt32("item1");
                            result.Item2 = reader.GetInt32("item2");
                            result.Item3 = reader.GetInt32("item3");
                            result.Item4 = reader.GetInt32("item4");
                            result.Item5 = reader.GetInt32("item5");
                            result.Item6 = reader.GetInt32("item6");
                            result.ItemsPurchased = reader.GetInt32("itemspurchased");
                            result.KillingSprees = reader.GetInt32("killingsprees");
                            result.Kills = reader.GetInt32("kills");
                            result.Lane = reader.GetString("lane");
                            result.LargestCriticalStrike = reader.GetInt32("largestcriticalstrike");
                            result.LargestKillingSpree = reader.GetInt32("largestkillingspree");
                            result.LargestMultiKill = reader.GetInt32("largestmultikill");
                            result.LongestTimeSpentLiving = reader.GetInt32("longesttimespentliving");
                            result.MagicDamageDealt = reader.GetInt32("magicdamagedealt");
                            result.MagicDamageDealtToChampions = reader.GetInt32("magicdamagedealttochampions");
                            result.MagicDamageTaken = reader.GetInt32("magicdamagetaken");
                            result.NeutralMinionsKilled = reader.GetInt32("neutralminionskilled");
                            result.NexusKills = reader.GetInt32("nexuskills");
                            result.NexusLost = reader.GetInt32("nexuslost");
                            result.NexusTakedowns = reader["nexustakedowns"] == DBNull.Value ? null : reader.GetInt32("nexustakedowns");
                            result.ObjectivesStolen = reader.GetInt32("objectivesstolen");
                            result.ObjectivesStolenAssists = reader.GetInt32("objectivesstolenassists");
                            result.PentaKills = reader.GetInt32("pentakills");
                            result.PhysicalDamageDealt = reader.GetInt32("physicaldamagedealt");
                            result.PhysicalDamageDealtToChampions = reader.GetInt32("physicaldamagedealttochampions");
                            result.PhysicalDamageTaken = reader.GetInt32("physicaldamagetaken");
                            result.ProfileIcon = reader.GetInt32("profileicon");
                            result.Puuid = reader.GetString("puuid");
                            result.QuadraKills = reader.GetInt32("quadrakills");
                            result.RiotIdTagline = reader.GetString("riotidtagline");
                            result.Role = reader.GetString("role");
                            result.SightWardsBoughtInGame = reader.GetInt32("sightwardsboughtingame");
                            result.Spell1Casts = reader.GetInt32("spell1casts");
                            result.Spell2Casts = reader.GetInt32("spell2casts");
                            result.Spell3Casts = reader.GetInt32("spell3casts");
                            result.Spell4Casts = reader.GetInt32("spell4casts");
                            result.Summoner1Casts = reader.GetInt32("summoner1casts");
                            result.Summoner1Id = reader.GetInt32("summoner1id");
                            result.Summoner2Casts = reader.GetInt32("summoner2casts");
                            result.Summoner2Id = reader.GetInt32("summoner2id");
                            result.SummonerId = reader.GetString("summonerid");
                            result.SummonerLevel = reader.GetInt32("summonerlevel");
                            result.SummonerName = reader.GetString("summonername");
                            result.TeamEarlySurrendered = reader.GetBoolean("teamearlysurrendered");

                            Camille.RiotGames.Enums.Team teamid;
                            if (Enum.TryParse(reader.GetString("teamidenum"), out teamid))
                            {
                                result.TeamId = teamid;
                            }
                            result.TeamPosition = reader.GetString("teamposition");
                            result.TimeCCingOthers = reader.GetInt32("timeccingothers");
                            result.TimePlayed = reader.GetInt32("timeplayed");
                            result.TotalDamageDealt = reader.GetInt32("totaldamagedealt");
                            result.TotalDamageDealtToChampions = reader.GetInt32("totaldamagedealttochampions");
                            result.TotalDamageShieldedOnTeammates = reader.GetInt32("totaldamageshieldedonteammates");
                            result.TotalDamageTaken = reader.GetInt32("totaldamagetaken");
                            result.TotalHeal = reader.GetInt32("totalheal");
                            result.TotalHealsOnTeammates = reader.GetInt32("totalhealsonteammates");
                            result.TotalMinionsKilled = reader.GetInt32("totalminionskilled");
                            result.TotalTimeCCDealt = reader.GetInt32("totaltimeccdealt");
                            result.TotalTimeSpentDead = reader.GetInt32("totaltimespentdead");
                            result.TotalUnitsHealed = reader.GetInt32("totalunitshealed");
                            result.TripleKills = reader.GetInt32("triplekills");
                            result.TrueDamageDealt = reader.GetInt32("truedamagedealt");
                            result.TrueDamageDealtToChampions = reader.GetInt32("truedamagedealttochampions");
                            result.TrueDamageTaken = reader.GetInt32("truedamagetaken");
                            result.TurretKills = reader.GetInt32("turretkills");
                            result.TurretsLost = reader.GetInt32("turretslost");
                            result.TurretTakedowns = reader["turrettakedowns"] == DBNull.Value ? null : reader.GetInt32("turrettakedowns");
                            result.UnrealKills = reader.GetInt32("unrealkills");
                            result.VisionScore = reader.GetInt32("visionscore");
                            result.VisionWardsBoughtInGame = reader.GetInt32("visionwardsboughtingame");
                            result.WardsKilled = reader.GetInt32("wardskilled");
                            result.WardsPlaced = reader.GetInt32("wardsplaced");
                            result.Win = reader.GetBoolean("win");                          
                        }
                    }

                    result.Perks = GetParticipantStats(region, matchid, participantid);
                }
            }
            catch(Exception e)
            {

            }
            

            return result;
        }

        /// <summary>
        /// Get ParticipantStats
        /// </summary>
        private Perks GetParticipantStats(string region, string matchid, int participantid)
        {
            Perks result = new Perks();

            try
            {
                using (MySqlCommand command = new MySqlCommand(string.Format(SELECT_PARTICIPANTSTATS, region), m_Connection))
                {
                    command.Parameters.AddWithValue("@param1", matchid);
                    command.Parameters.AddWithValue("@param2", participantid);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.StatPerks = new PerkStats();
                            result.StatPerks.Offense = reader.GetInt32("perksstat_defence");
                            result.StatPerks.Flex = reader.GetInt32("perksstat_flex");
                            result.StatPerks.Defense = reader.GetInt32("perksstat_offence");
                            result.Styles = new PerkStyle[2];

                            PerkStyle perk;

                            if (JsonHelper.TryDeserialize(reader.GetString("perkstyle1_json"), out perk))
                            {
                                result.Styles[0] = perk;
                            }

                            if (JsonHelper.TryDeserialize(reader.GetString("perkstyle2_json"), out perk))
                            {
                                result.Styles[1] = perk;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        /// <summary>
        /// Open Connection
        /// </summary>
        public void Open()
        {
            if (m_Connection.State != ConnectionState.Open)
            {
                m_Connection.Open();
            }
        }

        /// <summary>
        /// Close Connection
        /// </summary>
        public void Close()
        {
            if (m_Connection.State != ConnectionState.Closed)
            {
                m_Connection.Close();
            }
        }
    }
}
