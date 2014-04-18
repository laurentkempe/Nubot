namespace Nubot.Router
{
    using System;
    using System.Collections.Generic;
    using Interfaces;

    public class Router : IRouter
    {
        public Router()
        {
            HttpGetRoutes = new Dictionary<string, Func<dynamic, dynamic>>();
            HttpPostRoutes = new Dictionary<string, Tuple<Type, Func<dynamic, dynamic, dynamic>>>();
        }

        public void Get(string path, Func<dynamic, dynamic> action)
        {
            HttpGetRoutes.Add(path, action);
        }

        public Dictionary<string, Func<dynamic, dynamic>> HttpGetRoutes { get; private set; }

        public Dictionary<string, Tuple<Type, Func<dynamic, dynamic, dynamic>>> HttpPostRoutes { get; private set; }

        public void Post<T>(string path, Func<dynamic, dynamic, dynamic> func)
        {
            HttpPostRoutes.Add(path, new Tuple<Type, Func<dynamic, dynamic, dynamic>>(typeof(T), func));
        }
    }
}