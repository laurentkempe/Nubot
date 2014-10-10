namespace Nubot.Interfaces
{
    using System;

    public interface IMessenger
    {
        void On<TModel>(string eventName, Action<IMessage<TModel>> action);

        void Emit<TModel>(string eventName, TModel model);
    }
}