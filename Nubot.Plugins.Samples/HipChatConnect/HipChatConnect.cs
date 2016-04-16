namespace Nubot.Plugins.Samples.HipChatConnect
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Abstractions;
    using Models;
    using Nancy.ModelBinding;
    using Newtonsoft.Json;
    using HttpStatusCode = Nancy.HttpStatusCode;

    [Export(typeof (IRobotPlugin))]
    public class HipChatConnect : HttpPluginBase
    {
        private const string BaseUri = "https://27be5628.ngrok.io";

        [ImportingConstructor]
        public HipChatConnect(IRobot robot)
            : base("HipChat Connect", "/hipchat", robot)
        {
            //After.AddItemToEndOfPipeline((ctx) => ctx.Response
            //            .WithHeader("Access-Control-Allow-Origin", "*")
            //            .WithHeader("Access-Control-Allow-Methods", "GET")
            //            .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type"));

            //After.AddItemToEndOfPipeline(x =>
            //    x.Response.WithHeaders("Access-Control-Allow-Origin", "*")
            //        .WithHeaders("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept"));

            Get["atlassian-connect.json", runAsync: true] = async (_, ct) =>
            {
                var baseUri = BaseUri;
                //var baseUri = Context.Request.Url.SiteBase;

                var response = new
                {
                    name = "Nubot",
                    description = "An add-on to talk to Nubot.",
                    key = "nubot-addon",
                    links = new
                    {
                        self = $"{baseUri}/hipchat/atlassian-connect.json",
                        homepage = $"{baseUri}/hipchat/atlassian-connect.json"
                    },
                    vendor = new
                    {
                        name = "Laurent Kempe",
                        url = "http://laurentkempe.com"
                    },
                    capabilities = new
                    {
                        hipchatApiConsumer = new
                        {
                            scopes = new[]
                            {
                                "send_notification",
                                "view_room"
                            }
                        },
                        installable = new
                        {
                            callbackUrl = $"{baseUri}/hipchat/installable"
                        },
                        glance = new[]
                        {
                            new
                            {
                                name = new
                                {
                                    value = "Hello TC"
                                },
                                queryUrl = $"{baseUri}/hipchat/glance",
                                key = "nubot.glance",
                                target = "nubot.sidebar",
                                icon = new Icon
                                {
                                    url = $"{baseUri}/nubot/css/TC.png",
                                    url2 = $"{baseUri}/nubot/css/TC2.png"
                                }
                            }
                        }
                    }
                };

                return await Task.Run(() => JsonConvert.SerializeObject(response));
            };

            Post["installable", runAsync: true] = async (_, ctx) =>
            {
                var root = this.Bind<Root>();

                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(new Uri(root.capabilitiesUrl));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var capabilitiesRoot = await Task.Run(() => JsonConvert.DeserializeObject<CapabilitiesRoot>(content));

                //var client = new HttpClient(new HttpClientHandler {Credentials = new NetworkCredential(root.oauthId, root.oauthSecret)});

                var client = new HttpClient();

                var dataContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("scope", "send_notification")
                });

                var credentials = Encoding.ASCII.GetBytes($"{root.oauthId}:{root.oauthSecret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));

                var tokenResponse =
                    await client.PostAsync(new Uri(capabilitiesRoot.capabilities.oauth2Provider.tokenUrl), dataContent);
                var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
                var accessToken = await Task.Run(() => JsonConvert.DeserializeObject<AccessToken>(tokenContent));

                var notificationClient = new HttpClient();

                var notification = new
                {
                    message = " ",
                    card = new
                    {
                        style = "link",
                        url = "http://www.laurentkempe.com",
                        id = "fee4d9a3-685d-4cbd-abaa-c8850d9b1960",
                        title = "First Integration of Nubot by Laurent Kempé",
                        description = new
                        {
                            format = "html",
                            value =
                                "<b>Add-on link:</b> <a href='#' data-target='hip-connect-tester:hctester.dialog.simple' data-target-options='{\"options\":{\"title\":\"Custom Title\"}, \"parameters\":{\"from\":\"link\"}}'>Open Dialog with parameters</a>"
                        },
                        icon =
                            new { url = "http://icons.iconarchive.com/icons/designbolts/hand-stitched/24/RSS-icon.png" },
                        date = 1443057955792
                    }
                };

                var client2 = new HttpClient();
                client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var stringContent = new StringContent(JsonConvert.SerializeObject(notification).ToString(),
                    Encoding.UTF8, "application/json");
                var requestUri =
                    new Uri(
                        $"https://api.hipchat.com/v2/room/{root.roomId}/notification?auth_token={accessToken.access_token}");

                var httpResponseMessage = await client2.PostAsync(requestUri, stringContent);
                httpResponseMessage.EnsureSuccessStatusCode();

                //var t = this;

                return HttpStatusCode.OK;
            };

            Get["glance", runAsync: true] = async (_, ct) =>
            {
                var jwt = Request.Query["signed_request"];

                var response = new
                {
                    label = new
                    {
                        type = "html",
                        value = "<b>4</b> Builds"
                    },
                    status = new
                    {
                        type = "lozenge",
                        value = new
                        {
                            label = "GOOD",
                            type = "success"
                        }
                    },
                    metadata = new
                    {
                        customData = new {customAttr = "customValue"}
                    }
                };

                return await Task.Run(() => JsonConvert.SerializeObject(response));
            };
        }
    }
}