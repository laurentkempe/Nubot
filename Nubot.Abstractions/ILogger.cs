namespace Nubot.Abstractions
{
    public interface ILogger
    {
        void WriteLine(string message);

        void WriteLine(string format, params object[] parameters);
    }
}