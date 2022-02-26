using CutterBotCS.Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace CutterBotCS.Modules.Simple
{
    /// <summary>
    /// Simple Commands Module
    /// </summary>
    public class SimpleCommandsModule : ModuleBase<SocketCommandContext>
    {
        // ~say hello world -> hello world
        [Command("say")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder]string echo)
            => ReplyAsync(echo);

        // ~say https://tenor.com/view/hiya-anime-cute-smile-hello-gif-16987977
        [Command("Hello")]
        [Summary("Hello from umaru")]
        public Task HelloAsync() => ReplyAsync("https://tenor.com/view/hiya-anime-cute-smile-hello-gif-16987977");

        [Command("dn")]
        [Summary("???")] // Deez Nuts
        public Task DNAsync() => ReplyAsync("https://tenor.com/view/testing-new-deez-nuts-ha-teeth-gif-15758045");
    }
}
