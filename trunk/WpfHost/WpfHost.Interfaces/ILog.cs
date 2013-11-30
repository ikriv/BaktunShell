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
    /// Intefrace for the general logging service
    /// </summary>
    public interface ILog
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Warn(string message, Exception ex);
        void Error(string message);
        void Error(string message, Exception ex);
    }
}
