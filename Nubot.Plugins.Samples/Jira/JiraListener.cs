namespace Nubot.Plugins.Samples.Jira
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;
    using Interfaces;
    using Models;
    using Nancy;
    using Nancy.ModelBinding;

    [Export(typeof(IRobotPlugin))]
    public class JiraListener : HttpPluginBase
    {
        private readonly IEnumerable<IPluginSetting> _settings;

        private string JiraBaseUrl { get { return Robot.Settings.Get("AtlassianJiraUrl"); } }

        [ImportingConstructor]
        public JiraListener(IRobot robot)
            : base("Jira Listener", "/jira", robot)
        {
            _settings = new List<IPluginSetting>
            {
                new PluginSetting(Robot, "AtlassianJiraNotifyRoomName"),
                new PluginSetting(Robot, "AtlassianJiraUrl")
            };

            Post["/"] = x =>
            {
                var model = this.Bind<JiraModel>(new BindingConfig { IgnoreErrors = true, BodyOnly = true, Overwrite = true });

                Robot.SendNotification(
                    Robot.Settings.Get("AtlassianJiraNotifyRoomName").Trim(),
                    BuildMessage(model),
                    true);

                return HttpStatusCode.OK;
            };
        }

        public override IEnumerable<IPluginSetting> Settings
        {
            get { return _settings; }
        }

        private string BuildMessage(JiraModel model)
        {
            var stringBuilder = new StringBuilder();

            //SKYE-1650: Test webhook for the editor In Progress→Impeded→Impeded by Laurent Kempé. Current assignee is Laurent Kempé.

            switch (model.webhookEvent)
            {
                case "jira:issue_updated" : 
                    BuildChangeLogMessage(model, stringBuilder);
                    break;

                case "jira:issue_created": 
                    BuildIssueCreatedMessage(model, stringBuilder);
                    break;
            }

            return stringBuilder.ToString();
        }

        private void BuildIssueCreatedMessage(JiraModel model, StringBuilder stringBuilder)
        {
            stringBuilder
                .AppendFormat(
                    "<b>{0}</b> has been created by {1} and current assignee is {2}.",
                    GetFullIssueLink(model.issue),
                    GetUserProfileLinkHtml(model.issue.fields.reporter),
                    GetUserProfileLinkHtml(model.issue.fields.assignee));
        }

        private void BuildChangeLogMessage(JiraModel model, StringBuilder stringBuilder)
        {
            if (model.comment != null)
            {
                stringBuilder
                    .AppendFormat(
                        "{0} added a comment to <b><a href='{1}'>{2}</a></b>.",
                        GetUserProfileLinkHtml(model.user),
                        GetIssueLink(model.issue),
                        GetIssueDescription(model.issue));
                
                return;
            }

            if (model.changelog.items.Length == 2 &&
              ((model.changelog.items.Any(i => i.field == "assignee") && model.changelog.items.Any(i => i.field == "status"))))
            {
                var status = model.changelog.items.First(i => i.field == "status");

                if (status.toString == "Review")
                {
                    stringBuilder.AppendFormat(
                        "{0}, <b>{1}</b> has just been assigned to you for code review.",
                        GetUserProfileLinkHtml(model.issue.fields.assignee),
                        GetFullIssueLink(model.issue));
                }
            }
            else
            {
                switch (model.changelog.items[0].field)
                {
                    case "assignee":

                        stringBuilder.AppendFormat(
                            "{0}, <b>{1}</b> has just been assigned to you.",
                            GetUserProfileLinkHtml(model.issue.fields.assignee),
                            GetFullIssueLink(model.issue));
                        break;

                    case "status":

                        stringBuilder
                            .AppendFormat(
                                "<b><a href='{0}'>{1}</a></b> {2} -> {3} by {4}. Current assignee is {5}.",
                                GetIssueLink(model.issue),
                                GetIssueDescription(model.issue),
                                model.changelog.items[0].fromString,
                                model.changelog.items[0].toString,
                                GetUserProfileLinkHtml(model.user),
                                GetUserProfileLinkHtml(model.issue.fields.assignee));
                        break;
                }
            }
        }

        private string GetUserProfileLinkHtml(User user)
        {
            return string.Format("<a href='{0}'>{1}</a>", GetUserProfileLink(user.name), user.displayName);
        }

        private string GetFullIssueLink(Issue issue)
        {
            return string.Format("<a href='{0}'>{1}</a>", GetIssueLink(issue), GetIssueDescription(issue));
        }

        private string GetIssueLink(Issue issue)
        {
            return string.Format("{0}/browse/{1}", JiraBaseUrl, issue.key);
        }

        private static string GetIssueDescription(Issue issue)
        {
            return string.Format("{0} {1}", issue.key, issue.fields.summary);
        }

        private string GetUserProfileLink(string username)
        {
            return JiraBaseUrl + "/secure/ViewProfile.jspa?name=" + username;
        }
    }
}