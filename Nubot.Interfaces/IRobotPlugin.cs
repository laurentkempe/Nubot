namespace Nubot.Interfaces
{
    using System.Collections.Generic;

    public interface IRobotPlugin
    {
        string Name { get; }

        IEnumerable<string> HelpMessages { get; }

        IEnumerable<IPluginSetting> Settings { get; }

        void Respond(string message);

        string ExecutingDirectory { get; }
        string BasePluginsDirectory { get; }
        string MakeConfigFileName();
    }

    public interface IPluginSetting
    {
        string Key { get; }
        string Value { get; set; }
    }
}