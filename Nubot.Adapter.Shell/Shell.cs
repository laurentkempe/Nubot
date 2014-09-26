namespace Nubot.Adapters
{
    using Interfaces;
    using System;
    using System.ComponentModel.Composition;

    [Export(typeof(IAdapter)),
    ExportMetadata("Name", "Shell"),
    ExportMetadata("Version", "0.1.0")]

    public class Shell : AdapterBase
    {
        [ImportingConstructor]
        public Shell(IRobot robot)
            : base("Shell", robot)
        {
        }

        public override void Start()
        {
            Console.WriteLine("Starting the shell...");

            var input = string.Empty;
            while (true)
            {
                input = Console.ReadLine();
                Robot.Receive(input);
            }
        }

        public override void Message(string message)
        {
            Console.WriteLine(message);
        }

        public override bool SendNotification(string roomName, string authToken, string htmlMessage, bool notify = false)
        {
            return true;
        }
    }
}
