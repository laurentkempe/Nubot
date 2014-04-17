namespace Nubot.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IRouter
    {
        void Get(string path, Func<dynamic, dynamic> action);

        void Post<T>(string path, Func<dynamic, dynamic, dynamic> action);

        Dictionary<string, Func<dynamic, dynamic>> HttpGetRoutes { get; }

        Dictionary<string, Tuple<Type, Func<dynamic, dynamic, dynamic>>> HttpPostRoutes { get; }
    }
}