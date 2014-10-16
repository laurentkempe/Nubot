namespace Nubot.Console
{
    using System.Reflection;
    using Abstractions;
    using log4net;

    public class Log4NetLogger : ILogger
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void WriteLine(string message)
        {
            Logger.Debug(message);
        }

        public void WriteLine(string format, params object[] parameters)
        {
            Logger.DebugFormat(format, parameters);
        }
    }
}