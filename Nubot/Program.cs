namespace Nubot
{
    using Core;
    using Core.Brains;
    using Core.Messaging;
    using Topshelf;

    public class Program
    {
        private const string DefaultRobotName = "Nubot";

        public static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.UseLog4Net("nubot.exe.log4net");

                x.Service<Robot>(s =>
                {
                    s.ConstructUsing(name => new Robot(DefaultRobotName, new Log4NetLogger(), MvvmLightMessenger.Default, new AkavacheBrain()));
                    s.WhenStarted(robot => robot.Start());
                    s.WhenStopped(robot => robot.Stop());
                });

                x.RunAsLocalSystem();

                x.SetDescription(DefaultRobotName);
                x.SetDisplayName(DefaultRobotName);
                x.SetServiceName(DefaultRobotName.ToLower());
            });
        }
    }
}
