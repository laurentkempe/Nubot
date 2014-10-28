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
    using Nubot.Core.Messaging;

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
            var envelope = new Envelope(new TextMessage(user, string.Join(Environment.NewLine, messages)));
            
            EventEmitter.Emit("Send", envelope);
        }

        public void SendNotification(string room, string authToken, string htmlMessage, bool notify = false)
        {
            if (!string.IsNullOrEmpty(htmlMessage))
            {
                EventEmitter.Emit("SendNotification", new Notification
                {
                    Room = room,
                    AuthToken = authToken,
                    HtmlMessage = htmlMessage,
                    Notify = notify
                });
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

        public IAdapter CurrentAdapter { get; private set; }

        private List<IAdapter> _loadedAdapters = new List<IAdapter>();

        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<Lazy<IAdapter, IAdapterMetadata>> _adapters { get; set; }

        public IEnumerable<IAdapter> RobotAdapters { get { return _loadedAdapters; } }

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
            envelope.AppendMessages = messages;
            EventEmitter.Emit("Send", envelope);
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

            _loadedAdapters.Add(chainAdapter);

            if (!alreadyCreated)
            {
                chainAdapter.Start();
            }
            else
            {
                dropAdapter = chainAdapter;
                EventEmitter.On<Envelope>("Send", OnSendEvent);
                EventEmitter.On<Notification>("SendNotification", OnSendNotificationEvent);
            }
        }

        IAdapter dropAdapter = null;
        public void DropAdapter(string adapterName)
        {
            if (string.IsNullOrEmpty(adapterName) || adapterName.ToLower() == "shell")
            {
                return;
            }
            
            foreach (Lazy<IAdapter, IAdapterMetadata> adapter in _adapters)
            {
                if (adapter.Metadata.Name == adapterName)
                {
                    dropAdapter = adapter.Value;
                    _loadedAdapters.Remove(dropAdapter);
                    break;
                }
            }

            ((IMvvmLightMessenger)EventEmitter).Unregister<IEventMessage<Envelope>>(EventEmitter, OnSendEvent);
            ((IMvvmLightMessenger)EventEmitter).Unregister<IEventMessage<Notification>>(EventEmitter, OnSendNotificationEvent);
        }

        private void OnSendEvent(IEventMessage<Envelope> eventMessage)
        {
            dropAdapter.Send(eventMessage);
        }

        public void OnSendNotificationEvent(IEventMessage<Notification> eventMessage)
        {
            dropAdapter.SendNotification(eventMessage);
        }
    }
}