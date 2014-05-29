namespace Nubot.Plugins.Nubot.ViewModel
{
    using System.Collections.Generic;
    using Interfaces;

    public class IndexViewModel
    {
        public string RobotVersion { get; set; }

        public IEnumerable<IRobotPlugin> RobotPlugins { get; set; } 
    }
}