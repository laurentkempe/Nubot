namespace Nubot.Plugins
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Net.Http;
    using System.Web;
    using Interfaces;
    using Nancy.TinyIoc;
    using Newtonsoft.Json.Linq;

    [Export(typeof(IRobotPlugin))]
    public class YouTube : IRobotPlugin
    {
        private readonly IRobot _robot;

        public YouTube()
        {
            Name = "YouTube";

            _robot = TinyIoCContainer.Current.Resolve<IRobot>();
        }

        public string Name { get; private set; }

        public void Respond(string message)
        {
            _robot.Respond(@"(youtube) (me) (.*)", message, async match =>
            {
                var query = match.Groups[3].Value;

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
                    _robot.Message(string.Format("No video results for {0}", query));
                    return;
                }

                var links = (from video in videos
                    from link in video["link"]
                    where (string)link["rel"] == "alternate" && (string)link["type"] == "text/html"
                    select (string)link["href"]).ToList();

                _robot.Message(links.ElementAt(new Random().Next(0, links.Count())));
            });
        }
    }
}