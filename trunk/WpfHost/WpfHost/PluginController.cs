// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IKriv.WpfHost.Catalogs;
using IKriv.WpfHost.Interfaces;
using Microsoft.Practices.Unity;

namespace IKriv.WpfHost
{
    internal class PluginController : IDisposable
    {
        private readonly IUnityContainer _container;
        private readonly TaskScheduler _scheduler;
        private readonly ErrorHandlingService _errorHandlingService;

        public IEnumerable<PluginCatalogEntry> AvailablePlugins { get; private set; }
        public ObservableCollection<Plugin> LoadedPlugins { get; private set; }

        public PluginController(IUnityContainer container, ErrorHandlingService errorHandlingService)
        {
            _container = container;
            _errorHandlingService = errorHandlingService;
            _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            LoadedPlugins = new ObservableCollection<Plugin>();
        }

        public Task LoadCatalogAcync()
        {
            return Task.Factory.StartNew(LoadCatalog);
        }

        public Task<Plugin> LoadPluginAsync(PluginCatalogEntry info)
        {
            return 
                Task.Factory.StartNew(() => LoadPlugin(info))
                .ContinueWith(
                    loadPluginTask =>
                    {
                        var plugin = loadPluginTask.Result;
                        plugin.CreateView();
                        LoadedPlugins.Add(plugin);
                        return plugin;
                    },
                    _scheduler);
        }

        public void Dispose()
        {
            foreach (var plugin in LoadedPlugins)
            {
                DisposePlugin(plugin);
            }
        }

        public void RemovePlugin(Plugin plugin)
        {
            LoadedPlugins.Remove(plugin);
            DisposePlugin(plugin);
        }

        public IDictionary<Plugin, string[]> GetUnsavedItems()
        {
            return LoadedPlugins
                .AsParallel()
                .Select(plugin=>new { Plugin=plugin, Data=GetUnsavedItems(plugin) })
                .Where(item=>item.Data != null)
                .ToDictionary(item=>item.Plugin, item=>item.Data);
        }

        public string[] GetUnsavedItems(Plugin plugin)
        {
            try
            {
                var service = plugin.GetService<IUnsavedData>();
                if (service == null) return null;
                return service.GetNamesOfUnsavedItems();
            }
            catch (Exception ex)
            {
                _errorHandlingService.LogError("Error getting unsaved data from " + plugin.Title, ex);
                return null;
            }
        }

        private void DisposePlugin(Plugin plugin)
        {
            if (plugin == null) return;
            plugin.Error -= OnPluginError;

            try
            {
                plugin.Dispose();
            }
            catch (Exception ex)
            {
                _errorHandlingService.LogError("Error disposing plugin " + plugin.Title, ex);
            }
        }

        private Plugin LoadPlugin(PluginCatalogEntry info)
        {
            var plugin = _container.Resolve<Plugin>();
            plugin.Error += OnPluginError;

            try
            {
                plugin.Load(info);
            }
            catch (Exception)
            {
                DisposePlugin(plugin);
                throw;
            }

            return plugin;
        }

        private void LoadCatalog()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var catalogFile = Path.Combine(Path.GetDirectoryName(location), "plugins.xml");
            var catalog = new XmlPluginCatalog(catalogFile);
            AvailablePlugins = catalog.GetPluginList();
        }

        private void OnPluginError(object sender, PluginErrorEventArgs args)
        {
            var task = new Task(() => PluginErrorHandler(args));
            task.Start(_scheduler);
        }

        private void PluginErrorHandler(PluginErrorEventArgs args)
        {
            if (args == null) return;
            if (args.Plugin == null) return;

            string title = args.Plugin.Title;
            string message = String.Format("An error occurred in plugin {0}. The plugin tab will be now closed.\r\n{1}\r\n", title, args.Message);

            if (LoadedPlugins.Contains(args.Plugin))
            {
                _errorHandlingService.ShowError(message, args.Exception);
                LoadedPlugins.Remove(args.Plugin);
            }
            else
            {
                _errorHandlingService.LogError(message, args.Exception);
            }

            DisposePlugin(args.Plugin);
        }
    }
}
