//=============================================================================
// This file is part of The Scripps Research Institute's C-ME Application built
// by InterKnowlogy.  
//
// Copyright (C) 2006, 2007 Scripps Research Institute / InterKnowlogy, LLC.
// All rights reserved.
//
// For information about this application contact Tim Huckaby at
// TimHuck@InterKnowlogy.com or (760) 930-0075 x201.
//
// THIS CODE AND INFORMATION ARE PROVIDED ""AS IS"" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//=============================================================================

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

namespace MoleculeViewer
{
    public partial class AboutWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutWindow"/> class.
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void AboutWindowLoaded(object sender, RoutedEventArgs e)
        {
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            this.versionLabel.Content = "Version: " + version.ToString();
        }

        private void IKImageMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.OpenLink("http://www.interknowlogy.com");
        }

        private void IKHyperlinkClick(object sender, RoutedEventArgs e)
        {
            this.OpenLink("http://www.interknowlogy.com");
        }

        private void TsriImageMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.OpenLink("http://kuhn.scripps.edu");
        }

        private void TsriHyperlinkClick(object sender, RoutedEventArgs e)
        {
            this.OpenLink("http://kuhn.scripps.edu");
        }

        private void TimHyperlinkClick(object sender, RoutedEventArgs e)
        {
            this.OpenLink("mailto://TimHuck@InterKnowlogy.com");
        }

        private void PeterHyperlinkClick(object sender, RoutedEventArgs e)
        {
            this.OpenLink("mailto://kuhn@scripps.edu");
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void OpenLink(string url)
        {
            Process process = new Process();
            process.StartInfo.FileName = url;
            process.Start();
        }
    }
}
