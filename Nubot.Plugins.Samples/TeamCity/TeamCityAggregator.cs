namespace Nubot.Plugins.Samples.TeamCity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Text;
    using Interfaces;
    using Model;

    [Export(typeof(IRobotPlugin))]
    public class TeamCityAggregator : RobotPluginBase
    {
        private readonly Subject<TeamCityModel> _subject;

        [ImportingConstructor]
        public TeamCityAggregator(IRobot robot)
            : base("TeamCityAggregator", robot)
        {
            _subject = new Subject<TeamCityModel>();

            //todo We should give a time so that if no answer are recevied the build should be shown as hanging
            _subject
                .GroupBy(model => model.build.buildNumber)
                .SelectMany(grp => grp)
                .Buffer(4)
                .Subscribe(SendNotification);

            Robot.Messenger.On<TeamCityModel>("TeamCityBuild", OnTeamCityBuild);
        }

        private void SendNotification(IList<TeamCityModel> models)
        {
            var message = 
                models.All(model => model.build.buildResult.Equals("success", StringComparison.InvariantCultureIgnoreCase)) ? 
                BuildSuccessMessage(models.First().build) : 
                BuildFailureMessage(models.Select(m => m.build));

            //todo add color of the line https://www.hipchat.com/docs/api/method/rooms/message
            //todo Background color for message. One of "yellow", "red", "green", "purple", "gray", or "random". (default: yellow)

            Robot.SendNotification(
                Robot.Settings.Get("TeamCityNotifyRoomName").Trim(),
                Robot.Settings.Get("TeamCityHipchatAuthToken").Trim(),
                message,
                true);
        }

        private string BuildFailureMessage(IEnumerable<Build> builds)
        {
            var build = builds.First();
            var failedBuilds = builds.Where(b => !b.buildResult.Equals("success", StringComparison.InvariantCultureIgnoreCase)).ToList();

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

        private string BuildSuccessMessage(Build build)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendFormat(
                    @"<img src='http://ci.innoveo.com/img/buildStates/buildSuccessful.png' height='16' width='16'/><strong>Successfully</strong> built {0} branch {1} with build number <a href=""{2}""><strong>{3}</strong></a>",
                    build.projectName, build.branchName, build.buildStatusUrl, build.buildNumber);

            return stringBuilder.ToString();
        }

        private void OnTeamCityBuild(IMessage<TeamCityModel> message)
        {
            _subject.OnNext(message.Content);
        }
    }
}