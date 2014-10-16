namespace Nubot.Abstractions
{
    using System;
    using System.Text.RegularExpressions;

    public class TextListener : Listener
    {
        public TextListener(IRobot robot, Regex regex, Action<Response> responseAction)
            : base(robot, responseAction)
        {
            Matcher = msg  =>
            {
                var textMessage = msg as TextMessage;
                if (textMessage != null)
                {
                    var match = regex.Match(textMessage.Text);
                    return match;
                }

                return Match.Empty;
            };
        }
    }
}