namespace Nubot.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Net.Http;
    using System.Web;
    using Interfaces;
    using Nancy.TinyIoc;
    using Newtonsoft.Json.Linq;

    [Export(typeof(IRobotPlugin))]
    public class GoogleImages : IRobotPlugin
    {
        private readonly IRobot _robot;
        public GoogleImages()
        {
            Name = "Google Images";

            _robot = TinyIoCContainer.Current.Resolve<IRobot>();
        }

        public string Name { get; private set; }
        public IEnumerable<string> HelpMessages { get; private set; }

        public void Respond(string message)
        {
            _robot.Respond(@"(image|img) (me) (.*)", message, match => ImageMe(match.Groups[3].Value, url => _robot.Message(url)));
            _robot.Respond(@"(animate) (me) (.*)", message, match => ImageMe(match.Groups[3].Value, url => _robot.Message(url), true));
            _robot.Respond(@"(?:mo?u)?sta(?:s|c)he?(?: me)? (.*)", message,
                match =>
                {
                    const string mustachify = "http://mustachify.me/?";
                    var imagery = match.Groups[1].Value;

                    if (imagery.StartsWith("http"))
                    {
                        _robot.Message(string.Format("{0}{1}", mustachify, string.Format("src={0}", HttpUtility.UrlEncode(imagery))));
                    }
                    else
                    {
                        ImageMe(imagery, url => _robot.Message(string.Format("{0}src={1}", mustachify, HttpUtility.UrlEncode(url))), false, true);
                    }
                });
        }

        private static async void ImageMe(string query, Action<string> action, bool animated = false, bool faces = false)
        {
            var queryString = HttpUtility.ParseQueryString("");
            queryString["v"] = "1.0";
            queryString["rsz"] = "8";
            queryString["q"] = query;
            queryString["safe"] = "active";
            if (animated) queryString["imgtype"] = "animated";
            if (faces) queryString["imgtype"] = "face";
            var uriBuilder = new UriBuilder("http://ajax.googleapis.com/ajax/services/search/images") { Query = queryString.ToString() };

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(uriBuilder.Uri);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var jObject = JObject.Parse(responseBody);
                var images = jObject["responseData"]["results"];

                if (images.Any())
                {
                    action(string.Format("{0}", images.ElementAt(new Random().Next(0, images.Count()))["unescapedUrl"]));
                }
            }
        }
    }
}