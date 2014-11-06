using System;
using System.Collections.Generic;
using IKriv.WpfHost.Interfaces;

namespace IKriv.PluginHosting.Log
{
    public class LocalLog : MarshalByRefObject, ILog, IDisposable
    {
        private struct AppenderInfo
        {
            public AppenderInfo(ILogAppender appender, LogLevel level)
            {
                Appender = appender;
                Level = level;
            }

            public ILogAppender Appender;
            public LogLevel Level;
        }

        private readonly List<AppenderInfo> _appenders = new List<AppenderInfo>();

        public LocalLog Console(LogLevel level)
        {
            return AddAppender(new ConsoleAppender(), level);
        }

        public LocalLog Trace(string name, LogLevel level)
        {
            return AddAppender(new TraceAppender(name), level);
        }

        public LocalLog File(string name, LogLevel level)
        {
            return AddAppender(new TempFileAppender(name), level);
        }

        public LocalLog AddAppender(ILogAppender appender, LogLevel level)
        {
            if (level != LogLevel.None) _appenders.Add(new AppenderInfo(appender, level));
            return this;
        }

        public void Debug(string message)
        {
            Message(LogLevel.Debug, message);
        }

        public void Info(string message)
        {
            Message(LogLevel.Info, message);
        }

        public void Warn(string message)
        {
            Message(LogLevel.Warning, message);
        }

        public void Warn(string message, Exception ex)
        {
            Message(LogLevel.Warning, message + ". " + ex);
        }

        public void Error(string message)
        {
            Message(LogLevel.Error, message);
        }

        public void Error(string message, Exception ex)
        {
            Message(LogLevel.Error, message + ". " + ex);
        }

        public void Dispose()
        {
            foreach (var appender in _appenders)
            {
                appender.Appender.Dispose();
            }
        }

        public override object InitializeLifetimeService()
        {
            return null; // live forever
        }

        private void Message(LogLevel level, string message)
        {
            foreach (var appender in _appenders)
            {
                if (appender.Level <= level) appender.Appender.Write(level.ToString().ToUpper() + " " + message);
            }
        }
    }
}
