using System.Diagnostics;

namespace Inf1nity_Manager.Controls
{
    public class ConsoleTraceListener : TraceListener
    {
        public ConsoleTraceListener(Console console)
        {
            Console = console;
        }

        private Console Console;

        public override void Write(string message)
        {
            WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            Console.Log(message);
        }
    }
}
