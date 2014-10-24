namespace Nubot.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Abstractions;
    using Composition;
    using Microsoft.Owin.Hosting;
    using Nancy;
    using Settings;

    public class Robot : IRobot
    {
        private readonly List<Listener> _listeners;
        private readonly CompositionManager _compositionManager;
        private IDisposable _webApp;

        public Robot(string name, ILogger logger, IEventEmitter eventEmitter)
        {
            Name = name;
            Logger = logger;
            EventEmitter = eventEmitter;

            Version = "1.0"; //todo replace harcoding of the version number

            _listeners = new List<Listener>();

            Settings = new AppSettings();

            _compositionManager = new CompositionManager(this);
        }

        public string Name { get; private set; }
        public string Version { get; private set; }
        public ISettings Settings { get; private set; }
        public ILogger Logger { get; private set; }

        public void MessageRoom(string room, params string[] messages)
        {
            var user = new User(null, null, room, null);
            Adapter.Send(new Envelope(new TextMessage(user, string.Join(Environment.NewLine, messages))));
        }

        public void SendNotification(string room, string authToken, string htmlMessage, bool notify = false)
        {
            if (!string.IsNullOrEmpty(htmlMessage))
            {
                Adapter.SendNotification(room, authToken, htmlMessage, notify);
            }
        }

        public void Receive(Message message)
        {
            foreach (var listener in _listeners)
            {
                listener.Call(message);

                if (message.Done)
                {
                    break;
                }
            }
        }

        public void Respond(string regex, Action<Response> action)
        {
            if (_listeners.Exists(p => p.RegexText == regex))
            {
                return;
            }

            _listeners.Add(new TextListener(this, new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase), action));
        }

        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IRobotPlugin> RobotPlugins { get; private set; }

        public IEventEmitter EventEmitter { get; private set; }

        public void ReloadPlugins()
        {
            _compositionManager.Refresh();
        }

        public void ShowHelp(Response msg)
        {
            var messages = RobotPlugins.SelectMany(plugin => plugin.HelpMessages ?? Enumerable.Empty<string>()).OrderBy(s => s);

            var stringBuilder = new StringBuilder();

            foreach (var message in messages)
            {
                stringBuilder.AppendFormat("{0} {1}\n", Settings.Get("RobotName"), message);
            }

            msg.Send(stringBuilder.ToString());
        }

        public void Send(Envelope envelope, string[] messages)
        {
            Adapter.Send(envelope, messages);
        }

        [Import(AllowRecomposition = true)]
        public IAdapter Adapter { private get; set; }

        public void Start()
        {
            _compositionManager.Compose();

            Adapter.Start();

            StartWebServer();
        }

        public void Stop()
        {
            if (_webApp != null)
            {
                _webApp.Dispose();
            }
        }

        private void StartWebServer()
        {
            Helper.GetConfiguredContainer().Register<IRobot>(this); 

            var url = ConfigurationManager.AppSettings["RobotUrl"];

            _webApp = WebApp.Start<Startup>(url);

           Logger.WriteLine("Running on {0}", url);
        }
    }
}