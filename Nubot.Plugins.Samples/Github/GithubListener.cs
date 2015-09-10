namespace Nubot.Plugins.Samples.Github
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;
    using Abstractions;
    using Models;
    using Nancy;
    using Nancy.ModelBinding;

    [Export(typeof(IRobotPlugin))]
    public class GithubListener : HttpPluginBase
    {
        private readonly IEnumerable<IPluginSetting> _settings;

        [ImportingConstructor]
        public GithubListener(IRobot robot)
            : base("Github Listener", "/github", robot)
        {
            _settings = new List<IPluginSetting>
            {
                new PluginSetting(Robot, this, "GithubUrl"),
                new PluginSetting(Robot, this, "GithubNotifyRoomName"),
                new PluginSetting(Robot, this, "GithubHipchatAuthToken"),
            };

            Post["/"] = x =>
            {
                var model = this.Bind<GithubModel>();

                Robot.EventEmitter.Emit("Github.Push", model);

                Robot.SendNotification(
                    Robot.Settings.Get("GithubNotifyRoomName"),
                    Robot.Settings.Get("GithubHipchatAuthToken"),
                    BuildMessage(model));

                return HttpStatusCode.OK;
            };
        }

        private static string BuildMessage(GithubModel model)
        {
            var branch = model.Ref.Replace("refs/heads/", "");
            var authorNames = model.Commits.Select(c => c.Author.Name).Distinct();

            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendFormat(
                    "<b>{0}</b> committed on <a href='{1}'>{2}</a><br/>",
                    string.Join(", ", authorNames),
                    model.Repository.HtmlUrl + "/tree/" + branch,
                    branch
                    );

            foreach (var commit in model.Commits)
            {
                stringBuilder
                    .AppendFormat(
                        @"- {0} (<a href='{1}'>{2}</a>)<br/>",
                        commit.Message,
                        commit.Url,
                        commit.Id.Substring(0, 11));
            }

            return stringBuilder.ToString();
        }

        public override IEnumerable<IPluginSetting> Settings
        {
            get { return _settings; }
        }
    }
}
