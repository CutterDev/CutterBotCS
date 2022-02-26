using CutterBotCS.Worker;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace CutterBotCS.Modules
{
    /// <summary>
    /// Command Handler
    /// </summary>
    public class CommandHandler
    {
        private readonly DiscordSocketClient m_Client;
        private readonly CommandService m_Commands;
        private readonly IServiceProvider m_ServiceProvider;

        private Dictionary<ulong, char> m_GuildsPrefix;

        ConcurrentQueue<KeyValuePair<ulong, char>> m_NewGuilds;

        /// <summary>
        /// Ctor
        /// </summary>
        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider serviceprovider, Dictionary<ulong, char> guildprefixes)
        {
            m_Client = client;
            m_Commands = commands;
            m_ServiceProvider = serviceprovider;
            m_NewGuilds = new ConcurrentQueue<KeyValuePair<ulong, char>>();
            m_GuildsPrefix = guildprefixes;
        }

        /// <summary>
        /// Install Commands Async
        /// </summary>
        /// <returns></returns>
        public async Task InstallCommandsAsync()
        {
            m_Client.MessageReceived += HandleCommandAsync;

            // Add Modules Here
            await m_Commands.AddModulesAsync(assembly: Assembly.GetExecutingAssembly(), services: m_ServiceProvider);
        }

        /// <summary>
        /// Add New Guild
        /// </summary>
        public void AddNewGuild(ulong guildid, char prefix)
        {
            m_NewGuilds.Enqueue(new KeyValuePair<ulong, char>(guildid, prefix));
        }

        /// <summary>
        /// Check for New Guilds
        /// </summary>
        private void CheckForNewGuilds()
        {
            KeyValuePair<ulong, char> newguild;
            if (m_NewGuilds.Count > 0 && m_NewGuilds.TryDequeue(out newguild))
            {
                m_GuildsPrefix.Add(newguild.Key, newguild.Value);
            }
        }

        /// <summary>
        /// Handle Command Async
        /// </summary>
        private async Task HandleCommandAsync(SocketMessage arg)
        {
            CheckForNewGuilds();

            if (arg is SocketUserMessage && arg != null)
            {
                try
                {
                    var message = arg as SocketUserMessage;

                    var textchannel = arg.Channel as SocketTextChannel;

                    if (textchannel != null)
                    {
                        ulong guildid = textchannel.Guild.Id;

                        int argPos = 0;

                        if (m_GuildsPrefix.ContainsKey(guildid))
                        {
                            if (!(message.HasCharPrefix(m_GuildsPrefix[guildid], ref argPos) ||
                                  message.HasMentionPrefix(m_Client.CurrentUser, ref argPos)) ||
                                    message.Author.IsBot)
                                return;

                            var context = new SocketCommandContext(m_Client, message);

                            await m_Commands.ExecuteAsync(
                                context: context,
                                argPos: argPos,
                                services: m_ServiceProvider);
                        }
                    }

                }
                catch(Exception e)
                {
                    DiscordWorker.Log(string.Format("Error occured Handling Message:\r\n" +
                                      "Error: {0}", e.Message), LogType.Error);
                }
            }
            else
            {
                return;
            }
        }
    }
}
