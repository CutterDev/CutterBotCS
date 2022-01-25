using Camille.Enums;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace CutterBotCS.Modules.Riot
{
    public class RiotRegistryCommandsModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Register EUW Account to a Discord Player
        /// </summary>
        [Command("registereuw")]
        [Summary("Register to leaderboard for Ranked Solo")]
        public async Task RegisterPlayerEUW([Remainder] string name)
        {
            string message = await RiotCommandHelper.RegisterPlayerAsync(name, RegionalRoute.EUROPE, PlatformRoute.EUW1, Context.User.Id);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Register NA Account to a Discord Player
        /// </summary>
        [Command("registerna")]
        [Summary("Register to leaderboard for Ranked Solo")]
        public async Task RegisterPlayerNA([Remainder] string name)
        {
            string message = await RiotCommandHelper.RegisterPlayerAsync(name, RegionalRoute.AMERICAS, PlatformRoute.NA1, Context.User.Id);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Register EUNE Account to a Discord Player
        /// </summary>
        [Command("registereune")]
        [Summary("Register to leaderboard for Ranked Solo")]
        public async Task RegisterPlayerEUNE([Remainder] string name)
        {
            string message = await RiotCommandHelper.RegisterPlayerAsync(name, RegionalRoute.EUROPE, PlatformRoute.EUN1, Context.User.Id);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Add Register Discord Player EUW Manually to leaderboards
        /// </summary>
        [Command("playeraddeuw")]
        [Summary("Register Player to Leaderboard with their id also")]
        public async Task PlayerAddEUW(ulong id, [Remainder] string name)
        {
            string message = await RiotCommandHelper.SuperUserRegister(name, RegionalRoute.EUROPE, PlatformRoute.EUW1, id, Context.User.Id);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Add Register Discord Player EUW Manually to leaderboards
        /// </summary>
        [Command("playeraddeune")]
        [Summary("Register Player to Leaderboard with their id also")]
        public async Task PlayerAddEUNE(ulong id, [Remainder] string name)
        {
            string message = await RiotCommandHelper.SuperUserRegister(name, RegionalRoute.EUROPE, PlatformRoute.EUN1, id, Context.User.Id);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Add Register Discord Player EUW Manually to leaderboards
        /// </summary>
        [Command("playeraddna")]
        [Summary("Register Player to Leaderboard with their id also")]
        public async Task PlayerAddNA(ulong id, [Remainder] string name)
        {
            string message = await RiotCommandHelper.SuperUserRegister(name, RegionalRoute.AMERICAS, PlatformRoute.NA1, id, Context.User.Id);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Remove Player
        /// </summary>
        [Command("remove")]
        [Summary("Remove User who called the command's Summoner.")]
        public async Task RemovePlayer()
        {
            string message = RiotCommandHelper.RemovePlayer(Context.User.Id);

            await ReplyAsync(message);
        }

        /// <summary>
        /// Remove Player
        /// </summary>
        [Command("remove")]
        [Summary("Remove User who called the command's Summoner.")]
        public async Task RemovePlayer(ulong id)
        {
            string message = RiotCommandHelper.SuperUserRemovePlayer(id, Context.User.Id);

            await ReplyAsync(message);
        }
    }
}
