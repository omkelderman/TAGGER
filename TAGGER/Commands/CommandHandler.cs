using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace TAGGER.Commands
{
    class CommandHandler
    {
        [Command("nsfw")]
        public async Task NSFW(CommandContext ctx, string argument = "")
        {
            var role = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "NSFW");
            if (argument != "add" && argument != "remove")
            {
                await ctx.RespondAsync(ctx.Member.Mention + " please use either `!nsfw add` or `!nsfw remove` to recieve/remove the NSFW role.");
            }
            else if (argument == "add")
            {
                await TAGGER.Commands.NSFW.Add(ctx, role);
            }
            else if (argument == "remove")
            {
                await TAGGER.Commands.NSFW.Remove(ctx, role);
            }
        }

        [Command("calculate")]
        public async Task Calculate(CommandContext ctx, string argument = "", int tag = 4)
        {
            if (argument == "")
            {
                await ctx.RespondAsync(ctx.Member.Mention + " please use `!calculate {beatmap link} [player count (default = 4)]` to calculate the beatmap star rating in TAG mode.");
            }
            else
            {
                await TAGGER.Commands.Calculate.SR(ctx, argument, tag);
            }
        }
    }
}
