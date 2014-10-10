namespace Nubot.Interfaces
{
    public interface IMessage<out T>
    {
        /// <summary>
        /// Gets or sets the message's content.
        /// </summary>
        T Content
        {
            get;
        }
    }
}