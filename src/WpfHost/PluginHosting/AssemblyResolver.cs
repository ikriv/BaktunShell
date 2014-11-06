// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.Reflection;
using IKriv.WpfHost.Interfaces;

namespace IKriv.PluginHosting
{
    class AssemblyResolver
    {
        private string _thisAssemblyName;
        private string _interfacesAssemblyName;
        public void Setup()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            _thisAssemblyName = GetType().Assembly.GetName().Name;
            _interfacesAssemblyName = typeof(IWpfHost).Assembly.GetName().Name;

        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);

            if (name.Name == _thisAssemblyName) return GetType().Assembly;
            if (name.Name == _interfacesAssemblyName) return typeof(IWpfHost).Assembly;

            return null;
        }

    }
}
