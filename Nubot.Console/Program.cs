namespace Nubot.Console
{
    using Topshelf;

    public class Program
    {
        private const string DefaultRobotName = "Nubot";

        public static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.UseLog4Net("nubot.console.exe.log4net");

                x.Service<Robot>(s =>
                {
                    s.ConstructUsing(name => new Robot(DefaultRobotName));
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
