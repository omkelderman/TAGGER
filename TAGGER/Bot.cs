using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using System;

namespace TAGGER
{
    class Bot
    {
        static DiscordClient discord;

        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                discord = new DiscordClient(new DiscordConfiguration
                {
                    Token = args[0],
                    TokenType = TokenType.Bot
                });
                Init().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            else
            {
                Console.WriteLine("Please enter your Token as an argument");
            }
        }

        static async Task Init()
        {
            discord.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith("ping"))
                    await e.Message.RespondAsync("pong!");
            };

            Console.WriteLine("Connected!");

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
