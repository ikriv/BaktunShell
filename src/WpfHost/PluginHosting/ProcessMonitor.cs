// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.Diagnostics;

namespace IKriv.PluginHosting
{
    public class ProcessMonitor
    {
        private readonly Action _onProcessExit;
        private bool _fired;

        public ProcessMonitor(Action onProcessExit)
        {
            if (onProcessExit == null) throw new ArgumentNullException("onProcessExit");
            _onProcessExit = onProcessExit;
        }

        public void Start(int processId)
        {
            Start(Process.GetProcessById(processId));
        }

        public void Start(Process process)
        {
            if (process == null) 
            {
                FireOnce();
                return;
            }

            process.Exited += (sender, args) => FireOnce();
            process.EnableRaisingEvents = true;

            if (process.HasExited) FireOnce();
        }

        private void FireOnce()
        {
            lock (this)
            {
                if (_fired) return;
                _fired = true;
            }

            _onProcessExit();
        }
    }
}
