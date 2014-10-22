namespace Nubot.Abstractions
{
    using System;

    public interface IEventEmitter
    {
        void On<TModel>(string eventName, Action<IMessage<TModel>> action);

        void Emit<TModel>(string eventName, TModel model);
    }
}