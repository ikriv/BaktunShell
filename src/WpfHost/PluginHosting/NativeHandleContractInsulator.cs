// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.AddIn.Contract;

namespace IKriv.PluginHosting
{
    internal class NativeHandleContractInsulator : MarshalByRefObject, INativeHandleContract
    {
        private readonly INativeHandleContract _source;

        public NativeHandleContractInsulator(INativeHandleContract source)
        {
            _source = source;    
        }

        public IntPtr GetHandle()
        {
            return _source.GetHandle();
        }

        public int AcquireLifetimeToken()
        {
            return _source.AcquireLifetimeToken();
        }

        public int GetRemoteHashCode()
        {
            return _source.GetRemoteHashCode();
        }

        public IContract QueryContract(string contractIdentifier)
        {
            return _source.QueryContract(contractIdentifier);
        }

        public bool RemoteEquals(IContract contract)
        {
            return _source.RemoteEquals(contract);
        }

        public string RemoteToString()
        {
            return _source.RemoteToString();
        }

        public void RevokeLifetimeToken(int token)
        {
            _source.RevokeLifetimeToken(token);
        }

        public override object InitializeLifetimeService()
        {
            return null; // live forever
        }
    }
}
