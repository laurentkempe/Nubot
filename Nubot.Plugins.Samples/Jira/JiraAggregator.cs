namespace Nubot.Plugins.Samples.Jira
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Abstractions;
    using Models;

    [Export(typeof(IRobotPlugin))]
    public class JiraAggregator : RobotPluginBase
    {
        private string JiraBaseUrl { get { return Robot.Settings.Get("AtlassianJiraUrl"); } }

        private readonly IEnumerable<IPluginSetting> _settings;
        private readonly Subject<JiraModel> _subject;
        private readonly JiraMessageBuilder _jiraMessageBuilder;

        [ImportingConstructor]
        public JiraAggregator(IRobot robot)
            : base("Jira Aggregator", robot)
        {
            _settings = new List<IPluginSetting>
            {
                new PluginSetting(Robot, this, "AtlassianJiraNotifyRoomName"),
                new PluginSetting(Robot, this, "AtlassianJiraHipchatAuthToken")
            };

            _jiraMessageBuilder = new JiraMessageBuilder(JiraBaseUrl);

            _subject = new Subject<JiraModel>();

            var maxWaitDuration = TimeSpan.FromMinutes(3.0);

            _subject
                .GroupBy(model => model.issue.key)
                .SelectMany(grp => grp.Publish(hot => hot.Buffer(() => hot.Throttle(maxWaitDuration, Scheduler))))
                .Subscribe(SendNotification);

            Robot.Messenger.Subscribe<JiraModel>("JiraEvent", OnJiraEvent);
        }

        private void OnJiraEvent(IMessage<JiraModel> message)
        {
            _subject.OnNext(message.Content);
        }

        private void SendNotification(IList<JiraModel> jiraEvents)
        {
            if (!jiraEvents.Any()) return;

            Robot.SendNotification(
                Robot.Settings.Get("AtlassianJiraNotifyRoomName"),
                Robot.Settings.Get("AtlassianJiraHipchatAuthToken"),
                _jiraMessageBuilder.BuildMessage(jiraEvents.ToList()),
                true);
        }

        protected virtual IScheduler Scheduler
        {
            get { return DefaultScheduler.Instance; }
        }
    }
}