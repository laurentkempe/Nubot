namespace Nubot.Plugins.Samples.Tests.Unit.Jira
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Samples.Jira;
    using Samples.Jira.Models;

    public class JiraMessageBuilderTests
    {
        [Test]
        public void BuildMessage_IssueCreatedThenUpdated_ExpectOneCreationMessage()
        {
            //Arrange
            var jiraMessageBuilder = new JiraMessageBuilder("http://jira");

            var user = new User {displayName = "Laurent Kempé", name = "laurent"};

            var sameIssue = new Issue {key = "LK-10", fields = new Fields {summary = "Issue Description", reporter = user, assignee = user}};

            var jiraModel1 = new JiraModel
            {
                webhookEvent = "jira:issue_created",
                issue = sameIssue
            };

            var jiraModel2 = new JiraModel
            {
                webhookEvent = "jira:issue_updated",
                issue = sameIssue
            };

            var sameJiraIssueKeyEvents = new List<JiraModel> { jiraModel1, jiraModel2 };

            //Act
            var buildMessage = jiraMessageBuilder.BuildMessage(sameJiraIssueKeyEvents);

            //Assert
            Assert.That(buildMessage, Is.EqualTo("<b><a href='http://jira/browse/LK-10'>LK-10 Issue Description</a></b> has been created by <a href='http://jira/secure/ViewProfile.jspa?name=laurent'>Laurent Kempé</a> and current assignee is <a href='http://jira/secure/ViewProfile.jspa?name=laurent'>Laurent Kempé</a>."));
        }
    }
}