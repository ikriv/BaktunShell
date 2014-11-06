// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.Windows;

namespace IKriv.WpfHost.Interfaces
{
    /// <summary>
    /// Recommended (but not requried) base class for user-defined plugins
    /// </summary>
    public abstract class PluginBase : MarshalByRefObject, IPlugin
    {
        public abstract FrameworkElement CreateControl();

        public virtual object GetService(Type serviceType)
        {
            if (serviceType.IsAssignableFrom(GetType())) return this;
            return null;
        }

        public virtual void Dispose()
        {
        }

        public override object InitializeLifetimeService()
        {
            return null; // live forever
        }
    }
}
