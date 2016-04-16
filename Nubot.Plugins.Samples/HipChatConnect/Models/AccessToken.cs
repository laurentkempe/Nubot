namespace Nubot.Plugins.Samples.HipChatConnect.Models
{
    public class AccessToken
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public int group_id { get; set; }
        public string group_name { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
    }
}