namespace Nubot.Interfaces
{
    public interface ILogger
    {
        void WriteLine(string message);

        void WriteLine(string format, params object[] parameters);
    }
}