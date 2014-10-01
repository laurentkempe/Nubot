namespace Nubot.Plugins.Samples.TeamCity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Text;
    using System.Threading;
    using Interfaces;
    using Model;

    [Export(typeof(IRobotPlugin))]
    public class TeamCityAggregator : RobotPluginBase
    {
        private readonly IEnumerable<IPluginSetting> _settings;
        private readonly Subject<TeamCityModel> _subject;
        private const int ExpectedBuildCount = 4;

        [ImportingConstructor]
        public TeamCityAggregator(IRobot robot)
            : base("TeamCityAggregator", robot)
        {
            _settings = new List<IPluginSetting>
            {
                new PluginSetting(Robot, "TeamCityNotifyRoomName"),
                new PluginSetting(Robot, "TeamCityHipchatAuthToken")
            };

            _subject = new Subject<TeamCityModel>();

            var maxWaitDuration = TimeSpan.FromMinutes(8);

            _subject
                .GroupBy(model => model.build.buildNumber)
                .Subscribe(grp =>
                            {
                                var tokenSource = new CancellationTokenSource();
                                grp.Buffer(maxWaitDuration, ExpectedBuildCount).Subscribe(builds => SendNotification(builds, tokenSource), tokenSource.Token);
                            });

            Robot.Messenger.On<TeamCityModel>("TeamCityBuild", OnTeamCityBuild);
        }

        private void OnTeamCityBuild(IMessage<TeamCityModel> message)
        {
            _subject.OnNext(message.Content);
        }

        private void SendNotification(IList<TeamCityModel> buildStatuses, CancellationTokenSource tokenSource)
        {
            try
            {
                var success = buildStatuses.Count == ExpectedBuildCount &&
                              buildStatuses.All(buildStatus => IsSuccesfullBuild(buildStatus.build));

                var notify = !success;

                var message = success ? BuildSuccessMessage(buildStatuses.First().build) :
                    BuildFailureMessage(buildStatuses.Select(m => m.build));

                //todo add color of the line https://www.hipchat.com/docs/api/method/rooms/message
                //todo Background color for message. One of "yellow", "red", "green", "purple", "gray", or "random". (default: yellow)

                Robot.SendNotification(
                    Robot.Settings.Get("TeamCityNotifyRoomName").Trim(),
                    Robot.Settings.Get("TeamCityHipchatAuthToken").Trim(),
                    message,
                    notify);
            }
            finally
            {
                tokenSource.Cancel();
            }
        }

        private string BuildFailureMessage(IEnumerable<Build> builds)
        {
            var failedBuilds = builds.Where(b => !IsSuccesfullBuild(b)).ToList();

            var build = builds.First();
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendFormat(
                    @"<img src='http://ci.innoveo.com/img/buildStates/buildFailed.png' height='16' width='16'/><strong>Failed</strong> to build {0} branch {1} with build number <a href=""{2}""><strong>{3}</strong></a>. Failed build(s) ",
                    build.projectName, build.branchName, build.buildStatusUrl, build.buildNumber);


            stringBuilder.Append(
                string.Join(", ",
                            failedBuilds.Select(fb => string.Format(@"<a href=""{0}""><strong>{1}</strong></a>", fb.buildStatusUrl, fb.buildName))));

            return stringBuilder.ToString();
        }

        private static bool IsSuccesfullBuild(Build b)
        {
            return b.buildResult.Equals("success", StringComparison.InvariantCultureIgnoreCase);
        }

        private string BuildSuccessMessage(Build build)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendFormat(
                    @"<img src='http://ci.innoveo.com/img/buildStates/buildSuccessful.png' height='16' width='16'/><strong>Successfully</strong> built {0} branch {1} with build number <a href=""{2}""><strong>{3}</strong></a>",
                    build.projectName, build.branchName, build.buildStatusUrl, build.buildNumber);

            return stringBuilder.ToString();
        }

        public override IEnumerable<IPluginSetting> Settings
        {
            get { return _settings; }
        }
    }
}