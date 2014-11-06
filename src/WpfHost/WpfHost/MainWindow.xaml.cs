// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace IKriv.WpfHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnLoadPluginClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            if (viewModel == null) return;

            var menuItem = sender as MenuItem;
            if (menuItem == null) return;

            var catalogEntry = menuItem.CommandParameter as PluginCatalogEntry;
            if (catalogEntry == null) return;

            viewModel.LoadPluginCommand.Execute(catalogEntry);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var viewModel = DataContext as IDisposable;
            if (viewModel == null) return;
            viewModel.Dispose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Move tab panel 25 pixels to the right
            // Cannot do it through style, because Margin is hard coded in the TabControl template
            var tabPanel = GetChildOfType<TabPanel>(PluginTabs);
            tabPanel.Margin = new Thickness(25,2,2,2);
        }

        private static T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            var count = VisualTreeHelper.GetChildrenCount(depObj);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            if (viewModel == null) return;
            e.Cancel = !viewModel.CanClose();
        }
    }
}
