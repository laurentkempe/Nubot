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
        public void SendNotification_4SuccessfulForSameBuildEventSentUnderThe6MinutesTimeOut_ExpectSuccessfulNotificationSentWithCorrectMessage()
        {
            //Arrange
            var eventEmitter = new MvvmLightMessenger();

            var robot = Substitute.For<IRobot>();
            robot.EventEmitter.Returns(eventEmitter);

            var scheduler = new TestScheduler();
            var teamCityAggregator = new TeamCityAggregatorSut(robot, scheduler);

            var successfulTeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "10", buildResult = "success" } };

            //Act
            scheduler.Schedule(TimeSpan.FromMinutes(1), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(1), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(2), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(3), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));

            scheduler.AdvanceTo(TimeSpan.FromMinutes(8).Ticks);

            //Assert
            robot.Received(1).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void SendNotification_4SuccessfulForSameBuildEventSentUnderThe6MinutesTimeOutSliding_ExpectSuccessfulNotificationSentWithCorrectMessage()
        {
            //Arrange
            var eventEmitter = new MvvmLightMessenger();

            var robot = Substitute.For<IRobot>();
            robot.EventEmitter.Returns(eventEmitter);

            var scheduler = new TestScheduler();
            var teamCityAggregator = new TeamCityAggregatorSut(robot, scheduler);

            var successfulTeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "10", buildResult = "success" } };

            //Act
            scheduler.Schedule(TimeSpan.FromMinutes(1), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(2), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(3), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(7), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));

            scheduler.AdvanceTo(TimeSpan.FromMinutes(8).Ticks);

            //Assert
            robot.Received(1).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void SendNotification_3SuccessfulAnd1FailedForSameBuildEventSent_ExpectFailedNotificationSentWithCorrectMessage()
        {
            //Arrange
            var eventEmitter = new MvvmLightMessenger();

            var robot = Substitute.For<IRobot>();
            robot.EventEmitter.Returns(eventEmitter);

            var scheduler = new TestScheduler();
            var teamCityAggregator = new TeamCityAggregatorSut(robot, scheduler);

            var successfulTeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "10", buildResult = "success" } };
            var failedTeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "10", buildResult = "failed" } };

            //Act
            scheduler.Schedule(TimeSpan.FromMinutes(1), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(1), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(2), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(3), () => eventEmitter.Emit("TeamCityBuild", failedTeamCityBuildModel));

            scheduler.AdvanceTo(TimeSpan.FromMinutes(3).Ticks);

            //Assert
            robot.Received(1).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), true);
        }

        [Test]
        public void SendNotification_4SuccessfulForSameBuildEventSentButOneNotInTime_ExpectFailedNotificationSentWithCorrectMessage()
        {
            //Arrange
            var eventEmitter = new MvvmLightMessenger();

            var robot = Substitute.For<IRobot>();
            robot.EventEmitter.Returns(eventEmitter);

            var scheduler = new TestScheduler();
            var teamCityAggregator = new TeamCityAggregatorSut(robot, scheduler);

            var successfulTeamCityBuildModel = new TeamCityModel { build = new Build { buildNumber = "10", buildResult = "success" } };

            //Act
            scheduler.Schedule(TimeSpan.FromMinutes(0), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(2), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(4), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));
            scheduler.Schedule(TimeSpan.FromMinutes(9), () => eventEmitter.Emit("TeamCityBuild", successfulTeamCityBuildModel));

            scheduler.AdvanceBy(TimeSpan.FromMinutes(9).Ticks);

            //Assert
            robot.Received(1).SendNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), true);
        }

        private class TeamCityAggregatorSut : TeamCityAggregator
        {
            private readonly IScheduler _testScheduler;

            public TeamCityAggregatorSut(IRobot robot, TestScheduler testScheduler)
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