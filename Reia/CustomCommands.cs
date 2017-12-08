using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Reia
{
    public class CustomCommands
    {
        [Command("anime")]
        public async Task KitsuSearchAnime(CommandContext ctx, params string[] searchTerms)
        {
            if (searchTerms.Length == 0)
            {
                return;
            }

            DiscordEmbed result = await Commands.ModuleApiKitsu.SearchContent("anime", searchTerms);
            if (result == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}\nSorry, No results");
            }
            else
            {
                await ctx.RespondAsync($"Here you go, {ctx.User.Mention}\n", embed: result);
            }
        }

        [Command("manga")]
        public async Task KitsuSearchManga(CommandContext ctx, params string[] searchTerms)
        {
            if (searchTerms.Length == 0)
            {
                return;
            }

            DiscordEmbed result = await Commands.ModuleApiKitsu.SearchContent("manga", searchTerms);
            if (result == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}\nSorry, No results");
            }
            else
            {
                await ctx.RespondAsync($"Here you go, {ctx.User.Mention}\n", embed: result);
            }
        }

        [Command("youtube")]
        public async Task YoutubeSearch(CommandContext ctx, params string[] searchTerms)
        {
            if (searchTerms.Length == 0)
            {
                return;
            }

            string result = await Commands.ModuleApiYoutube.SearchContent(searchTerms);
            if (!String.IsNullOrEmpty(result))
            {
                await ctx.RespondAsync($"Here you go, {ctx.User.Mention}\n{result}");
            }
        }
    }
}
