// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BitnessCheck
{
    /// <summary>
    /// Interaction logic for BitnessCheck.xaml
    /// </summary>
    public partial class MainUserControl : UserControl
    {
        int _physicalMemory;
        int _virtualMemory;
        List<byte[]> _memory = new List<byte[]>();

        public MainUserControl()
        {
            InitializeComponent();
            BitnessText.Text = "Running at " + IntPtr.Size*8 + " bit";
            UpdateMemoryText();
        }
         
        private void UpdateMemoryText()
        {
            PhysicalMemoryText.Text = _physicalMemory + " G";
            VirtualMemoryText.Text = _virtualMemory + " G";
        }

        private void Reserve_Click(object sender, RoutedEventArgs e)
        {
            Allocate(Reserve);
        }

        private void Commit_Click(object sender, RoutedEventArgs e)
        {
            Allocate(Commit);
        }

        /* To outstmart .NET optimization and virtual memory mechanism we need to keep in mind these facts
         *
         * 1. It is not possible to allocate more than 2G of memory at a time, even when running as 64 bit
         *
         * 2. If we allocate memory in several independent arrays, they might be immediately garbage collected.
         *    We must reference all arrays from a single data structure, so it can only be garbage collected as a whole.
         * 
         * 3. To have virtual address space actually reserved, we must access at least one byte of the memory allocated.
         *    32-bit process cannot have 3G of virtual address space reserved and exception will be thrown at this point.
         *    
         * 4. If we want pages of physical memory allocated, and not just virtual address space reserved, we must explicitly
         *    access each page, which happens to be 4096 bytes on Intel systems. This is what Commit() method does.
         */
        private void Allocate(Action<byte[]> commitAction)
        {
            try
            {
                const long oneG = 1024L * 1024 * 1024;
                byte[] memory = new byte[oneG];
                commitAction(memory);
                _memory.Add(memory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failure");
            }

            UpdateMemoryText();
        }

        private void Reserve(byte[] b)
        {
            b[0] = 42;
            ++_virtualMemory;
        }


        private void Commit(byte[] b)
        {
            const int PageSize = 4096;

            // index can be int, as currently array size cannot exceed 2G
            for (int index=0; index<b.LongLength; index+=PageSize)
            {
                b[index] = 42; // actual value does not matter
            }

            ++_virtualMemory;
            ++_physicalMemory;
        }
    }
}
