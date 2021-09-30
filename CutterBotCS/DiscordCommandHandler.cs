using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CutterBotCS
{
    public class DiscordCommandHandler
    {
        private readonly char COMMAND_PREFIX = '?';
        private readonly DiscordSocketClient m_Client;
        private readonly CommandService m_Commands;

        /// <summary>
        /// Ctor
        /// </summary>
        public DiscordCommandHandler(DiscordSocketClient client, CommandService commands)
        {
            m_Client = client;
            m_Commands = commands;         
        }

        public async Task InstallCommandAsync()
        {
            m_Client.MessageReceived += HandleCommandAsync;

            await m_Commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                        services: null);
        }
        
        /// <summary>
        /// Handle Command Async
        /// </summary>
        private async Task HandleCommandAsync(SocketMessage socketmsg)
        {
            if (socketmsg is SocketUserMessage && socketmsg != null)
            {
                var msg = socketmsg as SocketUserMessage;

                // Create a number to track where the prefix ends and the command begins
                int argPos = 0;

                // Check for the prefix and make sure no bots trigger commands
                if (!(msg.HasCharPrefix(COMMAND_PREFIX, ref argPos) || msg.HasMentionPrefix(m_Client.CurrentUser, ref argPos) ||
                     msg.Author.IsBot))
                {
                    return;
                }

                // Create a websocket-based command context based on the message???
                var context = new SocketCommandContext(m_Client, msg);

                // execute the async....?
                await m_Commands.ExecuteAsync(
                    context: context,
                    argPos: argPos,
                    services: null);
            }
            else
            {
                return;
            }
        }
    }
}
