namespace Nubot.Plugins.Samples.HipChatConnect
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IdentityModel.Tokens;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.ServiceModel.Security.Tokens;
    using System.Text;
    using System.Threading.Tasks;
    using Abstractions;
    using Models;
    using Nancy;
    using Nancy.ModelBinding;
    using Newtonsoft.Json;
    using HttpStatusCode = Nancy.HttpStatusCode;

    [Export(typeof (IRobotPlugin))]
    public class HipChatConnect : HttpPluginBase
    {
        [ImportingConstructor]
        public HipChatConnect(IRobot robot)
            : base("HipChat Connect", "/hipchat", robot)
        {
            //See HipChat Connect https://developer.atlassian.com/hipchat/tutorials/building-an-add-on-with-your-own-technology-stackdocumentation
            // Glance queryUrl
            // When a user opens a room where your add - on is installed in the HipChat App, the HipChat App retrieves the initial glance
            // value by calling the queryUrl endpoint.This is a cross domain HTTP request, so you need to include CORS headers.
            After.AddItemToEndOfPipeline(ctx => ctx.Response
                        .WithHeader("Access-Control-Allow-Origin", "*")
                        .WithHeader("Access-Control-Allow-Methods", "GET")
                        .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type"));

            // Capabilities Descriptor
            Get["atlassian-connect.json", runAsync: true] = async (_, ct) =>
            {
                //var baseUri = Context.Request.Url.SiteBase;
                var baseUri = "https://8b053996.ngrok.io";

                return await Task.Run(() => GetCapabilitiesDescriptor(baseUri));
            };

            Post["installable", runAsync: true] = async (_, ctx) =>
            {
                var installationData = await GetInstallationData();

                var capabilitiesRoot = await GetCapabilitiesRoot(installationData);

                var accessToken = await GetAccessToken(installationData, capabilitiesRoot);

                await InstallationNotifyRoom(installationData, accessToken);

                return HttpStatusCode.OK;
            };

            Get["glance", runAsync: true] = async (_, ct) =>
            {
                var jwt = Request.Query["signed_request"];

                if (await ValidateToken(jwt))
                {
                    return await Task.Run(() => BuildInitialGlance());
                }

                return Task.FromResult(HttpStatusCode.Unauthorized);
            };
        }

        private static string GetCapabilitiesDescriptor(string baseUri)
        {
            var capabilitiesDescriptor = new
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

            return JsonConvert.SerializeObject(capabilitiesDescriptor);
        }

        private static string BuildInitialGlance()
        {
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

            var serializeObject = JsonConvert.SerializeObject(response);
            return serializeObject;
        }

        private async Task<bool> ValidateToken(dynamic jwt)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var readToken = jwtSecurityTokenHandler.ReadToken(jwt);

            var installationData = await Robot.Brain.GetAsync<InstallationData>(readToken.Issuer);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = installationData.oauthId,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningToken = new BinarySecretSecurityToken(Encoding.UTF8.GetBytes(installationData.oauthSecret))
            };

            try
            {
                SecurityToken token;
                var validatedToken = jwtSecurityTokenHandler.ValidateToken(jwt, validationParameters, out token);
                return validatedToken != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static async Task InstallationNotifyRoom(InstallationData installationData, AccessToken accessToken)
        {
            var notification = new
            {
                message = " ",
                card = new
                {
                    style = "link",
                    url = "http://laurentkempe.com",
                    id = "fee4d9a3-685d-4cbd-abaa-c8850d9b1960",
                    title = "First Integration of Nubot by Laurent Kempe",
                    description = new
                    {
                        format = "html",
                        value =
                            "<b>Add-on link:</b> <a href='#' data-target='hip-connect-tester:hctester.dialog.simple' data-target-options='{\"options\":{\"title\":\"Custom Title\"}, \"parameters\":{\"from\":\"link\"}}'>Open Dialog with parameters</a>"
                    },
                    icon =
                        new {url = "http://icons.iconarchive.com/icons/designbolts/hand-stitched/24/RSS-icon.png"},
                    date = 1443057955792
                }
            };

            var notificationClient = new HttpClient();
            notificationClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var stringContent = new StringContent(JsonConvert.SerializeObject(notification).ToString(),
                Encoding.UTF8, "application/json");
            var requestUri =
                new Uri(
                    $"https://api.hipchat.com/v2/room/{installationData.roomId}/notification?auth_token={accessToken.access_token}");

            var httpResponseMessage = await notificationClient.PostAsync(requestUri, stringContent);
            httpResponseMessage.EnsureSuccessStatusCode();
        }

        private async Task<AccessToken> GetAccessToken(InstallationData installationData, CapabilitiesRoot capabilitiesRoot)
        {
            var client = new HttpClient();

            var dataContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", "send_notification")
            });

            var credentials = Encoding.ASCII.GetBytes($"{installationData.oauthId}:{installationData.oauthSecret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(credentials));

            var tokenResponse =
                await client.PostAsync(new Uri(capabilitiesRoot.capabilities.oauth2Provider.tokenUrl), dataContent);
            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var accessToken = await Task.Run(() => JsonConvert.DeserializeObject<AccessToken>(tokenContent));

            await Robot.Brain.SetAsync("accessToken", accessToken);

            return accessToken;
        }

        private async Task<CapabilitiesRoot> GetCapabilitiesRoot(InstallationData installationData)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(new Uri(installationData.capabilitiesUrl));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var capabilitiesRoot = await Task.Run(() => JsonConvert.DeserializeObject<CapabilitiesRoot>(content));

            await Robot.Brain.SetAsync("capabilitiesRoot", capabilitiesRoot);

            return capabilitiesRoot;
        }

        private async Task<InstallationData> GetInstallationData()
        {
            var installationData = this.Bind<InstallationData>();

            await Robot.Brain.SetAsync(installationData.oauthId, installationData);

            return installationData;
        }
    }
}