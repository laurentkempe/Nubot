namespace Nubot.Plugins.Samples.Xkcd
{
    using System;
    using System.ComponentModel.Composition;
    using System.Net;
    using System.Threading.Tasks;
    using Abstractions;
    using Nancy.ModelBinding;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using HttpStatusCode = Nancy.HttpStatusCode;


    [Export(typeof(IRobotPlugin))]
    public class XkcdListener : HttpPluginBase
    {
        [ImportingConstructor]
        public XkcdListener(IRobot robot)
            : base("Xkcd", "/xkcd", robot)
        {
            Post["/", runAsync: true] = async (_, ct) =>
            {
                var request = this.Bind<HipchatRequest>(new BindingConfig { IgnoreErrors = true, BodyOnly = true, Overwrite = true });

                if (request.Item.message.message != "/xkcd") return HttpStatusCode.BadRequest;

                var xkcdImgUrl = await GetRandomComic();

                var response = new
                {
                    color = "green",
                    message = xkcdImgUrl,
                    notify = false,
                    message_format = "text"
                };

                return JsonConvert.SerializeObject(response);
            };
        }

        private static async Task<string> GetRandomComic()
        {
            using (var wc = new WebClient())
            {
                var data = await wc.DownloadStringTaskAsync("https://xkcd.com/info.0.json");
                dynamic result = JObject.Parse(data);
                int latestComic = result.num;

                var randomNum = new Random().Next(1, latestComic + 1);

                var comicData = await wc.DownloadStringTaskAsync($"https://xkcd.com/{randomNum}/info.0.json");
                dynamic comicResult = JObject.Parse(comicData);

                return comicResult.img;
            }
        }
    }
}