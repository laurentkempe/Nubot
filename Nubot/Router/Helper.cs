namespace Nubot.Router
{
    using System;
    using Nancy.TinyIoc;

    public static class Helper
    {
        private static readonly Lazy<TinyIoCContainer> Container = new Lazy<TinyIoCContainer>(() => new TinyIoCContainer());

        public static TinyIoCContainer GetConfiguredContainer()
        {
            return Container.Value;
        }
    }
}