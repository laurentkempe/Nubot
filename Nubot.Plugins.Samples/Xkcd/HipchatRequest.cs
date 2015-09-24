namespace Nubot.Plugins.Samples.Xkcd
{
    using System;
    using Newtonsoft.Json;

    public class HipchatRequest
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        public Item Item { get; set; }
        public int Webhook_Id { get; set; }
    }

    public class Item
    {
        public Message message { get; set; }
        public Room room { get; set; }
    }

    public class Message
    {
        public DateTime date { get; set; }
        public From from { get; set; }
        public string id { get; set; }
        public object[] mentions { get; set; }
        public string message { get; set; }
        public string type { get; set; }
    }

    public class From
    {
        public int id { get; set; }
        public string mention_name { get; set; }
        public string name { get; set; }
    }

    public class Room
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}