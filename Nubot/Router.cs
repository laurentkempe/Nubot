namespace Nubot
{
    using System;
    using System.Collections.Generic;
    using Interfaces;

    public class Router : IRouter
    {
        private readonly Dictionary<string, Func<dynamic, dynamic>> _httpHttpGetHttpGetRoutes;
        private readonly Dictionary<string, Tuple<Type, Func<dynamic, dynamic, dynamic>>> _httpPostRoutes;

        public Router() 
        {
            _httpHttpGetHttpGetRoutes = new Dictionary<string, Func<dynamic, dynamic>>();
            _httpPostRoutes = new Dictionary<string, Tuple<Type, Func<dynamic, dynamic, dynamic>>>();
        }

        public void Post<T>(string path, Func<dynamic, dynamic, dynamic> action)
        {
            _httpPostRoutes.Add(path, new Tuple<Type, Func<dynamic, dynamic, dynamic>>(typeof(T), action));
        }

        public Dictionary<string, Func<dynamic, dynamic>> HttpGetRoutes
        {
            get { return _httpHttpGetHttpGetRoutes; }
        }
        public Dictionary<string, Tuple<Type, Func<dynamic, dynamic, dynamic>>> HttpPostRoutes
        {
            get { return _httpPostRoutes; }
        }

        public void Get(string path, Func<dynamic, dynamic> action)
        {
            _httpHttpGetHttpGetRoutes.Add(path, action);
        }
    }
}