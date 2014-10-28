namespace Nubot.Abstractions
{
    using System;

    public class Notification
    {
        public string Room { set; get; }
        public string AuthToken { set; get; }
        public string HtmlMessage { set; get; }
        public bool? Notify { set; get; }
    }
}
