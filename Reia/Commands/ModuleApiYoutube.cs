using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Reia.Commands
{
    #region Start Youtube Search Result deserializer classes
    public class YoutubeSearchResult
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public string NextPageToken { get; set; }
        public PageInfoField PageInfo { get; set; }
        public List<Item> Items { get; set; }
    }

    public class PageInfoField
    {
        public string TotalResults { get; set; }
        public string ResultsPerPage { get; set; }
    }
    
    public class Item
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public IdField Id { get; set; }
        public SnippetField Snippet { get; set; }

    }

    public class SnippetField
    {
        public string PublishedAt { get; set; }
        public string ChannelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ThumbnailField Thumbnails { get; set; }
        public string ChannelTitle { get; set; }
        public string LiveBroadcastContent { get; set; }
    }

    public class IdField
    {
        public string Kind { get; set; }
        public string VideoId { get; set; }
    }

    public class ThumbnailField
    {
        public ThumbnailInfo Default { get; set; }
        public ThumbnailInfo Medium { get; set; }
        public ThumbnailInfo High { get; set; }
    }

    public class ThumbnailInfo
    {
        public string Url { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
    }
    #endregion End Youtube Search Result deserializer classes

    public static class ModuleApiYoutube
    {
        private const string URL_BASE = "https://www.googleapis.com/youtube/v3/";
        private const string YOUTUBE_VIDEO_URL_BASE = "https://www.youtube.com/watch";
        private static string _apiKey = String.Empty;


        public static void Init(string apiKey)
        {
            _apiKey = apiKey;
        }

        public static async Task<string> GetContentId(params string[] searchTerm)
        {
            HttpResponseMessage response;
            // Very crude...
            string queryUrl = URL_BASE + "search?part=snippet&maxResults=1&safeSearch=moderate&key=" + _apiKey +
                "&type=video&q=" + System.Net.WebUtility.UrlEncode(String.Join(" ", searchTerm));
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, queryUrl);
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            response = await Reia.Program.Client.SendAsync(httpRequestMessage);
            if (!response.IsSuccessStatusCode)
            {
                return String.Empty;
            }

                // Try Statement here someday...
            var youtubeSearchResult = await response.Content.ReadAsAsync<YoutubeSearchResult>();
            return youtubeSearchResult.Items[0].Id.VideoId;
        }

        public static async Task<string> SearchContent(params string[] searchTerm)
        {
            string s = await GetContentId(searchTerm);

            if (s == String.Empty)
            {
                return null;
            }
            return $"{YOUTUBE_VIDEO_URL_BASE}?v={s}";
        }
    }
}
