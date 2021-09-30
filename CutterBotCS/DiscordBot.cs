using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutterBotCS
{
    public class DiscordBot
    {
        private readonly string m_Token = "Nzg4ODAwMTU3NDkxMjY1NTg3.X9oxZA.femuQ83txCOvNTKvdoq_c-h_wCE";
        private DiscordSocketClient m_Client;
        private DiscordCommandHandler m_CommandHandler;

        //custom event  
        public event ConnectedEventHandler connected;

        /// <summary>
        /// Ctor
        /// </summary>
        public DiscordBot(string token)
        {
            m_Token = token;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public async Task Initialize()
        {
            // Check Token is not empty/null
            if (string.IsNullOrWhiteSpace(m_Token))
            {
                return;
            }

            m_Client = new DiscordSocketClient();

            // Events
            m_Client.Log += Log;
            m_Client.Connected += ClientConnected;

            await m_Client.LoginAsync(TokenType.Bot, m_Token);
            await m_Client.StartAsync();

            m_CommandHandler = new DiscordCommandHandler(m_Client, new Discord.Commands.CommandService());
            await m_CommandHandler.InstallCommandAsync();

            await Task.Delay(-1);            
        }

        /// <summary>
        /// Client Connected
        /// </summary>
        /// <returns></returns>
        private Task ClientConnected()
        {
            connected();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Log Message
        /// </summary>
        private Task Log(LogMessage msg)
        {
            return Task.CompletedTask;
        }

        //custom delegate  
        public delegate void ConnectedEventHandler();
    }
}
