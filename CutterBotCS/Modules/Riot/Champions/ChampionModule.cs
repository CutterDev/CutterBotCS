using CutterDragon;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace CutterBotCS.Modules.Riot.Champions
{
    /// <summary>
    /// Champion Module
    /// </summary>
    public class ChampionModule : ModuleBase<SocketCommandContext>
    {
        private ChampionCommandHelper m_ChampionHelper { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ChampionModule(CutterDragonWorker cdw)
        {
            m_ChampionHelper = new ChampionCommandHelper(cdw);
        }

        /// <summary>
        /// Get Information on a Champion
        /// </summary>
        [Command("champinfo")]
        [Summary("Get Champion Info")]
        public async Task ChampionInfoAsync([Remainder] string champion)
        {
            EmbedBuilder builder = m_ChampionHelper.GetChampionInfo(champion);

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}
