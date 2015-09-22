namespace Nubot.Plugins.Samples.TeamCity
{
    #region

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using Abstractions;
    using Github.Models;

    #endregion

    [Export(typeof (IRobotPlugin))]
    public class TeamCityBuildTrigger : RobotPluginBase
    {
        private readonly List<IPluginSetting> _settings;

        const string BodyFormat = @"<build branchName=""{0}""><buildType id=""{1}""/></build>";

        [ImportingConstructor]
        public TeamCityBuildTrigger(IRobot robot)
            : base("TeamCity Build Trigger", robot)
        {
            _settings = new List<IPluginSetting>
            {
                new PluginSetting(Robot, this, "TeamCityServerUrl"),
                new PluginSetting(Robot, this, "TeamCityBuildTriggerUsername"),
                new PluginSetting(Robot, this, "TeamCityBuildTriggerPassword")
            };

            Robot.Messenger.Subscribe<GithubModel>("Github.Push", OnPush);
        }

        private void OnPush(GithubModel githubModel)
        {
            if (githubModel.Deleted) return;

            TriggerTeamCityBuild(githubModel);
        }

        private void TriggerTeamCityBuild(GithubModel model)
        {
            var branch = model.Ref.Replace("refs/heads/", "");

            var buildConfId = "";

            var branchName = string.Empty;

            if (branch.StartsWith("feature") || branch.StartsWith("bugfix"))
            {
                buildConfId = "SkyeEditor_Features";
                branchName = branch.Substring(branch.LastIndexOf('/') + 1);
            }

            if (branch.StartsWith("release"))
            {
                //todo this seems not to work check http://ci.innoveo.com/viewType.html?buildTypeId=SkyeEditor_Release&branch_SkyeEditor_Releases=skye-editor-4.23.0&tab=buildTypeStatusDiv
                buildConfId = "SkyeEditor_Release";
                //LAURENT: TODO move to settings
                var releaseBranchNamePrefix = "/skye-editor-";
                branchName = branch.Substring(branch.LastIndexOf(releaseBranchNamePrefix, StringComparison.Ordinal) + releaseBranchNamePrefix.Length);
            }

            if (string.IsNullOrWhiteSpace(branchName)) return;

            StartBuildAsync(buildConfId, branchName);
        }

        private async void StartBuildAsync(string buildConfId, string branchName)
        {
            var body = string.Format(BodyFormat, branchName, buildConfId);

            var uriBuilder = new UriBuilder(string.Format("{0}httpAuth/app/rest/buildQueue", Robot.Settings.Get("TeamCityServerUrl")));

            var networkCredential = new NetworkCredential(Robot.Settings.Get("TeamCityBuildTriggerUsername"), Robot.Settings.Get("TeamCityBuildTriggerPassword"));
            var handler = new HttpClientHandler {Credentials = networkCredential, PreAuthenticate = true};

            var httpClient = new HttpClient(handler);

            var response = await httpClient.PostAsync(uriBuilder.Uri, new StringContent(body, Encoding.UTF8, "application/xml"));
            if (response.IsSuccessStatusCode)
            {
                Robot.Logger.WriteLine("Started build on TeamCity for branch {0}", branchName);
            }
        }

        public override IEnumerable<IPluginSetting> Settings
        {
            get { return _settings; }
        }
    }
}