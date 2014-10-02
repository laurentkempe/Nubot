namespace Nubot.Plugins.Samples.TeamCity
{
    using System.ComponentModel.Composition;
    using System.Text;
    using Interfaces;
    using Model;

    [Export(typeof(IRobotPlugin))]
    public class TeamCityAggregator : RobotPluginBase
    {
        [ImportingConstructor]
        public TeamCityAggregator(IRobot robot)
            : base("TeamCityAggregator", robot)
        {
            Robot.Messenger.On<TeamCityModel>("TeamCityBuild", OnTeamCityBuild);
        }

        private void OnTeamCityBuild(IMessage<TeamCityModel> message)
        {
            Robot.SendNotification(
                Robot.Settings.Get("TeamCityNotifyRoomName").Trim(),
                Robot.Settings.Get("TeamCityHipchatAuthToken").Trim(),
                BuildMessage(message.Content),
                true);
        }

        private string BuildMessage(TeamCityModel model)
        {
            var stringBuilder = new StringBuilder();

            var buildStatusHtml = model.build.buildStatusHtml.Replace(@"<span class=""tcWebHooksMessage"">", "").Replace("</span>", "");

            stringBuilder
                .Append(buildStatusHtml);

            return stringBuilder.ToString();
        }
    }
}