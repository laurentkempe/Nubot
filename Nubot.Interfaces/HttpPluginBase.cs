namespace Nubot.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Nancy;
    using Nancy.Responses.Negotiation;

    public abstract class HttpPluginBase : NancyModule, IRobotPlugin
    {
        protected readonly IRobot Robot;

        protected HttpPluginBase(string pluginName, string modulePath, IRobot robot)
            : base(modulePath)
        {
            Name = pluginName;

            Robot = robot;

            HelpMessages = new List<string>();
        }

        public string Name { get; private set; }

        public IEnumerable<string> HelpMessages { get; private set; }

        public virtual void Respond(string message)
        {
        }

        //public PluginViewRenderer PluginView { get { return new PluginViewRenderer(this); } }


        public virtual IEnumerable<Tuple<string, string>> StaticPaths
        {
            get
            {
                yield return new Tuple<string, string>(ModulePath + "/css", string.Format("plugins{0}/views/css", ModulePath));
                yield return new Tuple<string, string>(ModulePath + "/scripts", string.Format("plugins{0}/views/scripts", ModulePath));
            }
        }
    }

    //public class PluginViewRenderer : NancyModule.ViewRenderer
    //{
    //    private readonly string _modulePath;

    //    public PluginViewRenderer(INancyModule module)
    //        : base(module)
    //    {
    //        _modulePath = module.ModulePath;
    //    }

    //    public new Negotiator this[dynamic model]
    //    {
    //        get { return base[model]; }
    //    }

    //    public new Negotiator this[string viewName]
    //    {
    //        get { return base[string.Format("plugins{0}/views/", _modulePath) + viewName]; }
    //    }

    //    public new Negotiator this[string viewName, dynamic model]
    //    {
    //        get { return base[string.Format("plugins{0}/views/", _modulePath) + viewName, model]; }
    //    }

    //}
}