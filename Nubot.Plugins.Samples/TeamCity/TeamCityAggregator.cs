namespace Nubot.Plugins.Samples.TeamCity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Abstractions;
    using Model;

    [Export(typeof(IRobotPlugin))]
    public class TeamCityAggregator : RobotPluginBase
    {
        private readonly IEnumerable<IPluginSetting> _settings;
        private readonly Subject<TeamCityModel> _subject;
        private const int ExpectedBuildCount = 5;

        [ImportingConstructor]
        public TeamCityAggregator(IRobot robot)
            : base("TeamCity Aggregator", robot)
        {
            _settings = new List<IPluginSetting>
            {
                new PluginSetting(Robot, this, "TeamCityNotifyRoomName"),
                new PluginSetting(Robot, this, "TeamCityHipchatAuthToken"),
                new PluginSetting(Robot, this, "TeamCityBuildsMaxDuration")
            };

            _subject = new Subject<TeamCityModel>();

            var maxWaitDuration = TimeSpan.FromMinutes(double.Parse(Robot.Settings.Get("TeamCityBuildsMaxDuration")));

            _subject
                .GroupBy(model => model.build.buildNumber)
                .Subscribe(grp => grp.Buffer(maxWaitDuration, ExpectedBuildCount, Scheduler).Take(1).Subscribe(SendNotification));

            Robot.EventEmitter.On<TeamCityModel>("TeamCityBuild", OnTeamCityBuild);
        }

        protected virtual IScheduler Scheduler
        {
            get { return DefaultScheduler.Instance; }
        }

        private void OnTeamCityBuild(IMessage<TeamCityModel> message)
        {
            _subject.OnNext(message.Content);
        }

        private void SendNotification(IList<TeamCityModel> buildStatuses)
        {
            bool notify;
            var message = new TeamCityMessageBuilder(ExpectedBuildCount).BuildMessage(buildStatuses, out notify);

            //todo add color of the line https://www.hipchat.com/docs/api/method/rooms/message
            //todo Background color for message. One of "yellow", "red", "green", "purple", "gray", or "random". (default: yellow)

            Robot.SendNotification(
                Robot.Settings.Get("TeamCityNotifyRoomName"),
                Robot.Settings.Get("TeamCityHipchatAuthToken"),
                message,
                notify);
        }

        public override IEnumerable<IPluginSetting> Settings
        {
            get { return _settings; }
        }
    }
}