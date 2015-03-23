namespace Nubot.Abstractions
{
    using System;
    using System.Collections.Generic;

    public interface IRobot
    {
        string Name { get; }

        string Version { get; }

        ISettings Settings { get; }

        ILogger Logger { get; }

        void MessageRoom(string room, params string [] messages);

        void SendNotification(string room, string authToken, string htmlMessage, bool notify = false);

        /// <summary>
        /// Passes the given message to any interested Listeners.
        /// </summary>
        /// <param name="message">A Message instance. Listeners can flag this message as 'done' to prevent further execution.</param>
        void Receive(Message message);

        /// <summary>
        /// Adds a Listener that attempts to match incoming messages directed at the robot based on a Regex.
        /// All regexes treat patterns like they begin with a '^'
        /// </summary>
        /// <param name="regex">A Regex that determines if the callback should be called.</param>
        /// <param name="action">An action that is called with a Response object.</param>
        void Respond(string regex, Action<Response> action);

        IEnumerable<IRobotPlugin> RobotPlugins { get; }

        IEnumerable<IAdapter> RobotAdapters { get; }

        IEventEmitter EventEmitter { get; }

        void ReloadPlugins();

        void ShowHelp(Response msg);

        void Send(Envelope envelope, string[] messages);

        void ChainAdapter(string adapterName);

        void DropAdapter(string adapterName);
    }
}