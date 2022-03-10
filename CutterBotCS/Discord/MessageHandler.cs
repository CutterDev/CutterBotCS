using CutterBotCS.Worker;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Timers;

namespace CutterBotCS.Discord
{
    /// <summary>
    /// Handles messages for when they should be delete
    /// </summary>
    public class MessageHandler
    {
        private DiscordSocketClient m_Client;
        private Dictionary<ulong, MatchHistoryEmbedModel> m_MatchHistoryModels;
        private Timer m_MatchSelectorTimer;

        /// <summary>
        /// Constructor
        /// </summary>
        public MessageHandler()
        {
            m_MatchHistoryModels = new Dictionary<ulong, MatchHistoryEmbedModel>();
            m_MatchSelectorTimer = new Timer();
            m_MatchSelectorTimer.Interval = 5000;
            m_MatchSelectorTimer.Elapsed += MatchSelectorTimer_Elapsed;
            m_MatchSelectorTimer.Start();
        }

        /// <summary>
        /// Initialise
        /// </summary>
        public void Initialise(DiscordSocketClient client)
        {
            m_Client = client;
            m_MatchHistoryModels = new Dictionary<ulong, MatchHistoryEmbedModel>();
            m_MatchSelectorTimer = new Timer();
            m_MatchSelectorTimer.Interval = 5000;
            m_MatchSelectorTimer.Elapsed += MatchSelectorTimer_Elapsed;
            m_MatchSelectorTimer.Start();
        }

        /// <summary>
        /// Match Selector timer Elapsed event
        /// </summary>
        private void MatchSelectorTimer_Elapsed(object sender, ElapsedEventArgs eventargs)
        {
            if (m_Client.ConnectionState == ConnectionState.Connected)
            {
                List<ulong> modelstodelete = new List<ulong>();
                foreach (var model in m_MatchHistoryModels)
                {
                    if (model.Value == null || model.Value.Timestamp.AddMinutes(1) < DateTime.Now)
                    {
                        modelstodelete.Add(model.Key);
                    }
                }

                foreach (ulong discordid in modelstodelete)
                {
                    if (m_MatchHistoryModels.ContainsKey(discordid))
                    {
                        var model = m_MatchHistoryModels[discordid];
                        try
                        {
                            SocketGuild guild = m_Client.GetGuild(model.GuildId);

                            if (guild != null)
                            {
                                SocketTextChannel stc = guild.GetTextChannel(model.HistoryTextChannelId);

                                if (stc != null && model.HistoryMessageId > 0)
                                {
                                    stc.DeleteMessageAsync(model.HistoryMessageId);
                                }

                                if (stc != null && model.UserSentHistoryMessageId > 0)
                                {
                                    stc.DeleteMessageAsync(model.UserSentHistoryMessageId);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            DiscordWorker.Log(string.Format("Error Deleting Current History Model: {0}", e.Message), LogType.Error);
                        }


                        m_MatchHistoryModels.Remove(discordid);
                    }
                }
            }     
        }

        /// <summary>
        /// Add New Message Model
        /// </summary>
        public void AddNewMessageModel(ulong discordid, MatchHistoryEmbedModel model)
        {
            m_MatchHistoryModels.Add(discordid, model);
        }

        /// <summary>
        /// Try Get Model
        /// </summary>
        public bool TryGetModel(ulong discordid, out MatchHistoryEmbedModel model)
        {
            bool result = false;
            model = null;

            if (ContainsMessage(discordid))
            {
                result = true;
                model = m_MatchHistoryModels[discordid];
            }

            return result;
        }

        /// <summary>
        /// Contains Message
        /// </summary>
        public bool ContainsMessage(ulong discordid)
        {
            return m_MatchHistoryModels.ContainsKey(discordid);
        }
    }
}
