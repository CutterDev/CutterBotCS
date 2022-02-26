using Camille.Enums;
using Camille.RiotGames.MatchV5;
using CutterBotCS.Helpers;
using CutterBotCS.Worker;
using CutterDB.Entities;
using CutterDB.Tables;
using System;

namespace CutterBotCS.Modules.Riot
{
    /// <summary>
    /// Match History Database Handler
    /// </summary>
    public static class MatchHistoryDBHandler
    {
        /// <summary>
        /// Insert Match to Database
        /// </summary>
        public static bool InsertMatchToDatabase(Match match, string region)
        {
            bool result = false;

            MatchHistoryTable matchhistorytable = new MatchHistoryTable();
            string connstring = Properties.Settings.Default.LeagueDBConn;

            string dberror = string.Empty;
            matchhistorytable.OpenConnection(connstring, out dberror);
            DiscordWorker.Log(dberror, LogType.Error);

            var matchentity = GetMatchEntity(match);
            matchhistorytable.InsertEntity(matchentity, region, out dberror);
            DiscordWorker.Log(dberror, LogType.Error);

            ParticipantTable participanttable = new ParticipantTable();
            participanttable.OpenConnection(connstring, out dberror);
            DiscordWorker.Log(dberror, LogType.Error);

            ParticipantPerkStatsTable perkstable = new ParticipantPerkStatsTable();
            perkstable.OpenConnection(connstring, out dberror);
            DiscordWorker.Log(dberror, LogType.Error);

            for (int i = 0; i < 10; i++)
            {
                var p = match.Info.Participants[i];
                var pentity = GetParticipantEntity(match.Metadata.MatchId, p);
                participanttable.InsertEntity(pentity, region, out dberror);
                DiscordWorker.Log(dberror, LogType.Error);

                var statsentity = GetParticipantPerkStats(match.Metadata.MatchId, p);
                perkstable.InsertEntity(statsentity,region,  out dberror);
                DiscordWorker.Log(dberror, LogType.Error);
            }

            matchhistorytable.CloseConnection(out dberror);
            DiscordWorker.Log(dberror, LogType.Error);
            participanttable.CloseConnection(out dberror);
            DiscordWorker.Log(dberror, LogType.Error);

            perkstable.CloseConnection(out dberror);
            DiscordWorker.Log(dberror, LogType.Error);

            result = !string.IsNullOrWhiteSpace(dberror);

            return result;
        }

        /// <summary>
        /// Get Match From Database
        /// </summary>
        public static bool GetMatchFromDatabase(string matchid, string region, out Match match)
        {
            bool result = false;
            match = null;
            string dberror;
            string connstring = Properties.Settings.Default.LeagueDBConn;

            MatchHistoryTable matchhistorytable = new MatchHistoryTable();
            matchhistorytable.OpenConnection(connstring, out dberror);

            MatchEntity entity;
            if (matchhistorytable.TryGetEntity(matchid, region, out entity, out dberror))
            {
                match = GetMatch(entity);

                ParticipantTable participanttable = new ParticipantTable();
                participanttable.OpenConnection(connstring, out dberror);
                DiscordWorker.Log(dberror, LogType.Error);

                ParticipantPerkStatsTable perkstable = new ParticipantPerkStatsTable();
                perkstable.OpenConnection(connstring, out dberror);
                DiscordWorker.Log(dberror, LogType.Error);

                for (int i = 0; i < match.Info.Participants.Length; i++)
                {
                    var p = match.Info.Participants[i];

                    ParticipantEntity pentity;
                    if (participanttable.TryGetEntity(matchid, p.ParticipantId, region, out pentity, out dberror))
                    {
                        GetParticipantDetails(pentity, p);

                        ParticipantPerkStatsEntity perksentity;
                        if (perkstable.TryGetEntity(matchid, p.ParticipantId, region, out perksentity, out dberror))
                        {
                            GetPerkStats(perksentity, p);
                        }

                        DiscordWorker.Log(dberror, LogType.Error);
                    }

                    DiscordWorker.Log(dberror, LogType.Error);
                }

                matchhistorytable.CloseConnection(out dberror);
                DiscordWorker.Log(dberror, LogType.Error);
                participanttable.CloseConnection(out dberror);
                DiscordWorker.Log(dberror, LogType.Error);
                perkstable.CloseConnection(out dberror);
                DiscordWorker.Log(dberror, LogType.Error);

            }

            DiscordWorker.Log(dberror, LogType.Error);

            return result;
        }

        /// <summary>
        /// Get Match from Entity
        /// </summary>
        private static Match GetMatch(MatchEntity entity)
        {
            Match match = new Match();
            try
            {
                match.Metadata = new Metadata();
                match.Metadata.MatchId = entity.MatchId;
                match.Metadata.DataVersion = entity.DataVersion;
                match.Info = new Info();
                match.Info.GameCreation = entity.GameCreation;
                match.Info.GameDuration = entity.GameDuration;
                match.Info.GameEndTimestamp = entity.GameEndTimestamp;
                match.Info.GameId = entity.GameId;

                GameMode gamemode;
                if (Enum.TryParse(entity.GameModeEnum, out gamemode))
                {
                    match.Info.GameMode = gamemode;
                }

                match.Info.GameName = entity.GameName;
                match.Info.GameStartTimestamp = entity.GameStartTimestamp;

                GameType gametype;
                if (Enum.TryParse(entity.GameTypeEnum, out gametype))
                {
                    match.Info.GameType = gametype;
                }

                match.Info.GameVersion = entity.GameVersion;

                Map map;
                if (Enum.TryParse(entity.MapIdEnum, out map))
                {
                    match.Info.MapId = map;
                }

                match.Info.PlatformId = entity.PlatformId;

                Queue queue;
                if (Enum.TryParse(entity.QueueIdEnum, out queue))
                {
                    match.Info.QueueId = queue;
                }

                match.Info.Participants = new Participant[10];
                match.Info.Participants[0] = NewParticipant(entity.Participant1Id);
                match.Info.Participants[1] = NewParticipant(entity.Participant2Id);
                match.Info.Participants[2] = NewParticipant(entity.Participant3Id);
                match.Info.Participants[3] = NewParticipant(entity.Participant4Id);
                match.Info.Participants[4] = NewParticipant(entity.Participant5Id);
                match.Info.Participants[5] = NewParticipant(entity.Participant6Id);
                match.Info.Participants[6] = NewParticipant(entity.Participant7Id);
                match.Info.Participants[7] = NewParticipant(entity.Participant8Id);
                match.Info.Participants[8] = NewParticipant(entity.Participant9Id);
                match.Info.Participants[9] = NewParticipant(entity.Participant10Id);

                match.Info.Teams = new Team[2];
                Team team;
                if (JsonHelper.TryDeserialize(entity.TeamJson1, out team))
                {
                    match.Info.Teams[0] = team;
                }

                if (JsonHelper.TryDeserialize(entity.TeamJson2, out team))
                {
                    match.Info.Teams[1] = team;
                }
            }
            catch (Exception e)
            {
                DiscordWorker.Log(string.Format("Error Getting Match from Entity: {0}", e.Message), LogType.Error);
            }

            return match;
        }

        private static Participant NewParticipant(int participantid)
        {
            Participant participant = new Participant();
            participant.ParticipantId = participantid;

            return participant;
        }

        /// <summary>
        /// Get Match Entity
        /// </summary>
        private static MatchEntity GetMatchEntity(Match match)
        {
            MatchEntity entity = new MatchEntity();

            try
            {             
                string team1json = string.Empty;
                string team2json = string.Empty;
                JsonHelper.TrySerialize(match.Info.Teams[0], out team1json);
                JsonHelper.TrySerialize(match.Info.Teams[1], out team2json);

                entity.MatchId = match.Metadata.MatchId;
                entity.DataVersion = match.Metadata.DataVersion;
                entity.GameCreation = match.Info.GameCreation;
                entity.GameDuration = match.Info.GameDuration;
                entity.GameEndTimestamp = match.Info.GameEndTimestamp;
                entity.GameId = match.Info.GameId;
                entity.GameModeEnum = match.Info.GameMode.ToString();
                entity.GameName = match.Info.GameName;
                entity.GameStartTimestamp = match.Info.GameStartTimestamp;
                entity.GameTypeEnum = match.Info.GameType.ToString();
                entity.GameVersion = match.Info.GameVersion;
                entity.MapIdEnum = match.Info.MapId.ToString();
                entity.PlatformId = match.Info.PlatformId;
                entity.QueueIdEnum = match.Info.QueueId.ToString();
                entity.Participant1Id = match.Info.Participants[0].ParticipantId;
                entity.Participant2Id = match.Info.Participants[1].ParticipantId;
                entity.Participant3Id = match.Info.Participants[2].ParticipantId;
                entity.Participant4Id = match.Info.Participants[3].ParticipantId;
                entity.Participant5Id = match.Info.Participants[4].ParticipantId;
                entity.Participant6Id = match.Info.Participants[5].ParticipantId;
                entity.Participant7Id = match.Info.Participants[6].ParticipantId;
                entity.Participant8Id = match.Info.Participants[7].ParticipantId;
                entity.Participant9Id = match.Info.Participants[8].ParticipantId;
                entity.Participant10Id = match.Info.Participants[9].ParticipantId;
                entity.TeamJson1 = team1json;
                entity.TeamJson2 = team2json;
            }
            catch (Exception e)
            {
                DiscordWorker.Log(string.Format("Error Getting MatchEntity from Match: {0}", e.Message), LogType.Error);
            } 

            return entity;
        }

        /// <summary>
        /// Get Participant Details
        /// </summary>
        private static void GetParticipantDetails(ParticipantEntity entity, Participant p)
        {
            try
            {
                p.BaronKills = entity.BaronKills;
                p.BountyLevel = entity.BountyLevel;
                p.ChampExperience = entity.ChampExperience;
                p.ChampionId = (Champion)entity.ChampionIdEnum;
                p.ChampionName = entity.ChampionName;
                p.ChampionTransform = entity.ChampionTransform;
                p.ChampLevel = entity.ChampLevel;
                p.ConsumablesPurchased = entity.ConsumablesPurchased;
                p.DamageDealtToBuildings = entity.DamageDealtToBuildings;
                p.DamageDealtToObjectives = entity.DamageDealtToObjectives;
                p.DamageDealtToTurrets = entity.DamageDealtToTurrets;
                p.DamageSelfMitigated = entity.DamageSelfMitigated;
                p.Deaths = entity.Deaths;
                p.DetectorWardsPlaced = entity.DetectorWardsPlaced;
                p.DoubleKills = entity.DoubleKills;
                p.DragonKills = entity.DragonKills;
                p.FirstBloodAssist = entity.FirstBloodAssist;
                p.FirstBloodKill = entity.FirstBloodKill;
                p.FirstTowerAssist = entity.FirstTowerAssist;
                p.FirstTowerKill = entity.FirstTowerKill;
                p.GameEndedInEarlySurrender = entity.GameEndedInEarlySurrender;
                p.GameEndedInSurrender = entity.GameEndedInSurrender;
                p.GoldEarned = entity.GoldEarned;
                p.GoldSpent = entity.GoldSpent;
                p.IndividualPosition = entity.IndividualPosition;
                p.InhibitorKills = entity.InhibitorKills;
                p.InhibitorsLost = entity.InhibitorsLost;
                p.InhibitorTakedowns = entity.InhibitorTakedowns;
                p.Item0 = entity.Item0;
                p.Item1 = entity.Item1;
                p.Item2 = entity.Item2;
                p.Item3 = entity.Item3;
                p.Item4 = entity.Item4;
                p.Item5 = entity.Item5;
                p.Item6 = entity.Item6;
                p.ItemsPurchased = entity.ItemsPurchased;
                p.KillingSprees = entity.KillingSprees;
                p.Kills = entity.Kills;
                p.Lane = entity.Lane;
                p.LargestCriticalStrike = entity.LargestCriticalStrike;
                p.LargestKillingSpree = entity.LargestKillingSpree;
                p.LargestMultiKill = entity.LargestMultikill;
                p.LongestTimeSpentLiving = entity.LongestTimeSpentLiving;
                p.MagicDamageDealt = entity.MagicDamageDealt;
                p.MagicDamageDealtToChampions = entity.MagicDamageDealtToChampions;
                p.MagicDamageTaken = entity.MagicDamageTaken;
                p.NeutralMinionsKilled = entity.NeutralMinionsKilled;
                p.NexusKills = entity.NexusKills;
                p.NexusLost = entity.NexusLost;
                p.NexusTakedowns = entity.NexusTakedowns;
                p.ObjectivesStolen = entity.ObjectivesStolen;
                p.ObjectivesStolenAssists = entity.ObjectivesStolenAssists;
                p.PentaKills = entity.PentaKills;
                p.PhysicalDamageDealt = entity.PhysicalDamageDealt;
                p.PhysicalDamageDealtToChampions = entity.PhysicalDamageDealtToChampions;
                p.PhysicalDamageTaken = entity.PhysicalDamageTaken;
                p.ProfileIcon = entity.ProfileIcon;
                p.Puuid = entity.Puuid;
                p.QuadraKills = entity.QuadraKills;
                p.RiotIdTagline = entity.RiotIdTagline;
                p.Role = entity.Role;
                p.SightWardsBoughtInGame = entity.SightWardsBoughtInGame;
                p.Spell1Casts = entity.Spell1Casts;
                p.Spell2Casts = entity.Spell2Casts;
                p.Spell3Casts = entity.Spell3Casts;
                p.Spell4Casts = entity.Spell4Casts;
                p.Summoner1Casts = entity.Summoner1Casts;
                p.Summoner1Id = entity.Summoner1Id;
                p.Summoner2Casts = entity.Summoner2Casts;
                p.Summoner2Id = entity.Summoner2Id;
                p.SummonerId = entity.SummonerId;
                p.SummonerLevel = entity.SummonerLevel;
                p.SummonerName = entity.SummonerName;
                p.TeamEarlySurrendered = entity.TeamEarlySurrendered;

                Camille.RiotGames.Enums.Team team;
                if (Enum.TryParse(entity.TeamIdEnum, out team))
                {
                    p.TeamId = team;
                }

                p.TeamPosition = entity.TeamPosition;
                p.TimeCCingOthers = entity.TimeCcingOthers;
                p.TimePlayed = entity.TimePlayed;
                p.TotalDamageDealt = entity.TotalDamageDealt;
                p.TotalDamageDealtToChampions = entity.TotalDamageDealtToChampions;
                p.TotalDamageShieldedOnTeammates = entity.TotalDamageShieldedOnTeammates;
                p.TotalDamageTaken = entity.TotalDamageTaken;
                p.TotalHeal = entity.TotalHeal;
                p.TotalHealsOnTeammates = entity.TotalHealsOnTeammates;
                p.TotalMinionsKilled = entity.TotalMinionsKilled;
                p.TotalTimeCCDealt = entity.TotalTimeCcDealt;
                p.TotalTimeSpentDead = entity.TotalTimeSpentDead;
                p.TotalUnitsHealed = entity.TotalUnitsHealed;
                p.TripleKills = entity.TripleKills;
                p.TrueDamageDealt = entity.TrueDamageDealt;
                p.TrueDamageDealtToChampions = entity.TrueDamageDealtToChampions;
                p.TrueDamageTaken = entity.TrueDamageTaken;
                p.TurretKills = entity.TurretKills;
                p.TurretsLost = entity.TurretsLost;
                p.TurretTakedowns = entity.TurretTakedowns;
                p.UnrealKills = entity.UnrealKills;
                p.VisionScore = entity.VisionScore;
                p.VisionWardsBoughtInGame = entity.VisionWardsBoughtInGame;
                p.WardsKilled = entity.WardsKilled;
                p.WardsPlaced = entity.WardsPlaced;
                p.Win = entity.Win;
            }
            catch (Exception e)
            {
                DiscordWorker.Log(string.Format("Error Getting Participant from Entity to Participant: {0}", e.Message), LogType.Error);
            }
        }

        /// <summary>
        /// Get Participant Entity
        /// </summary>
        private static ParticipantEntity GetParticipantEntity(string matchid, Participant p)
        {
            ParticipantEntity pentity = new ParticipantEntity();

            try
            {
                pentity.MatchId = matchid;
                pentity.ParticipantId = p.ParticipantId;
                pentity.BaronKills = p.BaronKills;
                pentity.BountyLevel = p.BountyLevel;
                pentity.ChampExperience = p.ChampExperience;
                pentity.ChampionIdEnum = (int)p.ChampionId;
                pentity.ChampionName = p.ChampionName;
                pentity.ChampionTransform = p.ChampionTransform;
                pentity.ChampLevel = p.ChampLevel;
                pentity.ConsumablesPurchased = p.ConsumablesPurchased;
                pentity.DamageDealtToBuildings = p.DamageDealtToBuildings;
                pentity.DamageDealtToObjectives = p.DamageDealtToObjectives;
                pentity.DamageDealtToTurrets = p.DamageDealtToTurrets;
                pentity.DamageSelfMitigated = p.DamageSelfMitigated;
                pentity.Deaths = p.Deaths;
                pentity.DetectorWardsPlaced = p.DetectorWardsPlaced;
                pentity.DoubleKills = p.DoubleKills;
                pentity.DragonKills = p.DragonKills;
                pentity.FirstBloodAssist = p.FirstBloodAssist;
                pentity.FirstBloodKill = p.FirstBloodKill;
                pentity.FirstTowerAssist = p.FirstTowerAssist;
                pentity.FirstTowerKill = p.FirstTowerKill;
                pentity.GameEndedInEarlySurrender = p.GameEndedInEarlySurrender;
                pentity.GameEndedInSurrender = p.GameEndedInSurrender;
                pentity.GoldEarned = p.GoldEarned;
                pentity.GoldSpent = p.GoldSpent;
                pentity.IndividualPosition = p.IndividualPosition;
                pentity.InhibitorKills = p.InhibitorKills;
                pentity.InhibitorsLost = p.InhibitorsLost;
                pentity.InhibitorTakedowns = p.InhibitorTakedowns;
                pentity.Item0 = p.Item0;
                pentity.Item1 = p.Item1;
                pentity.Item2 = p.Item2;
                pentity.Item3 = p.Item3;
                pentity.Item4 = p.Item4;
                pentity.Item5 = p.Item5;
                pentity.Item6 = p.Item6;
                pentity.ItemsPurchased = p.ItemsPurchased;
                pentity.KillingSprees = p.KillingSprees;
                pentity.Kills = p.Kills;
                pentity.Lane = p.Lane;
                pentity.LargestCriticalStrike = p.LargestCriticalStrike;
                pentity.LargestKillingSpree = p.LargestKillingSpree;
                pentity.LargestMultikill = p.LargestMultiKill;
                pentity.LongestTimeSpentLiving = p.LongestTimeSpentLiving;
                pentity.MagicDamageDealt = p.MagicDamageDealt;
                pentity.MagicDamageDealtToChampions = p.MagicDamageDealtToChampions;
                pentity.MagicDamageTaken = p.MagicDamageTaken;
                pentity.NeutralMinionsKilled = p.NeutralMinionsKilled;
                pentity.NexusKills = p.NexusKills;
                pentity.NexusLost = p.NexusLost;
                pentity.NexusTakedowns = p.NexusTakedowns;
                pentity.ObjectivesStolen = p.ObjectivesStolen;
                pentity.ObjectivesStolenAssists = p.ObjectivesStolenAssists;
                pentity.PentaKills = p.PentaKills;
                pentity.PhysicalDamageDealt = p.PhysicalDamageDealt;
                pentity.PhysicalDamageDealtToChampions = p.PhysicalDamageDealtToChampions;
                pentity.PhysicalDamageTaken = p.PhysicalDamageTaken;
                pentity.ProfileIcon = p.ProfileIcon;
                pentity.Puuid = p.Puuid;
                pentity.QuadraKills = p.QuadraKills;
                pentity.RiotIdTagline = p.RiotIdTagline;
                pentity.Role = p.Role;
                pentity.SightWardsBoughtInGame = p.SightWardsBoughtInGame;
                pentity.Spell1Casts = p.Spell1Casts;
                pentity.Spell2Casts = p.Spell2Casts;
                pentity.Spell3Casts = p.Spell3Casts;
                pentity.Spell4Casts = p.Spell4Casts;
                pentity.Summoner1Casts = p.Summoner1Casts;
                pentity.Summoner1Id = p.Summoner1Id;
                pentity.Summoner2Casts = p.Summoner2Casts;
                pentity.Summoner2Id = p.Summoner2Id;
                pentity.SummonerId = p.SummonerId;
                pentity.SummonerLevel = p.SummonerLevel;
                pentity.SummonerName = p.SummonerName;
                pentity.TeamEarlySurrendered = p.TeamEarlySurrendered;
                pentity.TeamIdEnum = p.TeamId.ToString();
                pentity.TeamPosition = p.TeamPosition;
                pentity.TimeCcingOthers = p.TimeCCingOthers;
                pentity.TimePlayed = p.TimePlayed;
                pentity.TotalDamageDealt = p.TotalDamageDealt;
                pentity.TotalDamageDealtToChampions = p.TotalDamageDealtToChampions;
                pentity.TotalDamageShieldedOnTeammates = p.TotalDamageShieldedOnTeammates;
                pentity.TotalDamageTaken = p.TotalDamageTaken;
                pentity.TotalHeal = p.TotalHeal;
                pentity.TotalHealsOnTeammates = p.TotalHealsOnTeammates;
                pentity.TotalMinionsKilled = p.TotalMinionsKilled;
                pentity.TotalTimeCcDealt = p.TotalTimeCCDealt;
                pentity.TotalTimeSpentDead = p.TotalTimeSpentDead;
                pentity.TotalUnitsHealed = p.TotalUnitsHealed;
                pentity.TripleKills = p.TripleKills;
                pentity.TrueDamageDealt = p.TrueDamageDealt;
                pentity.TrueDamageDealtToChampions = p.TrueDamageDealtToChampions;
                pentity.TrueDamageTaken = p.TrueDamageTaken;
                pentity.TurretKills = p.TurretKills;
                pentity.TurretsLost = p.TurretsLost;
                pentity.TurretTakedowns = p.TurretTakedowns;
                pentity.UnrealKills = p.UnrealKills;
                pentity.VisionScore = p.VisionScore;
                pentity.VisionWardsBoughtInGame = p.VisionWardsBoughtInGame;
                pentity.WardsKilled = p.WardsKilled;
                pentity.WardsPlaced = p.WardsPlaced;
                pentity.Win = p.Win;
            }
            catch (Exception e)
            {
                DiscordWorker.Log(string.Format("Error Getting Participant to Participant Entity: {0}", e.Message), LogType.Error);
            }

            return pentity;
        }

        /// <summary>
        /// Get Perk Stats
        /// </summary>
        private static void GetPerkStats(ParticipantPerkStatsEntity entity, Participant p)
        {
            try
            {
                p.Perks = new Perks();
                p.Perks.StatPerks = new PerkStats();
                p.Perks.StatPerks.Defense = p.Perks.StatPerks.Defense;
                p.Perks.StatPerks.Flex = p.Perks.StatPerks.Flex;
                p.Perks.StatPerks.Defense = p.Perks.StatPerks.Offense;

                p.Perks.Styles = new PerkStyle[2];

                PerkStyle ps;
                if (JsonHelper.TryDeserialize(entity.PerkStyle1Json, out ps))
                {
                    p.Perks.Styles[0] = ps;
                }

                if (JsonHelper.TryDeserialize(entity.PerkStyle2Json, out ps))
                {
                    p.Perks.Styles[1] = ps;
                }
            }
            catch (Exception e)
            {
                DiscordWorker.Log(string.Format("Error Getting Perk Stats from Entity: {0}", e.Message), LogType.Error);
            }
        }

        /// <summary>
        /// Get Participant Perk Stats
        /// </summary>
        private static ParticipantPerkStatsEntity GetParticipantPerkStats(string matchid, Participant p)
        {
            ParticipantPerkStatsEntity pstatsentity = new ParticipantPerkStatsEntity();
            try
            {
                string perks1json;
                string perks2json;

                JsonHelper.TrySerialize(p.Perks.Styles[0], out perks1json);
                JsonHelper.TrySerialize(p.Perks.Styles[1], out perks2json);

                pstatsentity.MatchId = matchid;
                pstatsentity.ParticipantId = p.ParticipantId;
                pstatsentity.PerksStatDefence = p.Perks.StatPerks.Defense;
                pstatsentity.PerksStatFlex = p.Perks.StatPerks.Flex;
                pstatsentity.PerksStatOffence = p.Perks.StatPerks.Offense;
                pstatsentity.PerkStyle1Json = perks1json;
                pstatsentity.PerkStyle2Json = perks2json;
            }
            catch (Exception e)
            {
                DiscordWorker.Log(string.Format("Error Getting Perk Stats from Participant to Entity: {0}", e.Message), LogType.Error);
            }

            return pstatsentity;
        }
    }
}
