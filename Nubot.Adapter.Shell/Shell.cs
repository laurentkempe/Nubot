namespace Nubot.Adapters
{
    using Interfaces;
    using Nubot.Interfaces.Message;
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

            Console.WriteLine("* 'help' to Show help of all currently loaded plugin(s)");
            Console.WriteLine("* 'list' to List all available adapters");
            Console.WriteLine("* 'chain <adapter>' to Add specified adapter to get involved");
            Console.WriteLine("* 'drop <adapter>' to Drop specified adapter");
            Console.WriteLine();

            var input = string.Empty;
            while (true)
            {
                input = Console.ReadLine();
                Robot.Receive(input);
                Console.WriteLine();
            }
        }

        public override void Message(IMessage<string> message)
        {
            Console.WriteLine(message.Content);
        }
    }
}
