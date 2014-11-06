using System;

namespace IKriv.PluginHosting.Log
{
    class TraceAppender : ILogAppender
    {
        private readonly string _prefix;

        public TraceAppender(string name)
        {
            _prefix = String.IsNullOrEmpty(name) ? "" : name + ": ";
        }

        public void Write(string message)
        {
            System.Diagnostics.Trace.WriteLine(_prefix + message);
        }

        public void Dispose()
        {
        }
    }
}
