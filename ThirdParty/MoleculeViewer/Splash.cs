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
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//=============================================================================

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace MoleculeViewer
{
    /// <summary>
    /// Provides a popup window to display a splash logo for a specific duration.
    /// </summary>
    internal class Splash : Popup
    {
        private DispatcherTimer closeTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Splash"/> class.
        /// </summary>
        /// <remarks>
        /// Displays the animated splash screen and starts a timer to close the screen.
        /// </remarks>
        public Splash()
        {
            this.AllowsTransparency = true;
            this.Placement = PlacementMode.Center;

            Rect screenRect = new Rect(0, 0, SystemParameters.PrimaryScreenWidth,
                SystemParameters.PrimaryScreenHeight);
            this.PlacementRectangle = screenRect;

            IKLogoAnimated logo = new IKLogoAnimated();
            logo.InitializeComponent();
            this.Child = logo;

            this.closeTimer = new DispatcherTimer();
            this.closeTimer.Interval = TimeSpan.FromSeconds(3);
            this.closeTimer.Tick += this.CloseTimerTick;
            this.closeTimer.Start();
        }

        private void CloseTimerTick(object sender, EventArgs e)
        {
            this.closeTimer.Stop();
            this.IsOpen = false;
        }
    }
}
