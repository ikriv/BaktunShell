using System;
using System.IO;

namespace IKriv.PluginHosting.Log
{
    class TempFileAppender : ILogAppender
    {
        private readonly string _path;
        private StreamWriter _output;
        private bool _failed;

        public TempFileAppender(string name)
        {
            var tempFolder = Path.GetTempPath();
            var logFolder = Path.Combine(tempFolder, "WpfHost");
            _path = Path.Combine(logFolder, name + ".log");
        }

        public void Write(string message)
        {
            try
            {
                lock (this)
                {
                    if (_output == null && !_failed)
                    {
                        var folder = Path.GetDirectoryName(_path);
                        Directory.CreateDirectory(folder);
                        _output = new StreamWriter(_path, true);
                    }
                }

                if (_output != null)
                {
                    _output.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + message);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Could not write to log file. " + ex);
                _failed = true;
            }
        }

        public void Dispose()
        {
            try
            {
                if (_output != null) _output.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Error closing log. " + ex);
            }
            finally
            {
                _output = null;
            }
        }
    }
}
