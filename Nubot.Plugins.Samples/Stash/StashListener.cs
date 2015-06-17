namespace Nubot.Plugins.Samples.Stash
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;
    using Abstractions;
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
                new PluginSetting(Robot, this, "AtlassianStashUrl"),
                new PluginSetting(Robot, this, "AtlassianStashNotifyRoomName"),
                new PluginSetting(Robot, this, "AtlassianStashHipchatAuthToken"),
            };

            Post["/"] = x =>
            {
                var model = this.Bind<StashModel>();

                Robot.EventEmitter.Emit("StashCommit", model);

                Robot.SendNotification(
                    Robot.Settings.Get("AtlassianStashNotifyRoomName"),
                    Robot.Settings.Get("AtlassianStashHipchatAuthToken"),
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
            var repositoryUrl = string.Format("{0}/projects/{1}/repos/{2}",
                                              Robot.Settings.Get("AtlassianStashUrl"),
                                              model.Repository.Project.Name,
                                              model.Repository.Slug);

            var branches = model.RefChanges.Select(r => r.RefId.Replace("refs/heads/", "")).Distinct().ToList();

            string deleteMessage;
            if (BuildDeleteMessage(model, branches, out deleteMessage)) return deleteMessage;

            var authorNames = model.Changesets.Values.Select(v => v.ToCommit.Author.Name).Distinct().ToList();

            string mergeMessage;
            if (BuildMergeMessage(model, repositoryUrl, branches, authorNames, out mergeMessage)) return mergeMessage;

            var stringBuilder = new StringBuilder();

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

        private static bool BuildDeleteMessage(StashModel model, IEnumerable<string> branches, out string deleteMessage)
        {
            if (!IsBranchDelete(model))
            {
                deleteMessage = string.Empty;
                return false;
            }

            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendFormat(
                    "Branch <b>{0}</b> of <b>{1}/{2}</b> has been deleted<br/>",
                    string.Join(", ", branches),
                    model.Repository.Project.Name,
                    model.Repository.Name);

            deleteMessage = stringBuilder.ToString();
            return true;
        }

        private static bool IsBranchDelete(StashModel model)
        {
            return model.RefChanges.Count() == 1 && model.RefChanges[0].Type.Equals("DELETE", StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool BuildMergeMessage(StashModel model, string repositoryUrl, IEnumerable<string> branches, List<string> authorNames, out string mergeMessage)
        {
            var changeset = model.Changesets.Values[0];

            if (!IsPullRequestMerge(changeset))
            {
                mergeMessage = string.Empty;
                return false;
            }

            const string commit = "* commit ";
            var commitMessage = changeset.ToCommit.Message;

            var restMessage = commitMessage.Substring(0, commitMessage.LastIndexOf(commit, StringComparison.Ordinal)).Trim();

            var pullRequestNumber = GetPullRequestNumber(restMessage);

            var fromTo = restMessage.Substring(restMessage.IndexOf(" from ", StringComparison.Ordinal)).Trim();

            var pullRequestUrl = string.Format(@"{0}/pull-requests/{1}/overview", repositoryUrl, pullRequestNumber);

            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendFormat(
                    "<b>Pull request #<a href='{0}'>{1}</a></b> has been merged ",
                    pullRequestUrl,
                    pullRequestNumber);

            stringBuilder
                .AppendFormat(
                    "in <a href='{0}'>{1}/{2}</a><br/>",
                    repositoryUrl + "/browse",
                    model.Repository.Project.Name,
                    model.Repository.Name);

            stringBuilder
                .AppendFormat(
                    @"{0} (<a href='{1}'>{2}</a>)<br/>",
                    fromTo,
                    repositoryUrl + "/commits/" + changeset.ToCommit.Id,
                    changeset.ToCommit.DisplayId);

            mergeMessage = stringBuilder.ToString();
            return true;
        }

        private static string GetPullRequestNumber(string restMessage)
        {
            var substring = restMessage.Substring(restMessage.IndexOf('#') + 1);
            return substring.Substring(0, substring.IndexOf(' ')).Trim();
        }

        private static bool IsPullRequestMerge(Value changeset)
        {
            return changeset.ToCommit.Message.StartsWith("Merge pull request #");
        }
    }
}