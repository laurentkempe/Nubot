namespace Nubot.Plugins.Samples.Jira
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Models;

    public class JiraMessageBuilder
    {
        private readonly string _jiraBaseUrl;

        public JiraMessageBuilder(string jiraBaseUrl)
        {
            _jiraBaseUrl = jiraBaseUrl;
        }

        public string BuildMessage(List<JiraModel> sameJiraIssueKeyEvents)
        {
            var stringBuilder = new StringBuilder();

            var firstEvent = sameJiraIssueKeyEvents.First();

            if (CreatedIssueThenUpdated(sameJiraIssueKeyEvents, firstEvent))
            {
                BuildIssueCreatedMessage(firstEvent, stringBuilder);

                return stringBuilder.ToString();
            }

            if (OnlyUpdatedIssue(sameJiraIssueKeyEvents))
            {
                BuildChangeLogMessage(sameJiraIssueKeyEvents.Last(), stringBuilder);

                return stringBuilder.ToString();
            }

            switch (firstEvent.webhookEvent)
            {
                case "jira:issue_created":
                    BuildIssueCreatedMessage(firstEvent, stringBuilder);
                    break;

                case "jira:issue_updated":
                    BuildChangeLogMessage(firstEvent, stringBuilder);
                    break;
            }

            return stringBuilder.ToString();
        }

        private static bool CreatedIssueThenUpdated(List<JiraModel> sameJiraIssueKeyEvents, JiraModel firstEvent)
        {
            return firstEvent.webhookEvent.Equals("jira:issue_created") &&
                   sameJiraIssueKeyEvents.Skip(1).All(e => e.webhookEvent.Equals("jira:issue_updated"));
        }

        private static bool OnlyUpdatedIssue(List<JiraModel> sameJiraIssueKeyEvents)
        {
            return sameJiraIssueKeyEvents.All(e => e.webhookEvent.Equals("jira:issue_updated"));
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
            if (CommentAdded(model))
            {
                stringBuilder
                    .AppendFormat(
                        "{0} added a comment to <b>{1}</b>.",
                        GetUserProfileLinkHtml(model.user),
                        GetFullIssueLink(model.issue));

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

                    if (model.comment != null)
                    {
                        if (!string.IsNullOrEmpty(model.comment.body))
                        {
                            stringBuilder
                                .AppendFormat(
                                    "<br/>With followig comment '{0}'",
                                    model.comment.body);
                        }
                    }
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

        private static bool CommentAdded(JiraModel model)
        {
            return model.comment != null && model.changelog == null;
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
            return string.Format("{0}/browse/{1}", _jiraBaseUrl, issue.key);
        }

        private static string GetIssueDescription(Issue issue)
        {
            return string.Format("{0} {1}", issue.key, issue.fields.summary);
        }

        private string GetUserProfileLink(string username)
        {
            return string.Format("{0}/secure/ViewProfile.jspa?name={1}", _jiraBaseUrl, username);
        }
    }
}