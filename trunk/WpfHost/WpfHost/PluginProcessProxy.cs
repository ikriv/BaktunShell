// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using IKriv.PluginHosting;
using IKriv.WpfHost.Interfaces;

namespace IKriv.WpfHost
{
    internal class PluginProcessProxy : IDisposable
    {
        private readonly IWpfHost _host;
        private readonly PluginStartupInfo _startupInfo;
        private readonly ErrorHandlingService _errorHandlingService;
        private EventWaitHandle _readyEvent;
        private Process _process;
        private string _name;
        private IPluginLoader _pluginLoader;

        public Process Process { get { return _process; } }
        public IRemotePlugin RemotePlugin { get; private set; }

        public PluginProcessProxy(PluginStartupInfo startupInfo, IWpfHost host, ErrorHandlingService errorHandlingService)
        {
            _startupInfo = startupInfo;
            _host = host;
            _errorHandlingService = errorHandlingService;
        }

        public void Start()
        {
            if (Process != null) throw new InvalidOperationException("Plugin process already started, cannot load more than one plugin per process");
            StartPluginProcess(_startupInfo.FullAssemblyPath);
        }

        public void LoadPlugin()
        {
            if (Process == null) throw new InvalidOperationException("Plugin process not started");
            if (Process.HasExited) throw new InvalidOperationException("Plugin process has terminated unexpectedly");
            
            _pluginLoader = GetPluginLoader();
            RemotePlugin = _pluginLoader.LoadPlugin(_host, _startupInfo);
        }

        public void Dispose()
        {
            if (RemotePlugin != null)
            {
                try
                {
                    RemotePlugin.Dispose();
                }
                catch (Exception ex)
                {
                    _errorHandlingService.LogError("Error disposing remote plugin for " + _startupInfo.Name , ex);
                }
            }

            if (_pluginLoader != null)
            {
                try
                {
                    _pluginLoader.Dispose();
                }
                catch (Exception ex)
                {
                    _errorHandlingService.LogError("Error disposing plugin loader for " + _startupInfo.Name, ex);
                }
            }

            // this can take some time if we have many plugins; should be made asynchronous
            if (Process != null)
            {
                Process.WaitForExit(5000);
                if (!Process.HasExited)
                {
                    _errorHandlingService.LogError("Remote process for " + _startupInfo.Name + " did not exit within timeout period and will be terminated", null);
                    Process.Kill();
                }
            }
        }

        private void StartPluginProcess(string assemblyPath)
        {
            _name = "PluginProcess." + Guid.NewGuid();
            var eventName = _name + ".Ready";
            _readyEvent = new EventWaitHandle(false, EventResetMode.ManualReset, eventName);

            var directory = Path.GetDirectoryName(GetType().Assembly.Location);
            var exeFile = _startupInfo.Bits == 64 ? "PluginProcess64.exe" : "PluginProcess.exe";
            var processName = Path.Combine(directory, exeFile); 

            if (!File.Exists(processName)) throw new InvalidOperationException("Could not find file '" + processName + "'");

            const string quote = "\"";
            const string doubleQuote = "\"\"";

            var quotedAssemblyPath = quote + assemblyPath.Replace(quote, doubleQuote) + quote;
            var createNoWindow = !bool.Parse(ConfigurationManager.AppSettings["PluginProcess.ShowConsole"]);

            var info = new ProcessStartInfo
            {
                Arguments = _name + " " + quotedAssemblyPath,
                CreateNoWindow = createNoWindow,
                UseShellExecute = false,
                FileName = processName
            };

            Trace.WriteLine(info.Arguments);

            _process = Process.Start(info);
        }

        private IPluginLoader GetPluginLoader()
        {
            if (Process.HasExited)
            {
                throw new InvalidOperationException("Plugin process has terminated unexpectedly");
            }

            var timeoutMs = Int32.Parse(ConfigurationManager.AppSettings["PluginProcess.ReadyTimeoutMs"]);

            if (!_readyEvent.WaitOne(timeoutMs))
            {
                throw new InvalidOperationException("Plugin process did not respond within timeout period");
            }

            var hostChannelName = "WpfHost." + Process.GetCurrentProcess().Id;
            IpcServices.RegisterChannel(hostChannelName);

            var url = "ipc://" + _name + "/PluginLoader";
            var pluginLoader = (IPluginLoader)Activator.GetObject(typeof(IPluginLoader), url);
            return pluginLoader;
        }
    }
}
