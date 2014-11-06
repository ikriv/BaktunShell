// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;

namespace IKriv.WpfHost.Interfaces
{
    [Serializable]
    public class PluginStartupInfo
    {
        public string FullAssemblyPath { get; set; }
        public string AssemblyName { get; set; }
        public int Bits { get; set; }
        public string MainClass { get; set; }
        public string Name { get; set; }
        public string Parameters { get; set; }
    }
}
