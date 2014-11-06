// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace IKriv.WpfHost.Catalogs
{
    public class XmlPluginCatalog : IPluginCatalog
    {
        [XmlRoot(ElementName="PluginCatalog", Namespace="http://www.ikriv.com/wpfHosting/plugins")]
        public class XmlPluginData
        {
            [XmlArrayItem(ElementName = "Plugin")]
            public PluginCatalogEntry[] Plugins { get; set; }
        }

        public XmlPluginCatalog()
        {
        }

        public XmlPluginCatalog(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; set; }

        public IEnumerable<PluginCatalogEntry> GetPluginList()
        {
            using (var reader = new StreamReader(FileName))
            {
                var serializer = new XmlSerializer(typeof(XmlPluginData));
                var data = (XmlPluginData)serializer.Deserialize(reader);
                return data.Plugins;
            }
        }
    }
}
