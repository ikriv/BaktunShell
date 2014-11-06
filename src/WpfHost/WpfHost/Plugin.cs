// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.AddIn.Pipeline;
using System.IO;
using System.Windows;
using IKriv.PluginHosting;
using IKriv.WpfHost.Interfaces;
using Microsoft.Practices.Unity;

namespace IKriv.WpfHost
{
    internal class Plugin : IServiceProvider, IDisposable
    {
        private readonly IUnityContainer _childContainer;
        private readonly ILog _log;
        private PluginProcessProxy _remoteProcess;
        private PluginStartupInfo _startupInfp;
        private bool _isDisposing;
        private bool _fatalErrorOccurred;

        public PluginCatalogEntry CatalogEntry { get; private set; }
        public FrameworkElement View { get; private set; }
        public string Title { get; private set; }

        public event EventHandler<PluginErrorEventArgs> Error;

        public Plugin(IUnityContainer container, ILog log)
        {
            _log = log;
            _childContainer = container.CreateChildContainer();
        }

        // can be executed on any thread
        public void Load(PluginCatalogEntry catalogEntry)
        {
            if (CatalogEntry != null) throw new InvalidOperationException("Plugin can be loaded only once");

            CatalogEntry = catalogEntry;
            Title = catalogEntry.Name;

            Initialize();

            _log.Debug(String.Format("Loading plugin {0} from {1}, {2}", CatalogEntry.Name, CatalogEntry.AssemblyPath, CatalogEntry.MainClass));

            var host = _childContainer.Resolve<PluginViewOfHost>();
            host.FatalError += OnFatalError;

            _remoteProcess = _childContainer.Resolve<PluginProcessProxy>();

            _log.Debug("Starting plugin process");
            _remoteProcess.Start();
            new ProcessMonitor(OnProcessExited).Start(_remoteProcess.Process);

            _log.Debug("Calling LoadPlugin()");
            _remoteProcess.LoadPlugin();
        }

        // must execute on UI thread
        public void CreateView()
        {
            _log.Debug("Creating plugin view");
            View = FrameworkElementAdapters.ContractToViewAdapter(_remoteProcess.RemotePlugin.Contract);
            _log.Debug("Plugin view created");
        }

        public object GetService(Type serviceType)
        {
            if (_remoteProcess == null) return null;
            return _remoteProcess.RemotePlugin.GetService(serviceType);
        }

        public void Dispose()
        {
            _isDisposing = true;

            try
            {
                var disposableView = View as IDisposable;
                if (disposableView != null) disposableView.Dispose();
            }
            catch (Exception ex)
            {
                ReportError("Error when disposing view", ex);
            }

            _childContainer.Dispose();
        }

        private void Initialize()
        {
            _childContainer.RegisterType<IWpfHost, PluginViewOfHost>(new ContainerControlledLifetimeManager());
            _childContainer.RegisterType<PluginProcessProxy>(new ContainerControlledLifetimeManager());

            _startupInfp = new PluginStartupInfo
            {
                FullAssemblyPath = CatalogEntry.AssemblyPath,
                AssemblyName = Path.GetFileNameWithoutExtension(CatalogEntry.AssemblyPath),
                Bits = CatalogEntry.Bits,
                MainClass = CatalogEntry.MainClass,
                Name = CatalogEntry.Name,
                Parameters = CatalogEntry.Parameters
            };

            _childContainer.RegisterInstance(_startupInfp);
        }

        private void OnProcessExited()
        {
            if (!_isDisposing && !_fatalErrorOccurred )
            {
                ReportError("Plugin process terminated unexpectedly", null);
            }
        }

        private void OnFatalError(Exception ex)
        {
            _fatalErrorOccurred = true;
            ReportError(null, ex);
        }

        private void ReportError(string message, Exception ex)
        {
            if (Error != null)
            {
                Error(this, new PluginErrorEventArgs(this, message, ex));
            }
        }
    }
}
