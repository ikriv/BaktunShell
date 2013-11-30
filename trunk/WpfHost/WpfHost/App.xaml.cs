// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System.Windows;
using IKriv.PluginHosting.Log;
using IKriv.WpfHost.Interfaces;
using Microsoft.Practices.Unity;

namespace IKriv.WpfHost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IUnityContainer _container;

        protected override void OnStartup(StartupEventArgs e)
        {
            _container = new UnityContainer();
            ConfigureContainer();
            _container.Resolve<ILog>().Info("WpfHost starting");

            var mainWindow = _container.Resolve<MainWindow>();
            var viewModel = _container.Resolve<MainViewModel>();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _container.Resolve<ILog>().Info("WpfHost shutting down");
            _container.Dispose();
            base.OnExit(e);
        }

        private void ConfigureContainer()
        {
            _container.RegisterType<ErrorHandlingService>(Singleton());
            _container.RegisterInstance<ILog>(new LocalLog().File("WpfHost", LogLevel.Debug), Singleton());
        }

        private static ContainerControlledLifetimeManager Singleton()
        {
            return new ContainerControlledLifetimeManager();
        }
    }
}
