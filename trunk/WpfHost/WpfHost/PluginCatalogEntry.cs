// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System.Xml.Serialization;

namespace IKriv.WpfHost
{
    public class PluginCatalogEntry
    {
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public string Version { get; set; }
        [XmlAttribute] public string Description { get; set; }
        [XmlAttribute] public string AssemblyPath { get; set; }
        [XmlAttribute] public string MainClass { get; set; }
        [XmlAttribute] public int Bits { get; set; }
        [XmlAttribute] public string Parameters { get; set; }
    }
}
