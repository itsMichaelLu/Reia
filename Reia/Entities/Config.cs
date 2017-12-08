using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Reia.Entities
{
    public class Config
    {
        [JsonProperty("botToken")]
        internal string BotToken { get; set; } = "Insert your bot Token";

        [JsonProperty("botPrefix")]
        internal string BotPrefix { get; set; } = "Bot prefix";

        /* Not Used...
        [JsonProperty("apiKitsuUser")]
        internal string ApiKitsuUser { get; set; } = "kitsu username";
        */

        [JsonProperty("apiYoutubeKey")]
        internal string ApiYoutubeKey { get; set; } = "yt api key";
    }

    internal class ConfigReader
    {
        public static Config LoadFromFile(string path)
        {
            using (var sr = new StreamReader(path))
            {
                return JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
            }
        }

        public void SaveToFile(string path)
        {
            using (var sw = new StreamWriter(path))
            {
                sw.Write(JsonConvert.SerializeObject(new Config(), Formatting.Indented));
            }
        }

    }
}
