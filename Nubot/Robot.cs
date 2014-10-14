namespace Nubot
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Composition;
    using Interfaces;
    using Microsoft.Owin.Hosting;
    using Nancy;
    using Settings;
    using Nubot.Messaging;
    using Nubot.Interfaces.Message;

    public class Robot : IRobot
    {
        private readonly CompositionManager _compositionManager;
        private IDisposable _webApp;

        public Robot(string name, ILogger logger, IMessenger messengerManager)
        {
            Name = name;
            Logger = logger;
            Messenger = messengerManager;

            Version = "1.0"; //todo replace harcoding of the version number

            HelpList = new List<string>();

            Settings = new AppSettings();

            _compositionManager = new CompositionManager(this);
        }

        public string Name { get; private set; }
        public string Version { get; private set; }
        public ISettings Settings { get; private set; }
        public ILogger Logger { get; private set; }
        public List<string> HelpList { get; set; }
        public IAdapter CurrentAdapter { get; private set; }

        public void Message(string message)
        {
            Messenger.Emit("Message", message);
        }

        public void SendNotification(string room, string authToken, string htmlMessage, bool notify = false)
        {
            if (!string.IsNullOrEmpty(htmlMessage))
            {
                Messenger.Emit("SendNotification", new Notification
                {
                    Room = room,
                    AuthToken = authToken,
                    HtmlMessage = htmlMessage,
                    Notify = notify
                });
            }
        }

        public void Receive(string message)
        {
            RobotPlugins.ToList().ForEach(plugin => plugin.Respond(message));
        }

        public void Respond(string regex, string message, Action<Match> action)
        {
            var match = Regex.Match(message, regex);
            if (!match.Success) return;

            action.Invoke(match);
        }

        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IRobotPlugin> RobotPlugins { get; private set; }

        private List<IAdapter> _loadedAdapters = new List<IAdapter>();
        public IEnumerable<IAdapter> RobotAdapters { get { return _loadedAdapters; } }

        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<Lazy<IAdapter, IAdapterMetadata>> _adapters { get; set; }

        public IMessenger Messenger { get; private set; }

        public void ReloadPlugins()
        {
            _compositionManager.Refresh();
        }

        public void AddHelp(params string[] help)
        {
            HelpList = help.Concat(HelpList).ToList();
            HelpList.Sort();
        }

        public void ShowHelp()
        {
            var messages = RobotPlugins.SelectMany(plugin => plugin.HelpMessages ?? Enumerable.Empty<string>()).OrderBy(s => s);

            var stringBuilder = new StringBuilder();

            foreach (var message in messages)
            {
                stringBuilder.AppendFormat("{0} {1}\n", Settings.Get("RobotName"), message);
            }

            Messenger.Emit("Message", stringBuilder.ToString());
        }

        public void Start()
        {
            _compositionManager.Compose();

            StartWebServer();

            StartAdapter();
        }

        private void StartAdapter()
        {
            // let`s say the shell adapter was the default adapter
            // later we can load another adapter through shell command
            IAdapter shell = null;
            foreach (Lazy<IAdapter, IAdapterMetadata> adapter in _adapters)
            {
                if (adapter.Metadata.Name == "Shell")
                {
                    shell = adapter.Value;
                    CurrentAdapter = shell;
                    break;
                }
            }

            if (CurrentAdapter != null)
            {
                _loadedAdapters.Add(CurrentAdapter);
                CurrentAdapter.Start();
            }
            else
            {
                throw new Exception("adapter loading error!");
            }
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

        public void ChainAdapter(string adapterName)
        {
            if (string.IsNullOrEmpty(adapterName) || adapterName.ToLower() == "shell")
            {
                return;
            }

            bool alreadyCreated = false;
            IAdapter chainAdapter = null;
            foreach (Lazy<IAdapter, IAdapterMetadata> adapter in _adapters)
            {
                if (adapter.Metadata.Name == adapterName)
                {
                    alreadyCreated = adapter.IsValueCreated;
                    chainAdapter = adapter.Value;
                    break;
                }
            }

            if (!alreadyCreated)
            {
                _loadedAdapters.Add(chainAdapter);
                chainAdapter.Start();
            }
            else
            {
                Messenger.On<string>("Message", message => { chainAdapter.Message(message); });
                Messenger.On<Notification>("SendNotification", notify => { chainAdapter.SendNotification(notify); });
            }
        }

        public void DropAdapter(string adapterName)
        {
            if (string.IsNullOrEmpty(adapterName) || adapterName.ToLower() == "shell")
            {
                return;
            }

            IAdapter dropAdapter = null;
            foreach (Lazy<IAdapter, IAdapterMetadata> adapter in _adapters)
            {
                if (adapter.Metadata.Name == adapterName)
                {
                    dropAdapter = adapter.Value;
                    _loadedAdapters.Remove(dropAdapter);
                    break;
                }
            }

            // a little bit uncomfortable here
            ((IMvvmLightMessenger)Messenger).Unregister<IMessage<string>>(Messenger, message => { dropAdapter.Message(message); });
            ((IMvvmLightMessenger)Messenger).Unregister<IMessage<Notification>>(Messenger, notify => { dropAdapter.SendNotification(notify); });
        }
    }
}