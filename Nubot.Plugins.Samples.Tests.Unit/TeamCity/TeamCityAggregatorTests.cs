namespace Nubot.Plugins.Samples.Tests.Unit.TeamCity
{
    using System;
    using System.Reactive.Concurrency;
    using Abstractions;
    using Core.Messaging;
    using Microsoft.Reactive.Testing;
    using NSubstitute;
    using NUnit.Framework;
    using Samples.TeamCity;
    using Samples.TeamCity.Model;

    public class TeamCityAggregatorTests
    {
        [Test]
        public void SendNotification_5SuccessfulForSameBuildEventSentUnderThe8MinutesTimeOut_ExpectSuccessfulNotificationSentWithCorrectMessage()
        {
            //Arrange
            var robot = Substitute.For<IRobot>();

            var eventEmitter = new MvvmLightMessenger();
            robot.Messenger.Returns(eventEmitter);

            var settings = Substitute.For<ISettings>();
            settings.Get<double>("TeamCityBuildsMaxDuration").Returns(8.0);
            settings.Get<int>("TeamCityExpectedBuildCount").Returns(5);
            robot.Settings.Returns(settings);

            var scheduler = new TestScheduler();
            var teamCityAggregator = new TeamCityAggregatorSut(robot, scheduler);

            var successfulTeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "10", buildResult = "success" } };

            //Act
            scheduler.Schedule(TimeSpan.FromMinutes(1), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(1), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(2), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(3), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(5), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));

            scheduler.AdvanceTo(TimeSpan.FromMinutes(8).Ticks);

            //Assert
            robot.Received(1).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void SendNotification_5SuccessfulForSameBuildEventSentUnderThe8MinutesTimeOutSliding_ExpectSuccessfulNotificationSentWithCorrectMessage()
        {
            //Arrange
            var robot = Substitute.For<IRobot>();

            var eventEmitter = new MvvmLightMessenger();
            robot.Messenger.Returns(eventEmitter);

            var settings = Substitute.For<ISettings>();
            settings.Get<double>("TeamCityBuildsMaxDuration").Returns(8.0);
            settings.Get<int>("TeamCityExpectedBuildCount").Returns(5);
            robot.Settings.Returns(settings);

            var scheduler = new TestScheduler();
            var teamCityAggregator = new TeamCityAggregatorSut(robot, scheduler);

            var successfulTeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "10", buildResult = "success" } };

            //Act
            scheduler.Schedule(TimeSpan.FromMinutes(2), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(3), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(4), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(5), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(13), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));

            scheduler.AdvanceTo(TimeSpan.FromMinutes(14).Ticks);

            //Assert
            robot.Received(1).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void SendNotification_4SuccessfulAnd1FailedForSameBuildEventSent_ExpectFailedNotificationSentWithCorrectMessage()
        {
            //Arrange
            var robot = Substitute.For<IRobot>();

            var eventEmitter = new MvvmLightMessenger();
            robot.Messenger.Returns(eventEmitter);

            var settings = Substitute.For<ISettings>();
            settings.Get<double>("TeamCityBuildsMaxDuration").Returns(8.0);
            settings.Get<int>("TeamCityExpectedBuildCount").Returns(5);
            robot.Settings.Returns(settings);

            var scheduler = new TestScheduler();
            var teamCityAggregator = new TeamCityAggregatorSut(robot, scheduler);

            var successfulTeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "10", buildResult = "success" } };
            var failedTeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "10", buildResult = "failed" } };

            //Act
            scheduler.Schedule(TimeSpan.FromMinutes(1), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(1), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(2), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(3), () => eventEmitter.Publish("TeamCity.BuildStatus", failedTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(4), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));

            scheduler.AdvanceTo(TimeSpan.FromMinutes(5).Ticks);

            //Assert
            robot.Received(1).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), true);
        }

        [Test]
        public void SendNotification_5SuccessfulForSameBuildEventSentButOneNotInTime_ExpectFailedNotificationSentWithCorrectMessage()
        {
            //Arrange
            var robot = Substitute.For<IRobot>();

            var eventEmitter = new MvvmLightMessenger();
            robot.Messenger.Returns(eventEmitter);

            var settings = Substitute.For<ISettings>();
            settings.Get<double>("TeamCityBuildsMaxDuration").Returns(8.0);
            settings.Get<int>("TeamCityExpectedBuildCount").Returns(5);
            robot.Settings.Returns(settings);

            var scheduler = new TestScheduler();
            var teamCityAggregator = new TeamCityAggregatorSut(robot, scheduler);

            var successfulTeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "10", buildResult = "success" } };

            //Act
            scheduler.Schedule(TimeSpan.FromMinutes(0), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(2), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(4), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(5), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(14), () => eventEmitter.Publish("TeamCity.BuildStatus", successfulTeamCityBuildModel));

            scheduler.AdvanceBy(TimeSpan.FromMinutes(14).Ticks);

            //Assert
            robot.Received(1).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), true);
        }

        [Test]
        public void SendNotification_2Time5SuccessfulAnd1TimeFailedForSameBuildEventSentAllOnTime_Expect1FailedNotificationAnd2SuccessfulSentWithCorrectMessage()
        {
            //Arrange
            var robot = Substitute.For<IRobot>();

            var eventEmitter = new MvvmLightMessenger();
            robot.Messenger.Returns(eventEmitter);

            var settings = Substitute.For<ISettings>();
            settings.Get<double>("TeamCityBuildsMaxDuration").Returns(8.0);
            settings.Get<int>("TeamCityExpectedBuildCount").Returns(5);
            robot.Settings.Returns(settings);

            var scheduler = new TestScheduler();
            var teamCityAggregator = new TeamCityAggregatorSut(robot, scheduler);

            var successful1TeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "10", buildResult = "success" } };
            var successful2TeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "20", buildResult = "success" } };
            var failed3TeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "30", buildResult = "failed" } };

            //Act
            scheduler.Schedule(TimeSpan.FromMinutes(0), () => eventEmitter.Publish("TeamCity.BuildStatus", successful1TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(2), () => eventEmitter.Publish("TeamCity.BuildStatus", successful1TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(4), () => eventEmitter.Publish("TeamCity.BuildStatus", successful1TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(5), () => eventEmitter.Publish("TeamCity.BuildStatus", successful1TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(5.5), () => eventEmitter.Publish("TeamCity.BuildStatus", successful1TeamCityBuildModel));

            scheduler.Schedule(TimeSpan.FromMinutes(1), () => eventEmitter.Publish("TeamCity.BuildStatus", successful2TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(3), () => eventEmitter.Publish("TeamCity.BuildStatus", successful2TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(4), () => eventEmitter.Publish("TeamCity.BuildStatus", successful2TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(6), () => eventEmitter.Publish("TeamCity.BuildStatus", successful2TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(7), () => eventEmitter.Publish("TeamCity.BuildStatus", successful2TeamCityBuildModel));

            scheduler.Schedule(TimeSpan.FromMinutes(0.1), () => eventEmitter.Publish("TeamCity.BuildStatus", failed3TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(3), () => eventEmitter.Publish("TeamCity.BuildStatus", failed3TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(4), () => eventEmitter.Publish("TeamCity.BuildStatus", failed3TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(4), () => eventEmitter.Publish("TeamCity.BuildStatus", failed3TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(4.1), () => eventEmitter.Publish("TeamCity.BuildStatus", failed3TeamCityBuildModel));

            scheduler.AdvanceBy(TimeSpan.FromMinutes(9).Ticks);

            //Assert
            robot.Received(2).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), false);
            robot.Received(1).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), true);
        }

        [Test]
        public void SendNotification_5SuccessfulSameBuildEventSentInTimeThenMaxDurationTimePassed_ExpectOnly1SuccessNotificationSentWithCorrectMessage()
        {
            //Arrange
            var robot = Substitute.For<IRobot>();

            var eventEmitter = new MvvmLightMessenger();
            robot.Messenger.Returns(eventEmitter);

            var settings = Substitute.For<ISettings>();
            settings.Get<double>("TeamCityBuildsMaxDuration").Returns(8.0);
            settings.Get<int>("TeamCityExpectedBuildCount").Returns(5);
            robot.Settings.Returns(settings);

            var scheduler = new TestScheduler();
            var teamCityAggregator = new TeamCityAggregatorSut(robot, scheduler);

            var successful1TeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "10", buildResult = "success" } };

            //Act
            scheduler.Schedule(TimeSpan.FromMinutes(0), () => eventEmitter.Publish("TeamCity.BuildStatus", successful1TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(2), () => eventEmitter.Publish("TeamCity.BuildStatus", successful1TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(4), () => eventEmitter.Publish("TeamCity.BuildStatus", successful1TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(5), () => eventEmitter.Publish("TeamCity.BuildStatus", successful1TeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(5.5), () => eventEmitter.Publish("TeamCity.BuildStatus", successful1TeamCityBuildModel));

            scheduler.AdvanceBy(TimeSpan.FromMinutes(15).Ticks);

            //Assert
            robot.Received(1).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), false);
        }

        private class TeamCityAggregatorSut : TeamCityAggregator
        {
            public TeamCityAggregatorSut(IRobot robot, TestScheduler testScheduler)
                : base(robot)
            {
                Scheduler = testScheduler;
            }

            protected override IScheduler Scheduler { get; }
        }
    }
}