// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System.Windows;
using IKriv.WpfHost.Interfaces;

namespace SolarSystem
{
    public class Plugin : PluginBase
    {
        public override FrameworkElement CreateControl()
        {
            return new MainUserControl();
        }
    }
}
