namespace Nubot.Abstractions
{
    using System;

    public interface IEventEmitter
    {
        void On<TModel>(string eventName, Action<IEventMessage<TModel>> action);

        void Emit<TModel>(string eventName, TModel model);
    }
}