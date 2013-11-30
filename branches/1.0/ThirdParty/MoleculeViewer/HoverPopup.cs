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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MoleculeViewer
{
    /// <summary>
    /// Provides a popup control that supports <see cref="HoverObject"/> visual state management.
    /// </summary>
    internal class HoverPopup : Popup
    {
        private const int hideDelay = 500;

        private HoverObject hoverObject;
        private ArtifactViewer artifactViewer;
        private DispatcherTimer awayTimer;
        private DateTime? awayStartTime;
        private Rect? hoverRect;
        private Grid layoutGrid;
        private StackPanel layoutStackPanel;

        /// <summary>
        /// Initializes a new instance of the <see cref="HoverPopup"/> class and sets up its child UI elements.
        /// </summary>
        /// <param name="hoverObject">The hover object.</param>
        /// <param name="artifactViewer">The artifact viewer.</param>
        internal HoverPopup(HoverObject hoverObject, ArtifactViewer artifactViewer)
        {
            this.hoverObject = hoverObject;
            this.artifactViewer = artifactViewer;

            this.StaysOpen = false;
            this.AllowsTransparency = true;
            this.Placement = PlacementMode.Mouse;
            this.PopupAnimation = PopupAnimation.Fade;
            this.MaxWidth = 300;
            this.MouseMove += this.HoverPopupMouseMove;

            this.awayTimer = new DispatcherTimer();
            this.awayTimer.Interval = TimeSpan.FromMilliseconds(50);
            this.awayTimer.Tick += this.AwayTimerTick;

            this.layoutGrid = new Grid();
            this.layoutGrid.Margin = new Thickness(0, 0, 8, 8);
            this.Child = this.layoutGrid;

            DropShadowBitmapEffect dropShadowEffect = new DropShadowBitmapEffect();
            dropShadowEffect.ShadowDepth = 5;
            this.layoutGrid.BitmapEffect = dropShadowEffect;

            Border border = new Border();
            border.Background = Brushes.White;
            border.Padding = new Thickness(5);
            border.CornerRadius = new CornerRadius(5);
            this.layoutGrid.Children.Add(border);

            this.layoutStackPanel = new StackPanel();
            border.Child = this.layoutStackPanel;

            this.AddTitle();
        }

        private void HoverPopupMouseMove(object sender, MouseEventArgs e)
        {
            Point currentPosition = e.GetPosition(this.Child);

            if (this.hoverRect == null)
            {
                Point p1 = new Point(Math.Min(0, currentPosition.X),
                    Math.Min(0, currentPosition.Y));

                Point p2 = new Point(Math.Max(this.layoutGrid.ActualWidth, currentPosition.X),
                    Math.Max(this.layoutGrid.ActualHeight, currentPosition.Y));

                this.hoverRect = new Rect(p1, p2);
            }
            else
            {
                bool mouseIsHovering = this.hoverRect.Value.Contains(currentPosition);

                if (!mouseIsHovering && this.awayStartTime == null)
                {
                    this.awayStartTime = DateTime.Now;
                    this.awayTimer.Start();
                }
                else if (mouseIsHovering)
                {
                    this.awayTimer.Stop();
                    this.awayStartTime = null;
                }
            }
        }

        private void AwayTimerTick(object sender, EventArgs e)
        {
            if (this.awayStartTime.Value.AddMilliseconds(HoverPopup.hideDelay)
                < DateTime.Now)
            {
                this.awayTimer.Stop();
                this.awayStartTime = null;

                this.IsOpen = false;
            }
        }

        private void AddTitle()
        {
            if (hoverObject.DisplayName != "")
            {
                Label label = new Label();
                label.Content = hoverObject.DisplayName;
                label.FontSize = 16;
                label.Padding = new Thickness(0);
                this.layoutStackPanel.Children.Add(label);
            }
        }
    }
}
