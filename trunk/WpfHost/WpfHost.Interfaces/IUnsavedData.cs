// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
namespace IKriv.WpfHost.Interfaces
{
    /// <summary>
    /// Interface for plugins that may have unsaved data
    /// </summary>
    public interface IUnsavedData
    {
        /// <summary>
        /// Get list of currently unsaved data items
        /// </summary>
        /// <returns>Array of unsaved data item names</returns>
        string[] GetNamesOfUnsavedItems();
    }
}
