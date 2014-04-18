namespace Nubot.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IRouter
    {
        Dictionary<string, Func<dynamic, dynamic>> HttpGetRoutes { get; }

        Dictionary<string, Tuple<Type, Func<dynamic, dynamic, dynamic>>> HttpPostRoutes { get; }

        void Get(string path, Func<dynamic, dynamic> action);

        void Post<T>(string path, Func<dynamic, dynamic, dynamic> func);
    }
}