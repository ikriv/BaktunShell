// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.Windows;
using IKriv.WpfHost.Interfaces;

namespace UseLogService
{
    class Plugin : PluginBase, IUnsavedData
    {
        private readonly ILog _log;
        private readonly string _parameter;
        private MainUserControl _control;

        public Plugin(IWpfHost host)
        {
            _log = host.GetService<ILog>();
            var startupInfo = host.GetService<PluginStartupInfo>();
            if (startupInfo != null) _parameter = startupInfo.Parameters;
        }

        public override FrameworkElement CreateControl()
        {
            _control = new MainUserControl { Log = _log };
            _control.Message.Text = _parameter;
            return _control;
        }

        public string[] GetNamesOfUnsavedItems()
        {
            if (_control == null) return null;
            return _control.GetNamesOfUnsavedItems();
        }
    }
}
