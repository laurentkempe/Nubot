namespace Nubot.Nancy
{
    using System;
    using global::Nancy.TinyIoc;

    public static class Helper
    {
        private static readonly Lazy<TinyIoCContainer> Container = new Lazy<TinyIoCContainer>(() => new TinyIoCContainer());

        public static TinyIoCContainer GetConfiguredContainer()
        {
            return Container.Value;
        }
    }
}