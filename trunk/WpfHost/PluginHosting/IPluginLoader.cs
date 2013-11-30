// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using IKriv.WpfHost.Interfaces;

namespace IKriv.PluginHosting
{
    public interface IPluginLoader : IDisposable
    {
        IRemotePlugin LoadPlugin(IWpfHost host, PluginStartupInfo startupInfo);
    }
}
