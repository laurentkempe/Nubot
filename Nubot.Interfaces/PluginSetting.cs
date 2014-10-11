namespace Nubot.Interfaces
{
    using System.Linq;

    public class PluginSetting : IPluginSetting
    {
        private readonly IRobot _robot;
        private readonly IRobotPlugin _plug;

        public PluginSetting(IRobot robot, IRobotPlugin plug, string key)
        {
            _robot = robot;
            Key = key;
        }
        public string Key { get; private set; }

        public string Value
        {
            get { return _robot.Settings.Get(Key); }
            set
            {
                _robot.Settings.Set(Key , value);

                // not only Update App.config but also the corresponding config file
                _robot.Settings.Set(Key, value,  _plug.MakeConfigFileName());
            }
        }
    }
}