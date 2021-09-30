using Discord.Commands;
using System.Threading.Tasks;

namespace CutterBotCS.Modules
{
    /// <summary>
    /// Simple Commands Module
    /// </summary>
    public class SimpleCommandsModule : ModuleBase<SocketCommandContext>
    {
        // ~say hello world -> hello world
        [Command("say")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
            => ReplyAsync(echo);
    }
}
