// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.Windows;
using IKriv.PluginHosting;
using IKriv.WpfHost.Interfaces;

namespace IKriv.WpfHost
{
    class ErrorHandlingService
    {
        private readonly ILog _log;

        public ErrorHandlingService(ILog log)
        {
            _log = log;
        }

        public void ShowError(string message, Exception ex)
        {
            _log.Error(message, ex);

            var text = (ex==null)? message : message + GetSeparator(message) + ExceptionUtil.GetUserMessage(ex);

            var mainWindow = GetMainWindow();
            if (mainWindow == null)
            {
                MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(mainWindow, text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LogError(string message, Exception ex)
        {
            _log.Error(message, ex);
        }

        public bool Confirm(string message)
        {
            return MessageBox.Show(message, "Please confirm", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
        }

        private static Window GetMainWindow()
        {
            if (Application.Current == null) return null;
            return Application.Current.MainWindow;
        }

        private static string GetSeparator(string message)
        {
            if (message.EndsWith(".")) return " "; 
            if (message.EndsWith("\n")) return "";
            return ". ";
        }
    }
}
