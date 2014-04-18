namespace Nubot.Router
{
    using System;
    using Annotations;
    using Interfaces;
    using Nancy;
    using Nancy.ModelBinding;

    [UsedImplicitly]
    public class NancyRouterModule : NancyModule
    {
        public NancyRouterModule(IRobot robot)
        {
            foreach (var route in robot.Router.HttpGetRoutes)
            {
                Get[route.Key] = route.Value;
            }

            foreach (var route in robot.Router.HttpPostRoutes)
            {
                var r = route;

                Post[route.Key] = x =>
                {
                    var modelType = r.Value.Item1;
                    var action = r.Value.Item2;

                    var model = Activator.CreateInstance(modelType);
                    var bindTo = this.BindTo(model);

                    return action(bindTo, x);
                };
            }
        }
    }
}