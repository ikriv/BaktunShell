// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace TestExceptions
{
    /// <summary>
    /// Interaction logic for MainUserControl.xaml
    /// </summary>
    public partial class MainUserControl : UserControl
    {
        public MainUserControl()
        {
            InitializeComponent();
        }

        private void UiThreadException_Click(object sender, RoutedEventArgs e)
        {
            throw new MyException("Test exception");
        }

        private void WorkerThreadException_Click(object sender, RoutedEventArgs e)
        {
            var thread = new Thread(() => { throw new MyException("Test exception"); });
            thread.Start();
        }
    }
}
