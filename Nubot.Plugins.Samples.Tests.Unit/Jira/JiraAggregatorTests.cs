namespace Nubot.Plugins.Samples.Tests.Unit.Jira
{
    using System;
    using System.Reactive.Concurrency;
    using Abstractions;
    using Core.Messaging;
    using Microsoft.Reactive.Testing;
    using NSubstitute;
    using NUnit.Framework;
    using Samples.Jira;
    using Samples.Jira.Models;
    using User = Samples.Jira.Models.User;

    public class JiraAggregatorTests
    {
        [Test]
        public void SendNotification_1EventAfter1MinuteAnotherAfter1Minute30_Expect1NotificationSentAfter5Minutes()
        {
            //Arrange
            var eventEmitter = new MvvmLightMessenger();

            var robot = Substitute.For<IRobot>();
            robot.EventEmitter.Returns(eventEmitter);

            var scheduler = new TestScheduler();
            var jiraAggregator = new JiraAggregatorSut(robot, scheduler);

            var user = new User {displayName = "Laurent Kempé", name = "laurent"};

            var addCommentEvent1 = new JiraModel
            {
                issue = new Issue {key = "LK-10", fields = new Fields {reporter = user, assignee = user}},
                webhookEvent = "jira:issue_created",
                comment = new CommentDetails(),
                user = user
            };

            var addCommentEvent2 = new JiraModel
            {
                issue = new Issue {key = "LK-10", fields = new Fields {reporter = user, assignee = user}},
                webhookEvent = "jira:issue_updated",
                comment = new CommentDetails(),
                user = user
            };

            //Act
            scheduler.Schedule(TimeSpan.FromMinutes(1), () => eventEmitter.Emit("JiraEvent", addCommentEvent1));
            scheduler.Schedule(TimeSpan.FromSeconds(90), () => eventEmitter.Emit("JiraEvent", addCommentEvent2));

            scheduler.AdvanceTo(TimeSpan.FromMinutes(5).Ticks);

            //Assert
            robot.Received(1).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), true);
        }

        [Test]
        public void SendNotification_1EventAfter1MinuteThenNothingDuring7Minutes_Expect1NotificationSentAfter8Minutes()
        {
            //Arrange
            var eventEmitter = new MvvmLightMessenger();

            var robot = Substitute.For<IRobot>();
            robot.EventEmitter.Returns(eventEmitter);

            var scheduler = new TestScheduler();
            var jiraAggregator = new JiraAggregatorSut(robot, scheduler);

            var user = new User { displayName = "Laurent Kempé", name = "laurent" };

            var addCommentEvent1 = new JiraModel
            {
                issue = new Issue { key = "LK-10", fields = new Fields { reporter = user, assignee = user } },
                webhookEvent = "jira:issue_updated",
                comment = new CommentDetails(),
                user = user
            };

            //Act
            scheduler.Schedule(TimeSpan.FromMinutes(1), () => eventEmitter.Emit("JiraEvent", addCommentEvent1));

            scheduler.AdvanceTo(TimeSpan.FromMinutes(8).Ticks);

            //Assert
            robot.Received(1).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), true);
        }

        [Test]
        public void SendNotification_1EventAfter0SecondThen1After3Minutes_Expect1NotificationSentAfter5Minutes()
        {
            //Arrange
            var eventEmitter = new MvvmLightMessenger();

            var robot = Substitute.For<IRobot>();
            robot.EventEmitter.Returns(eventEmitter);

            var scheduler = new TestScheduler();
            var jiraAggregator = new JiraAggregatorSut(robot, scheduler);

            var user = new User { displayName = "Laurent Kempé", name = "laurent" };

            var addCommentEvent1 = new JiraModel
            {
                issue = new Issue { key = "LK-10", fields = new Fields { reporter = user, assignee = user } },
                webhookEvent = "jira:issue_created",
                comment = new CommentDetails(),
                user = user
            };

            var addCommentEvent2 = new JiraModel
            {
                issue = new Issue { key = "LK-10", fields = new Fields { reporter = user, assignee = user } },
                webhookEvent = "jira:issue_updated",
                comment = new CommentDetails(),
                user = user
            };

            //Act
            scheduler.Schedule(TimeSpan.FromSeconds(0), () => eventEmitter.Emit("JiraEvent", addCommentEvent1));
            scheduler.Schedule(TimeSpan.FromSeconds(120), () => eventEmitter.Emit("JiraEvent", addCommentEvent2));

            scheduler.AdvanceTo(TimeSpan.FromSeconds(181).Ticks);
            robot.Received(0).SendNotification("", "", "Message", true);

            scheduler.AdvanceTo(TimeSpan.FromMinutes(5).Ticks);

            //Assert
            robot.Received(1).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), true);
        }

        private class JiraAggregatorSut : JiraAggregator
        {
            private readonly IScheduler _testScheduler;

            public JiraAggregatorSut(IRobot robot, TestScheduler testScheduler)
                : base(robot)
            {
                _testScheduler = testScheduler;
            }

            protected override IScheduler Scheduler
            {
                get { return _testScheduler; }
            }
        }
    }
}