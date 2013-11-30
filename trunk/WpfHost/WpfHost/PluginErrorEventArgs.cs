// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;

namespace IKriv.WpfHost
{
    class PluginErrorEventArgs : EventArgs
    {
        public PluginErrorEventArgs(Plugin plugin, string message, Exception exception)
        {
            Plugin = plugin;
            Message = message;
            Exception = exception;
        }

        public Plugin Plugin { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }
    }
}
