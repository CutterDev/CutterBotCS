using CutterBotCS.RiotAPI;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CutterBotCS.Modules.Leaderboard
{
    /// <summary>
    /// Leaderboard
    /// </summary>
    public class Leaderboard
    {
        private DiscordSocketClient m_DiscordClient;
        private RiotAPIHandler m_RiotHandler;

        private LeaderboardUICreator m_Creator;
        private ulong m_CurrentLeaderboardMessage;

        /// <summary>
        /// Milliseconds (10 Minutes)
        /// </summary>
        private const double TIMER_INTERVAL = 60000 * 10;

        /// <summary>
        /// Timer to Draw Leaderboard
        /// </summary>
        System.Timers.Timer DrawTimer = new System.Timers.Timer();

        /// <summary>
        /// Constructor
        /// </summary>
        public Leaderboard(RiotAPIHandler riothandler, DiscordSocketClient dsc)
        {
            m_RiotHandler = riothandler;
            m_DiscordClient = dsc;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            DrawTimer.Elapsed += DrawTimer_Elapsed;

            // 15 minutes
            DrawTimer.Interval = TIMER_INTERVAL;
        }

        /// <summary>
        /// Draw Timer Elapsed Event
        /// </summary>
        private async void DrawTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime timenow = DateTime.UtcNow;

            int minutes = timenow.Minute;

            // Check its the 15th minute interval of the day
            if (minutes % 15 == 0)
            {
                SocketGuild sg = m_DiscordClient.GetGuild(Properties.Settings.Default.DiscordGuildId);

                if (sg != null)
                {
                    SocketTextChannel stc = sg.GetTextChannel(Properties.Settings.Default.DiscordLeaderboardChannelId);
                    if (stc != null)
                    {
                        List<LeaderboardEntry> boys = await m_RiotHandler.GetBoyzLeagueAsync(EntryFilter.None);
                        await SendLeaderboard(boys, stc, false);
                    }
                }
            }         
        }

        /// <summary>
        /// Send Leaderboard Drawing to Discord Channel
        /// </summary>
        public async Task SendLeaderboard(List<LeaderboardEntry> entries, SocketTextChannel channel, bool usepodium)
        {
            string image = AppDomain.CurrentDomain.BaseDirectory + "/Resources/Images/leaderboard.png";

            if (m_CurrentLeaderboardMessage > 0)
            {
                await channel.DeleteMessageAsync(m_CurrentLeaderboardMessage);
            }

            if (m_Creator == null)
            {
                m_Creator = new LeaderboardUICreator();
                m_Creator.Initialize();
            }

            string errmessage;

            m_Creator.CreateLeaderboard(entries, image, out errmessage, usepodium);

            RestUserMessage rest;
            if (string.IsNullOrWhiteSpace(errmessage))
            {
                rest = await channel.SendFileAsync(image, string.Empty);
            }
            else
            {
                rest = await channel.SendMessageAsync(errmessage);
            }

            m_CurrentLeaderboardMessage = rest.Id;
        }
    }
}
