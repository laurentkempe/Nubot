namespace Nubot.Plugins.Samples
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Net.Http;
    using Abstractions;
    using Nancy.Helpers;
    using Newtonsoft.Json.Linq;

    [Export(typeof(IRobotPlugin))]
    public class YouTube : RobotPluginBase
    {
        [ImportingConstructor]
        public YouTube(IRobot robot)
            : base("YouTube", robot)
        {
            HelpMessages = new List<string> {
                "youtube me <query> - Queries YouTube and returns a random video from the top 15 videos found"};

            Robot.Respond(@"(youtube) (me) (.*)", async msg =>
            {
                var query = msg.Match[3];

                var queryString = HttpUtility.ParseQueryString("");
                queryString["orderBy"] = "relevance";
                queryString["max-results"] = "15";
                queryString["alt"] = "json";
                queryString["q"] = query;

                var uriBuilder = new UriBuilder("http://gdata.youtube.com/feeds/api/videos") { Query = queryString.ToString() };

                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(uriBuilder.Uri);
                if (!response.IsSuccessStatusCode) return;

                var responseBody = await response.Content.ReadAsStringAsync();

                var jObject = JObject.Parse(responseBody);
                var videos = jObject["feed"]["entry"];

                if (videos == null)
                {
                    msg.Send(string.Format("No video results for {0}", query));
                    return;
                }

                var links = (from video in videos
                             from link in video["link"]
                             where (string)link["rel"] == "alternate" && (string)link["type"] == "text/html"
                             select (string)link["href"]).ToList();

                msg.Send(links.ElementAt(new Random().Next(0, links.Count())));
            });

        }
    }
}