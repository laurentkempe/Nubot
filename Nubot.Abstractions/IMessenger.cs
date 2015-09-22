namespace Nubot.Abstractions
{
    using System;

    public interface IMessenger
    {
        void Subscribe<TModel>(string eventName, Action<IMessage<TModel>> action);

        void Publish<TModel>(string eventName, TModel model);
    }
}