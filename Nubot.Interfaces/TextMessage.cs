namespace Nubot.Interfaces
{
    public class TextMessage : Message
    {
        public TextMessage(User user, string text)
            : base(user)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}