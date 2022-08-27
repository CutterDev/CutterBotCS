using CutterBotCS.Discord;
using Discord.Commands;
using System;
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
        public Task HelloAsync() 
        {
            string[] links = new string[]
            {
                "https://tenor.com/view/forrest-gump-hello-wave-hi-waving-gif-22571528",
                "https://tenor.com/view/hello-there-private-from-penguins-of-madagascar-hi-wave-hey-there-gif-16043627",
                "https://giphy.com/gifs/sesamestreet-3pZipqyo1sqHDfJGtz",
                "https://tenor.com/view/gon-gon-freecss-hunterxhunter-hxh-hi-gif-22377842",
                "https://tenor.com/view/hello-cat-waving-trendizisst-hi-gif-21629939",
                "https://tenor.com/view/loveyou-hello-gif-23041108",
                "https://tenor.com/view/quby-chan-hi-wave-hello-hi-there-gif-17010845",
                "https://tenor.com/view/hello-gif-25645320",
                "https://tenor.com/view/jorrparivar-digital-pratik-patient-kachhuaa-jorrparivar-kachhuaa-turtle-gif-24955516",
                "https://tenor.com/view/hello-bob-minions-the-rise-of-gru-hi-greetings-gif-26087717",
                "https://tenor.com/view/hi-hey-hello-there-kitten-cute-gif-16697937",
                "https://tenor.com/view/angels-of-death-edward-mason-eddie-angels-of-death-rachel-gardner-rachel-angels-of-death-gif-25108343",
                "https://tenor.com/view/hello-bear-gif-19757133",
                "https://tenor.com/view/hello-hi-bluebird-disney-pixar-gif-16903826",
                "https://tenor.com/view/hey-there-cute-kid-singing-mic-gif-16113705",
                "https://tenor.com/view/hiya-anime-cute-smile-hello-gif-16987977",
                "https://tenor.com/view/bonjour-party-inglourious-basterds-gif-14867086"
            };

            Random rnd = new Random();
            int linkindex = rnd.Next(links.Length); 

            return ReplyAsync(links[linkindex]);
        }

        [Command("dn")]
        [Summary("???")] // Deez Nuts
        public Task DNAsync() => ReplyAsync("https://tenor.com/view/testing-new-deez-nuts-ha-teeth-gif-15758045");

        [Command ("Lailai")]
        [Summary("Sherveens dumbass thingy")]
        public Task LailaiAsync() => ReplyAsync("https://tenor.com/view/hello-there-private-from-penguins-of-madagascar-hi-wave-hey-there-gif-16043627");
    }
}
