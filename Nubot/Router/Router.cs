namespace Nubot.Router
{
    using System;
    using System.Collections.Generic;
    using Interfaces;

    public class Router : IRouter
    {
        public Router()
        {
            HttpRoutes = new Dictionary<Route, Func<dynamic, dynamic>>();
        }

        public void Get(string path, Func<dynamic, dynamic> func)
        {
            HttpRoutes.Add(new Route {Method = Route.RouteMethod.Get, Path = path}, func);
        }

        public Dictionary<Route, Func<dynamic, dynamic>> HttpRoutes { get; private set; }

        public void Post(string path, Func<dynamic, dynamic> func)
        {
            var route = new Route {Method = Route.RouteMethod.Post, Path = path};

            HttpRoutes.Add(route, func);
        }
    }
}