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

using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;

namespace MoleculeViewer
{
    public partial class MainWindow : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.Visibility = Visibility.Hidden;

            Splash splash = new Splash();

            splash.Closed += delegate
            {
                this.Visibility = Visibility.Visible;
            };

            splash.IsOpen = true;
        }

        private void OpenMenuItemClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "Sample PDBs";
            openFileDialog.Filter = "PDB File|*.pdb";

            if (openFileDialog.ShowDialog() ?? false)
            {
                LocalEntity localEntity = new LocalEntity(openFileDialog.FileName);

                //this.Title = localEntity.DisplayName;

                this.tsriImage.Visibility = Visibility.Hidden;

                this.artifactControl.OpenArtifact(localEntity);

                this.closeMenuItem.IsEnabled = true;
            }
        }

        private void CloseMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.tsriImage.Visibility = Visibility.Visible;

            this.artifactControl.CloseArtifact();

            this.closeMenuItem.IsEnabled = false;
        }

        private void AboutMenuItemClick(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }
    }
}
