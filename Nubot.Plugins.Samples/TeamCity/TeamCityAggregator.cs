namespace Nubot.Plugins.Samples.TeamCity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Abstractions;
    using Abstractions.ReactiveExtensions;
    using Model;

    [Export(typeof(IRobotPlugin))]
    public class TeamCityAggregator : RobotPluginBase
    {
        private readonly Subject<TeamCityModel> _subject;
        private readonly int _expectedBuildCount;

        [ImportingConstructor]
        public TeamCityAggregator(IRobot robot)
            : base("TeamCity Aggregator", robot)
        {
            Settings = new List<IPluginSetting>
            {
                new PluginSetting(Robot, this, "TeamCityNotifyRoomName"),
                new PluginSetting(Robot, this, "TeamCityHipchatAuthToken"),
                new PluginSetting(Robot, this, "TeamCityBuildsMaxDuration"),
                new PluginSetting(Robot, this, "TeamCityExpectedBuildCount")
            };

            _subject = new Subject<TeamCityModel>();

            _expectedBuildCount = Robot.Settings.Get<int>("TeamCityExpectedBuildCount");

            var maxWaitDuration = TimeSpan.FromMinutes(Robot.Settings.Get<double>("TeamCityBuildsMaxDuration"));

            var buildsPerBuildNumber = _subject.GroupBy(model => model.build.buildNumber);

            buildsPerBuildNumber.Subscribe(grp => grp.BufferUntilInactive(maxWaitDuration, Scheduler, _expectedBuildCount).Take(1).Subscribe(SendNotification));

            Robot.Messenger.Subscribe<TeamCityModel>("TeamCity.BuildStatus", OnTeamCityBuild);
        }

        protected virtual IScheduler Scheduler => DefaultScheduler.Instance;

        private void OnTeamCityBuild(TeamCityModel teamCityModel)
        {
            _subject.OnNext(teamCityModel);
        }

        private void SendNotification(IList<TeamCityModel> buildStatuses)
        {
            bool notify;
            var message = new TeamCityMessageBuilder(_expectedBuildCount).BuildMessage(buildStatuses, out notify);

            //todo add color of the line https://www.hipchat.com/docs/api/method/rooms/message
            //todo Background color for message. One of "yellow", "red", "green", "purple", "gray", or "random". (default: yellow)

            Robot.SendNotification(
                Robot.Settings.Get("TeamCityNotifyRoomName"),
                Robot.Settings.Get("TeamCityHipchatAuthToken"),
                message,
                notify);
        }

        public override IEnumerable<IPluginSetting> Settings { get; }
    }
}