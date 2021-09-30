using Discord.Commands;
using System.Threading.Tasks;

namespace CutterBotCS
{
    /// <summary>
    /// Info Module
    /// </summary>
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("Hello")]
        [Summary("Echoes a Message")]
        public Task HelloAsync([Remainder] [Summary("The text to echo")] string echo)
        {
           return ReplyAsync(echo);
        }
    }
}
