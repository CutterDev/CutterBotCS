using Discord.Commands;
using Discord.WebSocket;
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

        /// <summary>
        /// Ctor
        /// </summary>
        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            m_Client = client;
            m_Commands = commands;
        }

        /// <summary>
        /// Install Commands Async
        /// </summary>
        /// <returns></returns>
        public async Task InstallCommandsAsync()
        {
            m_Client.MessageReceived += HandleCommandAsync;

            // Add Modules Here
            await m_Commands.AddModulesAsync(assembly: Assembly.GetExecutingAssembly(), services: null);
        }

        /// <summary>
        /// Handle Command Async
        /// </summary>
        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg is SocketUserMessage && arg != null)
            {
                var message = arg as SocketUserMessage;

                int argPos = 0;

                if (!(message.HasCharPrefix(Properties.Settings.Default.CommandPrefix, ref argPos) ||
                    message.HasMentionPrefix(m_Client.CurrentUser, ref argPos)) ||
                    message.Author.IsBot)
                    return;

                var context = new SocketCommandContext(m_Client, message);

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
