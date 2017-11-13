using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;

using TAGGER.Commands;

namespace TAGGER
{
    class Bot
    {
        static DiscordClient discord;
        static CommandsNextModule commands;

        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                discord = new DiscordClient(new DiscordConfiguration
                {
                    Token = args[0],
                    TokenType = TokenType.Bot,
                    UseInternalLogHandler = true,
                    LogLevel = LogLevel.Debug
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
            await discord.ConnectAsync();

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });
            commands.RegisterCommands<CommandHandler>();

            await Task.Delay(-1);
        }
    }
}
