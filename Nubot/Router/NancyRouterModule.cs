namespace Nubot.Router
{
    using System;
    using Interfaces;
    using Nancy;
    using Nancy.ModelBinding;

    public class NancyRouterModule : NancyModule
    {
        private readonly IRobot _robot;

        public NancyRouterModule(IRobot robot)
        {
            _robot = robot;

            foreach (var route in _robot.Router.HttpGetRoutes)
            {
                Get[route.Key] = route.Value;
            }

            foreach (var route in _robot.Router.HttpPostRoutes)
            {
                var route1 = route;
                Post[route.Key] = x =>
                {
                    var modelType = route1.Value.Item1;
                    var action = route1.Value.Item2;

                    var model = Activator.CreateInstance(modelType);

                    var bindTo = this.BindTo(model);

                    return action(bindTo, x);
                };
            }
        }
    }
}