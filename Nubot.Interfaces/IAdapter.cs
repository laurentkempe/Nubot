namespace Nubot.Interfaces
{
    public interface IAdapter
    {
        void Start();

        void Message(string message);
    }
}