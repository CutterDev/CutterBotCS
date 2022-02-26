using Camille.Enums;
using CutterBotCS.RiotAPI;
using Discord.Commands;
using System.Threading.Tasks;

namespace CutterBotCS.Modules.Riot
{
    /// <summary>
    /// Riot EUW Commands Module
    /// </summary>
    [Group("RiotEUW")]
    public class RiotEUWCommandsModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Riot Command Handler
        /// </summary>
        RiotCommandHandler m_RiotCommandHandler { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RiotEUWCommandsModule(RiotAPIHandler handler)
        {
            m_RiotCommandHandler = new RiotCommandHandler(handler);
        }


        /// <summary>
        /// Riot EUW Mastery for a summoner, e.g. !RiotEUW Mastery CutterHealer
        /// </summary>
        [Command("Mastery")]
        [Summary("Gets top 10 Champion Masteries of a summoner")]
        public async Task UnregisteredEUWMasteries([Remainder] string summonername)
        {
            string message = await m_RiotCommandHandler.MasteryAsync(summonername, PlatformRoute.EUW1);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Riot NA History for a summoner, e.g. !RiotEUW History CutterHealer
        /// </summary>
        [Command("History")]
        [Summary("Get most recent 10 games of a summoner")]
        public async Task MatchHistoryEUWAsync([Remainder] string summonername)
        {
            string message = await m_RiotCommandHandler.GetMatchHistoryAsync(summonername, PlatformRoute.EUW1, RegionalRoute.EUROPE);

            await ReplyAsync(message);
        }
    }
}
