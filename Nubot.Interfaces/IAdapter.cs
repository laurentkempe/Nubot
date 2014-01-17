namespace Nubot.Interfaces
{
    public interface IAdapter
    {
        IRobot Robot { get; set; }

        void Start();

        void Message(string message);
    }
}