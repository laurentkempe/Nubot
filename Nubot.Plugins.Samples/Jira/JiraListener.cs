namespace Nubot.Plugins.Samples.Jira
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Abstractions;
    using Models;
    using Nancy;
    using Nancy.ModelBinding;

    [Export(typeof(IRobotPlugin))]
    public class JiraListener : HttpPluginBase
    {
        private readonly IEnumerable<IPluginSetting> _settings;

        [ImportingConstructor]
        public JiraListener(IRobot robot)
            : base("Jira Listener", "/jira", robot)
        {
            _settings = new List<IPluginSetting>
            {
                new PluginSetting(Robot, this, "AtlassianJiraUrl"),
                new PluginSetting(Robot, this, "AtlassianJiraNotifyRoomName"),
                new PluginSetting(Robot, this, "AtlassianJiraHipchatAuthToken")
            };

            Post["/"] = x =>
            {
                var model = this.Bind<JiraModel>(new BindingConfig { IgnoreErrors = true, BodyOnly = true, Overwrite = true });

                Robot.EventEmitter.Emit("JiraEvent", model);

                //Robot.SendNotification(
                //    Robot.Settings.Get("AtlassianJiraNotifyRoomName").Trim(),
                //    Robot.Settings.Get("AtlassianJiraHipchatAuthToken").Trim(),
                //    BuildMessage(model),
                //    true);

                return HttpStatusCode.OK;
            };
        }

        public override IEnumerable<IPluginSetting> Settings
        {
            get { return _settings; }
        }

    }
}