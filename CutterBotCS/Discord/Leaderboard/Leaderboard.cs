using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camille.Enums;
using CutterBotCS.RiotAPI;
using CutterBotCS.Worker;
using CutterDB.Entities;
using CutterDB.Tables;
using Discord.Rest;
using Discord.WebSocket;


namespace CutterBotCS.Discord
{
    /// <summary>
    /// Leaderboard
    /// </summary>
    public class Leaderboard
    {
        private DiscordBot m_Db;
        private RiotAPIHandler m_RiotHandler;
        private List<GuildItem> m_Guilds;
        private LeaderboardCreator m_Creator;
        private bool m_TimerOnClockInterval = false;

        /// <summary>
        /// Milliseconds (10 Minutes)
        /// </summary>
        private const double TIMER_INTERVAL = 60000 * 15;

        /// <summary>
        /// Timer to Draw Leaderboard
        /// </summary>
        System.Timers.Timer DrawTimer = new System.Timers.Timer();

        /// <summary>
        /// Constructor
        /// </summary>
        public Leaderboard(RiotAPIHandler riothandler)
        {
            m_RiotHandler = riothandler;
        }

        /// <summary>
        /// Intialize Leaderboard
        /// </summary>
        public void Initialize(List<GuildItem> guilditems, DiscordBot db)
        {
            m_Guilds = guilditems;
            m_Db = db;

            DrawTimer.Elapsed += DrawTimer_Elapsed;

            // Milliseconds
            DrawTimer.Interval = 5000;
            DrawTimer.Enabled = true;
        }

        /// <summary>
        /// Draw Timer Elapsed Event
        /// </summary>
        private async void DrawTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime timenow = DateTime.UtcNow;

            // Check its the 15th minute interval of the day
            if (m_TimerOnClockInterval || (timenow.Minute % 15 == 0) &&
                m_Guilds != null)
            {
                m_TimerOnClockInterval = true;
                DrawTimer.Interval = TIMER_INTERVAL;

                string botdbconnstring = Properties.Settings.Default.BotDBConn;
                string playerserror;
                PlayersTable pt = new PlayersTable();
                pt.OpenConnection(botdbconnstring, out playerserror);
                DiscordWorker.Log(playerserror, LogType.Error);

                string gterror;
                GuildTable gt = new GuildTable();
                gt.OpenConnection(botdbconnstring, out gterror);
                DiscordWorker.Log(gterror, LogType.Error);

                SocketTextChannel stc;
                foreach (var guild in m_Guilds)
                {
                    List<PlayerEntity> playerentities = new List<PlayerEntity>();

                    // Get Players from database
                    if (pt.TryGetGuildPlayers(guild.GuildId, out playerentities, out playerserror) &&
                        m_Db.TryGetSocketTextChannel(guild.GuildId, guild.LeaderboardChannelId, out stc))
                    {
                        List<Player> players = new List<Player>();

                        foreach (var entity in playerentities)
                        {
                            players.Add(new Player()
                            {
                                SummonerName = entity.SummonerName,
                                PlatformRoute = (PlatformRoute)entity.PlatformRoute,
                                RegionalRoute = (RegionalRoute)entity.RegionalRoute
                            });
                        }

                        if (guild.LeaderboardMessageId > 0)
                        {
                            try
                            {
                                // Delete Current Leaderboard
                                await stc.DeleteMessageAsync(guild.LeaderboardMessageId);
                            }
                            catch (Exception deletexception)
                            {
                                DiscordWorker.Log(string.Format("Error Deleting Leaderboard\r\nGuildId: {0} \r\nError: {1}", 
                                                                guild.GuildId, deletexception.Message), LogType.Error);
                            }
                        }        

                        List<LeaderboardEntry> boys = await m_RiotHandler.GetPlayersLeaderboardEntries(players, EntryFilter.None);
                        ulong messageid = await SendLeaderboard(boys, stc, guild.Id, guild.LeaderboardTitle);

                        guild.LeaderboardMessageId = messageid;
                        gt.UpdateLeaderboardMessageGuild(guild.GuildId, messageid, out gterror);
                        DiscordWorker.Log(gterror, LogType.Error);
                    }

                    DiscordWorker.Log(playerserror, LogType.Error);
                }                
            }         
        }

        /// <summary>
        /// Send Leaderboard Drawing to Discord Channel
        /// </summary>
        public async Task<ulong> SendLeaderboard(List<LeaderboardEntry> entries, SocketTextChannel channel, string id, string leaderboardtitle)
        {
            string image = string.Format(ProgramConstants.RESOURCE_IMAGES_DIR + "/{0}.png", id);

            if (m_Creator == null)
            {
                m_Creator = new LeaderboardCreator();
            }

            RestUserMessage rest = null;

            if (m_Creator.TryCreateLeaderboard(entries, image, id, leaderboardtitle))
            {
                rest = await channel.SendFileAsync(image, string.Empty);
            }

            return rest != null ? rest.Id : 0;
        }
    }
}
