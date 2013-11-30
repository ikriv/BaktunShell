// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using IKriv.WpfHost.Interfaces;

namespace IKriv.PluginHosting
{
    internal class RemotePlugin : MarshalByRefObject, IRemotePlugin
    {
        private readonly IPlugin _plugin;
        
        public RemotePlugin(IPlugin plugin)
        {
            _plugin = plugin;
            var control = plugin.CreateControl();
            var localContract = FrameworkElementAdapters.ViewToContractAdapter(control);
            Contract = new NativeHandleContractInsulator(localContract);
        }

        public INativeHandleContract Contract { get; private set; }

        public object GetService(Type serviceType)
        {
            return _plugin.GetService(serviceType);
        }

        public void Dispose()
        {
            _plugin.Dispose();
        }

        public override object InitializeLifetimeService()
        {
            return null; // live forever
        }
    }
}
