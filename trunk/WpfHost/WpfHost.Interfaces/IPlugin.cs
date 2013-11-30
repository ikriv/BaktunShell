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
    /// Interface for user-defined plugin
    /// </summary>
    public interface IPlugin : IServiceProvider, IDisposable
    {
        /// <summary>
        /// Creates plugin's visual element; called only ones in plugin's lifetime
        /// </summary>
        /// <returns>WPF framework element of the plugin</returns>
        FrameworkElement CreateControl();
    }
}
