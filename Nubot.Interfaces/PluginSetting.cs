namespace Nubot.Interfaces
{
    public class PluginSetting : IPluginSetting
    {
        private readonly IRobot _robot;

        public PluginSetting(IRobot robot, string key)
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
            }
        }
    }
}