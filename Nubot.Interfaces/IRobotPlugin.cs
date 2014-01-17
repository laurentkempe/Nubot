namespace Nubot.Interfaces
{
    public interface IRobotPlugin
    {
        string Name { get; }

        void Respond(string message);
    }
}