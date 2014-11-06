// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;

namespace IKriv.WpfHost.Interfaces
{
    /// <summary>
    /// Plugins' view of host
    /// </summary>
    public interface IWpfHost : IServiceProvider
    {
        /// <summary>
        /// Reports fatal plugin error to the host; the plugin will be closed
        /// </summary>
        /// <param name="userMessage">Message explaining the nature of the error</param>
        /// <param name="fullExceptionText">Exception call stack as string</param>
        void ReportFatalError(string userMessage, string fullExceptionText);

        /// <summary>
        /// ID of the host process
        /// </summary>
        int HostProcessId { get; }
    }
}
