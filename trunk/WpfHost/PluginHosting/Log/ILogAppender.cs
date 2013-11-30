using System;

namespace IKriv.PluginHosting.Log
{
    public interface ILogAppender : IDisposable
    {
        void Write(string message);
    }
}
