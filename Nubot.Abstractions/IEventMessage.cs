namespace Nubot.Abstractions
{
    public interface IEventMessage<out T>
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