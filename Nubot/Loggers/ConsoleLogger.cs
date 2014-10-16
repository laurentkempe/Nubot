namespace Nubot.Loggers
{
    using System;
    using Abstractions;

    public class ConsoleLogger : ILogger
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteLine(string format, params object[] parameters)
        {
            Console.WriteLine(format, parameters);
        }
    }
}