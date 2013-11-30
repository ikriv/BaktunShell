// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Threading;
using IKriv.PluginHosting.Log;
using IKriv.WpfHost.Interfaces;

namespace IKriv.PluginHosting
{
    /// <summary>
    /// Loads plugins for the host
    /// </summary>
    public class PluginLoader : MarshalByRefObject, IPluginLoader
    {
        private Dispatcher _dispatcher;
        private LocalLog _log;
        private IWpfHost _host;
        private string _name;

        public void Run(string name)
        {
            _name = name;
            _dispatcher = Dispatcher.CurrentDispatcher;

            using (_log = CreateLog())
            {
                try
                {
                    _log.Info("PluginHost running at " + IntPtr.Size * 8 + " bit, CLR version " + Environment.Version);
                    new AssemblyResolver().Setup();
                    AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                    IpcServices.RegisterChannel(name);
                    RegisterObject();
                    SignalReady();
                    Dispatcher.Run();
                }
                catch (Exception ex)
                {
                    ReportFatalError(ex);
                }

                Thread.Sleep(100); // allow any pending remoting operations to finish
                _log.Debug("Shutdown complete");
            }
        }

        public IRemotePlugin LoadPlugin(IWpfHost host, PluginStartupInfo startupInfo)
        {
            _host = host;

            _log.Info(String.Format("LoadPlugin('{0}','{1}')", startupInfo.AssemblyName, startupInfo.MainClass));

            new ProcessMonitor(Dispose).Start(_host.HostProcessId);

            Func<PluginStartupInfo, object> createOnUiThread = LoadPluginOnUiThread;
            var result = _dispatcher.Invoke(createOnUiThread, startupInfo);

            _log.Debug("Returning plugin object to host");

            if (result is Exception)
            {
                _log.Error("Error loading plugin", (Exception)result);
                throw new TargetInvocationException((Exception)result);
            }
            return (IRemotePlugin)result;
        }

        public void Dispose()
        {
            _log.Info("Shutdown requested");

            if (_dispatcher != null)
            {
                _log.Debug("Performing dispatcher shutdown");
                _dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
            }
            else
            {
                _log.Debug("No dispatcher, exiting the process");
                _log.Dispose();
                Environment.Exit(1);
            }
        }

        public override object InitializeLifetimeService()
        {
            return null; // live forever
        }

        private object LoadPluginOnUiThread(PluginStartupInfo startupInfo)
        {
            _log.Debug("Creating plugin on UI thread");

            var assembly = startupInfo.AssemblyName;
            var mainClass = startupInfo.MainClass;

            try
            {
                var obj = PluginCreator.CreatePlugin(startupInfo.AssemblyName, startupInfo.MainClass, _host);
                _log.Debug("Created local plugin class instance");

                var localPlugin = obj as IPlugin;

                if (localPlugin == null)
                {
                    var message = string.Format("Object of type {0} cannot be loaded as plugin " +
                        "because it does not implement IPlugin interface", mainClass);

                    throw new InvalidOperationException(message);
                }

                var remotePlugin = new RemotePlugin(localPlugin);
                _log.Debug("Created plugin control");
                return remotePlugin;
            }
            catch (Exception ex)
            {
                var message = String.Format("Error loading type '{0}' from assembly '{1}'. {2}",
                    mainClass, assembly, ex.Message);

                return new ApplicationException(message, ex);
            }
        }

        private void RegisterObject()
        {
            _log.Debug(String.Format("Listening on ipc://{0}/PluginLoader", _name));
            RemotingServices.Marshal(this, "PluginLoader", typeof(IPluginLoader));
        }

        private void SignalReady()
        {
            var eventName = _name + ".Ready";
            var readyEvent = EventWaitHandle.OpenExisting(eventName);
            readyEvent.Set();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = 
                (e.ExceptionObject as Exception) ??
                new Exception("Unknown error. Exception object is null");

            ReportFatalError(exception);
        }

        private void ReportFatalError(Exception exception)
        {
            _log.Error("Unhandled exception", exception);

            if (_host != null)
            {
                _log.Debug("Reporting fatal error to host");
                _host.ReportFatalError(ExceptionUtil.GetUserMessage(exception), exception.ToString());
            }
            else
            {
                _log.Warn("Host is null, cannot report error to host");
            }

            _log.Info("Exiting the process to prevent 'program stopped working' dialog");
            _log.Dispose(); // flush pending data
            Environment.Exit(2);
        }

        private LocalLog CreateLog()
        {
            return new LocalLog()
                            .Console(LogLevel.Debug)
                            .Trace(_name, LogLevel.Debug)
                            .File(_name, LogLevel.Debug);
        }
    }
}
