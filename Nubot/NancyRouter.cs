namespace Nubot
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Nancy;
    using Nancy.ModelBinding;
    using Nancy.Routing;
    using Nancy.TinyIoc;

    public class NancyRouter : NancyModule
    {
        public NancyRouter()
        {
            var robotRouter = TinyIoCContainer.Current.Resolve<IRouter>();

            foreach (var route in robotRouter.HttpGetRoutes)
            {
                Get[route.Key] = route.Value;
            }

            foreach (var route in robotRouter.HttpPostRoutes)
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