namespace Nubot.Plugins.Samples
{
    using System.ComponentModel.Composition;
    using Abstractions;

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
            Robot.Respond(@"what are the three rules?", msg => msg.Send(string.Join(@"\n", _rules)));
        }
    }
}