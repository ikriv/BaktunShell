using System;

namespace IKriv.PluginHosting.Log
{
    class ConsoleAppender : ILogAppender
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }

        public void Dispose()
        {
        }
    }
}
