namespace Nubot.Interfaces.Message
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class Notification
    {
        public string Room { set; get; }
        public string AuthToken { set; get; }
        public string HtmlMessage { set; get; }
        public bool? Notify { set; get; }
    }
}
