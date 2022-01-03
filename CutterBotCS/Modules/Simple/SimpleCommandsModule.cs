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
        public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
            => ReplyAsync(echo);

        // ~say https://tenor.com/view/hiya-anime-cute-smile-hello-gif-16987977
        [Command("Hello")]
        [Summary("Hello from umaru")]
        public Task SayAsync() => ReplyAsync("https://tenor.com/view/hiya-anime-cute-smile-hello-gif-16987977");

        // ~say Command List for Players
        [Command("Help")]
        [Summary("Show commands for Umaru")]
        public Task HelpAsync() => ReplyAsync(HelpCommands.GetHelpList());

        // ~say Command List for Players
        [Command("Save")]
        [Summary("Save Discord Bot")]
        public Task TaskSaveAsync()
        {
            HelpCommands.Save(DiscordBot.CONFIG_DIR, DiscordBot.CONFIG_NAME);
            return ReplyAsync("Might of saved. Fuck knows.");
        }
    }
}
