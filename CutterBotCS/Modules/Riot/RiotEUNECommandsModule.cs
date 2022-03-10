using Camille.Enums;
using CutterBotCS.Discord;
using CutterBotCS.RiotAPI;
using Discord.Commands;
using System.Threading.Tasks;

namespace CutterBotCS.Modules.Riot
{
    /// <summary>
    /// Riot EUNE Commands Module
    /// </summary>
    [Group("RiotEUNE")]
    public class RiotEUNECommandsModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Riot Command Handler
        /// </summary>
        RiotCommandHandler m_RiotCommandHandler { get; set; }

        public RiotEUNECommandsModule(RiotAPIHandler handler)
        {
            m_RiotCommandHandler = new RiotCommandHandler(handler);
        }

        /// <summary>
        /// Riot EUNE Mastery for a summoner, e.g. !RiotEUNE Mastery CutterHealer
        /// </summary>
        [Command("RiotEUNE Mastery")]
        [Summary("Gets top 10 Champion Masteries of a summoner")]
        public async Task EUNEMasteries([Remainder] string summonername)
        {
            string message = await m_RiotCommandHandler.MasteryAsync(summonername, PlatformRoute.EUN1);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Riot EUNE History for a summoner, e.g. !RiotEUNE History CutterHealer
        /// </summary>
        [Command("RiotEUNE History")]
        [Summary("Get most recent 10 games of a summoner")]
        public async Task EUNEAsync([Remainder] string summonername)
        {
            MatchHistoryEmbedModel model = await m_RiotCommandHandler.GetEmbedMatchModel(summonername, PlatformRoute.EUN1, RegionalRoute.EUROPE);

            if (model != null)
            {
                await ReplyAsync(embed: model.Embed.Build());
            }
            else
            {
                await ReplyAsync("Error occured please contact CutterHealer#0001");
            }
        }       
    }
}
