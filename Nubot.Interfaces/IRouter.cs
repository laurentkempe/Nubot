namespace Nubot.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IRouter
    {
        Dictionary<Route, Func<dynamic, dynamic>> HttpRoutes { get; }

        void Get(string path, Func<dynamic, dynamic> func);

        void Post(string path, Func<dynamic, dynamic> func);
    }
}