namespace Nubot.Plugins.Samples.Stash
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;
    using Interfaces;
    using Models;
    using Nancy;
    using Nancy.ModelBinding;

    [Export(typeof(IRobotPlugin))]
    public class StashListener : HttpPluginBase
    {
        private readonly IEnumerable<IPluginSetting> _settings;

        [ImportingConstructor]
        public StashListener(IRobot robot)
            : base("Stash Listener", "/stash", robot)
        {
            _settings = new List<IPluginSetting>
            {
                new PluginSetting(Robot, "AtlassianStashUrl"),
                new PluginSetting(Robot, "AtlassianStashNotifyRoomName"),
                new PluginSetting(Robot, "AtlassianStashHipchatAuthToken"),
            };

            Post["/"] = x =>
            {
                var model = this.Bind<StashModel>();

                Robot.SendNotification(
                    Robot.Settings.Get("AtlassianStashNotifyRoomName").Trim(),
                    Robot.Settings.Get("AtlassianStashHipchatAuthToken").Trim(),
                    BuildMessage(model),
                    true);

                return HttpStatusCode.OK;
            };
        }

        public override IEnumerable<IPluginSetting> Settings
        {
            get { return _settings; }
        }

        private string BuildMessage(StashModel model)
        {
            var stringBuilder = new StringBuilder();

            var repositoryUrl = string.Format("{0}/projects/{1}/repos/{2}",
                                              Robot.Settings.Get("AtlassianStashUrl"),
                                              model.Repository.Project.Name,
                                              model.Repository.Slug);

            var branches = model.RefChanges.Select(r => r.RefId.Replace("refs/heads/", "")).Distinct();

            if (model.RefChanges.Count() == 1 && model.RefChanges[0].Type.Equals("DELETE", StringComparison.InvariantCultureIgnoreCase))
            {
                stringBuilder
                    .AppendFormat(
                        "Branch <b>{0}</b> of <b>{1}/{2}</b> has been deleted<br/>",
                        string.Join(", ", branches),
                        model.Repository.Project.Name,
                        model.Repository.Name);

                return stringBuilder.ToString();
            }

            var authorNames = model.Changesets.Values.Select(v => v.ToCommit.Author.Name).Distinct();

            stringBuilder
                .AppendFormat(
                    "<b>{0}</b> committed to {1} branch at <a href='{2}'>{3}/{4}</a><br/>",
                    string.Join(", ", authorNames),
                    model.RefChanges.Count(),
                    repositoryUrl + "/browse",
                    model.Repository.Project.Name,
                    model.Repository.Name);

            stringBuilder
                .AppendFormat(
                    @"<b>On branch ""{0}""</b><br/>",
                    string.Join(", ", branches));

            foreach (var changeset in model.Changesets.Values)
            {
                stringBuilder
                    .AppendFormat(
                        @"- {0} (<a href='{1}'>{2}</a>)<br/>",
                        changeset.ToCommit.Message,
                        repositoryUrl + "/commits/" + changeset.ToCommit.Id,
                        changeset.ToCommit.DisplayId);
            }

            return stringBuilder.ToString();
        }
    }
}