using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Reia.Commands
{
    public static class ModuleApiKitsu
    {
        private const string API_URL_BASE = "https://kitsu.io/api/edge/";
        private const string ANIME_URL_BASE = "http://kitsu.io/anime/";
        private const string MANGA_URL_BASE = "http://kitsu.io/manga/";
        private const int NUM_RESULTS = 5;

        public static void Init()
        {
            // TODO Set URLs in here, instead of hard-coded
        }

        public static List<string> GetCategories(JObject categories)
        {
            List<string> s = new List<string>();
            int numCats = categories["data"].ToObject<List<object>>().Count;
            for (int i = 0; i < numCats; i++)
            {
                s.Add(categories["data"][i]["attributes"]["title"].ToString());
            }
            return s;
        }

        public static List<string> GetOtherSearchResults(JObject searchResults)
        {
            List<string> s = new List<string>();
            
            int numResults = searchResults["data"].ToObject<List<object>>().Count;
            int i = 0;

            if (numResults <= 1)
            {
                return null;
            }

            while (i <= NUM_RESULTS - 1 && i < numResults - 1)
            {
                s.Add(searchResults["data"][i + 1]["attributes"]["canonicalTitle"].ToString());
                i++;
            }
            return s;
        }

        public static async Task<DiscordEmbed> DecodeAnimeSearchResults(JObject searchResults)
        {
            JToken entry = searchResults["data"][0];
            JToken attr = entry["attributes"];
            List<string> aka;
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = attr["canonicalTitle"].ToString(),
                Description = attr["synopsis"].ToString(),
                Color = new DiscordColor(0x00, 0x80, 0x80),
                ImageUrl = attr["posterImage"]["large"].ToString(),
                Timestamp = DateTimeOffset.Now
            };

            if (!String.IsNullOrEmpty(attr["showType"].ToString()))
            {
                embed.AddField("Media", attr["showType"].ToString(), inline: true);
            }

            if (!String.IsNullOrEmpty(attr["episodeCount"].ToString()))
            {
                embed.AddField("Episodes", attr["episodeCount"].ToString(), inline: true);
            }
            // Needs to be formatted with title casing
            embed.AddField("Status", System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(attr["status"].ToString()), inline: true);

            if (attr.ToObject<Dictionary<string, object>>().ContainsKey("averageRating"))
            {
                if (!String.IsNullOrEmpty(attr["averageRating"].ToString()))
                {
                    embed.AddField("Rating", $"{attr["averageRating"].ToString()}/100", inline: true);
                }
            }

            DateTime startDate = DateTime.Parse(attr["startDate"].ToString());            
            if (String.IsNullOrEmpty(attr["endDate"].ToString()) || (attr["startDate"].ToString() == attr["endDate"].ToString()))
            {
                embed.AddField("Aired", $"{startDate.ToString("d MMMM yyyy")}", inline: true);
            }
            else
            {
                DateTime endDate = DateTime.Parse(attr["endDate"].ToString());
                embed.AddField("Aired", $"{startDate.ToString("d MMMM yyyy")} to {endDate.ToString("d MMMM yyyy")}", inline: true);
            }

            aka = new List<string>();
            foreach (var temp in new List<string>(new string[] { "en", "en_jp", "ja_jp" }))
            {
                if (attr["titles"].ToObject<Dictionary<string, object>>().ContainsKey(temp))
                {
                    if (!String.IsNullOrEmpty(attr["titles"][temp].ToString()))
                    {
                        aka.Add(attr["titles"][temp].ToString());
                    }
                }
            }

            if (aka.Count > 0)
            {
                embed.AddField("Also Known As", String.Join("; ", aka));
            }

            var response = await Reia.Program.Client.GetStringAsync(entry["relationships"]["categories"]["links"]["related"].ToString());
            embed.AddField("Tags", String.Join(", ", GetCategories(JObject.Parse(response))));

            var otherSearchResults = GetOtherSearchResults(searchResults);
            if (otherSearchResults != null)
            {
                embed.AddField("Related Searches", String.Join("\n", otherSearchResults));
            }

            embed.AddField("Links", ANIME_URL_BASE + attr["slug"].ToString());

            return embed;

        }

        public static async Task<DiscordEmbed> DecodeMangaSearchResults(JObject searchResults)
        {
            JToken entry = searchResults["data"][0];
            JToken attr = entry["attributes"];
            List<string> aka;
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = attr["canonicalTitle"].ToString(),
                Description = attr["synopsis"].ToString(),
                Color = new DiscordColor(0x00, 0x80, 0x80),
                ImageUrl = attr["posterImage"]["large"].ToString(),
                Timestamp = DateTimeOffset.Now
            };
            if (!String.IsNullOrEmpty(attr["mangaType"].ToString()))
            {
                embed.AddField("Media", attr["mangaType"].ToString(), inline: true);
            }

            if (!String.IsNullOrEmpty(attr["chapterCount"].ToString()))
            {
                embed.AddField("Chapters", attr["chapterCount"].ToString(), inline: true);
            }
            
            if (!String.IsNullOrEmpty(attr["volumeCount"].ToString()))
            {
                embed.AddField("Volumes", attr["volumeCount"].ToString(), inline: true);
            }

            // Need to add status field, with string formatted to title casing
            embed.AddField("Status", System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(attr["status"].ToString()), inline: true);

            DateTime startDate = DateTime.Parse(attr["startDate"].ToString());

            if (String.IsNullOrEmpty(attr["endDate"].ToString()) || (attr["startDate"].ToString() == attr["endDate"].ToString()))
            {
                embed.AddField("Date", $"{startDate.ToString("d MMMM yyyy")}", inline: true);
            }
            else
            {
                DateTime endDate = DateTime.Parse(attr["endDate"].ToString());
                embed.AddField("Date", $"{startDate.ToString("d MMMM yyyy")} to {endDate.ToString("d MMMM yyyy")}", inline: true);
            }

            if (attr.ToObject<Dictionary<string, object>>().ContainsKey("averageRating"))
            {
                if (!String.IsNullOrEmpty(attr["averageRating"].ToString()))
                {
                    embed.AddField("Rating", $"{attr["averageRating"].ToString()}/100", inline: true);
                }
            }

            aka = new List<string>();

            foreach (var temp in new List<string>(new string[] { "en", "en_jp", "ja_jp" }))
            {
                if (attr["titles"].ToObject<Dictionary<string, object>>().ContainsKey(temp))
                {
                    if (!String.IsNullOrEmpty(attr["titles"][temp].ToString()))
                    {
                        aka.Add(attr["titles"][temp].ToString());
                    }
                }
            }

            if (aka.Count > 0)
            {
                embed.AddField("Also Known As", String.Join("; ", aka));
            }

            var response = await Reia.Program.Client.GetStringAsync(entry["relationships"]["categories"]["links"]["related"].ToString());
            embed.AddField("Tags", String.Join(", ", GetCategories(JObject.Parse(response))));

            var otherSearchResults = GetOtherSearchResults(searchResults);
            if (otherSearchResults != null)
            {
                embed.AddField("Related Searches", String.Join("\n", otherSearchResults));
            }

            embed.AddField("Links", MANGA_URL_BASE + attr["slug"].ToString());

            return embed;
        }

        public static async Task<DiscordEmbed> SearchContent(string type, params string[] searchTerm)
        {
            HttpResponseMessage response;
            HttpRequestMessage httpRequestMessage;
            JObject kitsuSearchResults;
            if (type.ToLower() != "manga" && type.ToLower() != "anime")
            {
                return null;
            }

            string queryUrl = API_URL_BASE + $"{type}?";
            queryUrl += $"page%5blimit%5d={NUM_RESULTS + 1}";
            queryUrl += $"&filter%5btext%5d={System.Net.WebUtility.UrlEncode(String.Join(" ", searchTerm))}";

            httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, queryUrl);
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));
            
            response = await Reia.Program.Client.SendAsync(httpRequestMessage);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return null;
            }

            // Try Statement here someday...
            var s = await response.Content.ReadAsStringAsync();
            kitsuSearchResults = JObject.Parse(s);
            if (((JArray)kitsuSearchResults["data"]).Count <= 0)
            {
                return null;
            }

            if (type.ToLower() == "manga")
            {
                return await DecodeMangaSearchResults(kitsuSearchResults);
            }
            else if (type.ToLower() == "anime")
            {
                return await DecodeAnimeSearchResults(kitsuSearchResults);
            }
            else
            {
                Console.WriteLine(response);
                return null;
            }

        }
    }
}
