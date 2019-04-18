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
        [Description("Search for an Anime")]
        public async Task KitsuSearchAnime(CommandContext ctx, params string[] searchTerms)
        {
            if (searchTerms.Length == 0)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}\nOwO Where are your search terms?");
                return;
            }

            DiscordEmbed result = await Commands.ModuleApiKitsu.SearchContent("anime", searchTerms);
            if (result == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}\nSorry UwU, No results");
            }
            else
            {
                await ctx.RespondAsync($"Here you go, {ctx.User.Mention}\n", embed: result);
            }
        }

        [Command("manga")]
        [Description("Search for an Manga")]
        public async Task KitsuSearchManga(CommandContext ctx, params string[] searchTerms)
        {
            if (searchTerms.Length == 0)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}\nOwO Where are your search terms?");
                return;
            }

            DiscordEmbed result = await Commands.ModuleApiKitsu.SearchContent("manga", searchTerms);
            if (result == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}\nSorry UwU, No results");
            }
            else
            {
                await ctx.RespondAsync($"Here you go, {ctx.User.Mention}\n", embed: result);
            }
        }

        [Command("youtube")]
        [Description("Search for a Youtube video")]
        [Aliases("yt")]
        public async Task YoutubeSearch(CommandContext ctx, params string[] searchTerms)
        {
            if (searchTerms.Length == 0)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}\nWhere are your search terms?");
                return;
            }

            string result = await Commands.ModuleApiYoutube.SearchContent(searchTerms);
            if (!String.IsNullOrEmpty(result))
            {
                await ctx.RespondAsync($"Here you go, {ctx.User.Mention}\n{result}");
            }
        }

        [Command("avatar")]
        [Description("Gets the URL of a user's avatar")]
        [Aliases("av")]
        public async Task GetAvatar(CommandContext ctx, DiscordUser usr)
        {
            await ctx.RespondAsync($"{usr.AvatarUrl}");
        }

        [Command("image")]
        public async Task ImgurSearch(CommandContext ctx, params string[] searchTerms)
        {
            await ctx.RespondAsync("Whoop");
        }

        [Command("roll")]
        [Description("Return a random integer from a range")]
        [Aliases("rr")]
        public async Task RollNumber(CommandContext ctx, params int[] param)
        {
            if(param.Length == 0)
            {
                await ctx.RespondAsync("OwO Whats the number range you want to roll");
            }

            // Make sure there are 1 or 2 parameters only
            if (!(param.Length == 1 || param.Length == 2))
            {
                await ctx.RespondAsync("Takes only 1 or 2 numbers u fool");
                return;
            }

            int min = 1;
            int max = 1;
            if (param.Length == 2)
            {
                max = param[1];
                min = param[0];
                if (param[1] < 0)
                {
                    await ctx.RespondAsync("ʕ •̀ o •́ ʔ NO NEGATIVE NUMBERS");
                    return;
                }
            }
            else if (param.Length == 1)
            {
                max = param[0];
                min = 1;

            }

            if (param[0] < 0)
            {
                await ctx.RespondAsync("ʕ •̀ o •́ ʔ NO NEGATIVE NUMBERS");
                return;
            }
            Random rand = new Random();
            await ctx.RespondAsync($"{ctx.User.Mention} rolled a number between {min} and {max} and got...\n🎲 {rand.Next(min, max)} 🎲");
        }
    }
}
