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
    public class GoogleImages : RobotPluginBase
    {
        [ImportingConstructor]
        public GoogleImages(IRobot robot)
            : base("Google Images", robot)
        {
            HelpMessages = new List<string>
            {
                "image|img me <query> - Queries Google Images for <query> and returns a random top result.",
                "animate me <query> - The same thing as `image me`, except adds a few parameters to try to return an animated GIF instead.",
                "mustache me <url> - Adds a mustache to the specified URL.",
                "mustache me <query> - Searches Google Images for the specified query and mustaches it."
            };

            Robot.Respond(@"(image|img) (me) (.*)", msg => ImageMe(msg.Match[3], url => msg.Send(url)));
            Robot.Respond(@"(animate) (me) (.*)", msg => ImageMe(msg.Match[3], url => msg.Send(url), true));

            Robot.Respond(@"(?:mo?u)?sta(?:s|c)he?(?: me)? (.*)", msg =>
            {
                const string mustachify = "http://mustachify.me/?";
                var imagery = msg.Match[1];

                if (imagery.StartsWith("http"))
                {
                    msg.Send(string.Format("{0}{1}", mustachify, string.Format("src={0}", HttpUtility.UrlEncode(imagery))));
                }
                else
                {
                    ImageMe(imagery, url => msg.Send(string.Format("{0}src={1}", mustachify, HttpUtility.UrlEncode(url))), false, true);
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