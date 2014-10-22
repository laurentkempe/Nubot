namespace Nubot.Abstractions
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Listeners receive every message from the chat source and decide if they want to act on it.
    /// </summary>
    public abstract class Listener
    {
        private readonly IRobot _robot;
        private readonly Action<Response> _responseAction;
        public string RegexText { protected set; get; }

        protected Listener(IRobot robot, Action<Response> responseAction)
        {
            _robot = robot;
            _responseAction = responseAction;
        }

        protected Func<Message, Match> Matcher { get; set; }

        public bool Call(Message message)
        {
            var match = Matcher(message);
            if (!match.Success) return false;

            _responseAction(new Response(_robot, message, match.Groups.Cast<Group>().Select(g => g.Value).ToArray()));

            return true;
        }
    }
}