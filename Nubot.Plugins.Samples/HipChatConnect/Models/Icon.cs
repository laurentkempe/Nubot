namespace Nubot.Plugins.Samples.HipChatConnect.Models
{
    using Newtonsoft.Json;

    public class Icon
    {
        public string url { get; set; }

        [JsonProperty("url@2x")]
        public string url2 { get; set; }
    }
}