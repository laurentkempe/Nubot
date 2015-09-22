namespace Nubot.Abstractions
{
    using System;

    public interface IMessenger
    {
        void Subscribe<TModel>(string eventName, Action<TModel> action);

        void Publish<TModel>(string eventName, TModel model);
    }
}