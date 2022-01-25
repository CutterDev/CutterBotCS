using Camille.Enums;
using Discord.Commands;
using System.Threading.Tasks;

namespace CutterBotCS.Modules.Riot
{
    /// <summary>
    /// Riot NA Commands Module
    /// </summary>
    [Group("RiotNA")]
    public class RiotNACommandsModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Riot NA Mastery for a summoner, e.g. !RiotNA Mastery CutterHealer
        /// </summary>
        [Command("Mastery")]
        [Summary("Gets top 10 Champion Masteries of a Summoner")]
        public async Task NAMasteries([Remainder] string summonername)
        {
            string message = await RiotCommandHelper.MasteryAsync(summonername, PlatformRoute.NA1);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Riot NA History for a summoner, e.g. !RiotNA History CutterHealer
        /// </summary>
        [Command("History")]
        [Summary("Get most recent 10 games of a Summoner")]
        public async Task NAsync([Remainder] string summonername)
        {
            string message = await RiotCommandHelper.GetMatchHistoryAsync(summonername, PlatformRoute.NA1, RegionalRoute.EUROPE);

            await ReplyAsync(message);
        }

    }
}
