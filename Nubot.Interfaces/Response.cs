namespace Nubot.Abstractions
{
    /// <summary>
    /// Responses are sent to matching listeners.
    /// Messages know about the content and user that made the original message, and how to reply back to them.
    /// </summary>
    public class Response
    {
        private readonly IRobot _robot;
        private readonly Envelope _envelope;

        public Response(IRobot robot, Message message, string[] match)
        {
            _envelope = new Envelope(message);

            _robot = robot;
            Match = match;
        }

        public string[] Match { get; private set; }

        public void Send(params string[] messages)
        {
            _robot.Send(_envelope, messages);
        }
    }
}