namespace Nubot.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public interface IRobot
    {
        string Name { get; }
        
        string Version { get; }
        
        ISettings Settings { get; }
        
        ILogger Logger { get; }

        void Message(string message);

        void SendNotification(string room, string htmlMessage, bool notify = false);

        void Receive(string body);

        void Respond(string regex, string message, Action<Match> action);

        IEnumerable<IRobotPlugin> RobotPlugins { get; }

        void ReloadPlugins();

        void ShowHelp();
    }
}