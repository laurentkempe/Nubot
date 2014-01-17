namespace Nubot
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Configuration;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Composition;
    using Interfaces;
    using Loggers;
    using Microsoft.Owin.Hosting;
    using Nancy.TinyIoc;
    using Settings;

    public class Robot : IRobot
    {
        private readonly CompositionManager _compositionManager;

        public Robot(string name)
        {
            Name = name;
            Version = "1.0"; //todo replace harcoding of the version number

            Settings = new AppSettings();
            Logger = new ConsoleLogger();

            _compositionManager = new CompositionManager(this);
        }

        public string Name { get; private set; }
        public string Version { get; private set; }
        public ISettings Settings { get; private set; }
        public ILogger Logger { get; private set; }

        public void Message(string message)
        {
            Adapter.Message(message);
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
        private List<IRobotPlugin> RobotPlugins { get; set; }

        [Import(AllowRecomposition = true)]
        public IAdapter Adapter { private get; set; }

        public void Start()
        {
            TinyIoCContainer.Current.Register<IRobot>(this);

            _compositionManager.Compose();

            Adapter.Robot = this;

            Adapter.Start();
            StartWebServer();
        }

        private void StartWebServer()
        {
            var url = ConfigurationManager.AppSettings["RobotUrl"];

            using (WebApp.Start<Startup>(url))
            {
                Logger.WriteLine("Running on {0}", url);
                Logger.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }
}