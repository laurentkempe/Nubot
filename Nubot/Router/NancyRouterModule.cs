namespace Nubot.Router
{
    using Annotations;
    using Interfaces;
    using Nancy;
    using Nancy.ModelBinding;

    [UsedImplicitly]
    public class NancyRouterModule : NancyModule
    {
        public NancyRouterModule(IRobot robot)
        {
            foreach (var route in robot.Router.HttpRoutes)
            {
                if (route.Key.Method == Route.RouteMethod.Get)
                {
                    Get[route.Key.Path] = route.Value;
                }

                if (route.Key.Method == Route.RouteMethod.Post)
                {
                    var route1 = route;

                    Post[route.Key.Path] = x =>
                    {
                        var action = route1.Value;

                        var result = action(this.Bind());

                        if (result is Interfaces.HttpStatusCode)
                        {
                            var httpStatusCode = (Interfaces.HttpStatusCode)result;

                            return httpStatusCode.ToNancyHttpStatusCode();                            
                        }

                        var stringResult = result as string;
                        
                        if (stringResult != null)
                        {
                            return stringResult;
                        }

                        return null;
                    };
                }
            }
        }
    }
}