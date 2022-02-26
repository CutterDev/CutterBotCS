using CutterBotCS.RiotAPI;
using CutterBotCS.Worker;
using CutterDragon;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CutterBotCS.Discord
{
    public class DiscordBotManager
    {
        /// <summary>
        /// Discord Bot
        /// </summary>
        private DiscordBot m_DiscordBot { get; set; }

        /// <summary>
        /// Riot Handler
        /// </summary>
        private RiotAPIHandler m_RiotHandler { get; set; }

        /// <summary>
        /// Leaderboard for League
        /// </summary>
        private Leaderboard m_Leaderboard { get; set; }

        /// <summary>
        /// Cutrter Dragon Worker
        /// </summary>
        private CutterDragonWorker m_CutterDragonWorker { get; set; }

        /// <summary>
        /// Discord Bot Manager
        /// </summary>
        public DiscordBotManager(CutterDragonWorker cwd)
        {
            m_CutterDragonWorker = cwd;
        }

        /// <summary>
        /// Intialise
        /// </summary>
        public void Initialise(string discordbottoken, string riotapitoken)
        {
            m_RiotHandler = new RiotAPIHandler();
            m_RiotHandler.Initialize(riotapitoken);

            m_Leaderboard = new Leaderboard(m_RiotHandler);

            InitiateBot(discordbottoken);
        }

        /// <summary>
        /// Initiate Bots
        /// </summary>
        private async void InitiateBot(string discordbottoken)
        {
            try
            {
                m_DiscordBot = new DiscordBot(discordbottoken, m_Leaderboard);
                await m_DiscordBot.Initialize(BuildServiceProvider());                          
            }
            catch (Exception e)
            {
                DiscordWorker.Log(String.Format("Error Initializing Bots: {0}", e.Message), LogType.Error);
            }          
        }

        /// <summary>
        /// Build Service Provider
        /// </summary>
        private IServiceCollection BuildServiceProvider()
        {
            return new ServiceCollection()
            .AddSingleton(this.m_RiotHandler)
            .AddSingleton(this.m_CutterDragonWorker);
        } 
    }
}
