namespace Nubot.Plugins
{
    using System;
    using System.ComponentModel.Composition;
    using Interfaces;

    [Export(typeof(IRobotPlugin))]
    public class Rules : RobotPluginBase
    {
        private readonly string[] _rules = { "1. A robot may not injure a human being or, through inaction, allow a human being to come to harm.",
                                             "2. A robot must obey any orders given to it by human beings, except where such orders would conflict with the First Law.",
                                             "3. A robot must protect its own existence as long as such protection does not conflict with the First or Second Law." };

        [ImportingConstructor]
        public Rules(IRobot robot)
            : base("Rules", robot)
        {
        }

        public override void Respond(string message)
        {
            if (message.Equals("what are the three rules?", StringComparison.InvariantCultureIgnoreCase))
            {
                Robot.Message(string.Join(@"\n", _rules));
            }
        }
    }
}