namespace Nubot.Interfaces
{
    using System.Collections.Generic;

    public interface IRobotPlugin
    {
        string Name { get; }

        IEnumerable<string> HelpMessages { get; }

        void Respond(string message);
    }
}