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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MoleculeViewer
{
    /// <summary>
    /// Control to provide user feedback on the current state of mouse and 
    /// keyboard inputs and corresponding actions being executed by the viewer.
    /// </summary>
    public partial class ActionIndicator : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionIndicator"/> class.
        /// </summary>
        public ActionIndicator()
        {
            InitializeComponent();

            this.Update(false);
        }

        /// <summary>
        /// Updates the indicator as active or inactive to match the current keyboard and mouse state.
        /// </summary>
        /// <param name="isActive">if set to <c>true</c>, indicator is active.</param>
        public void Update(bool isActive)
        {
            this.UpdateTextValues(isActive);
            this.UpdateIndicatorFills(isActive);
        }

        /// <summary>
        /// Flashes the control's wheel display.
        /// </summary>
        public void FlashWheel()
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation(
                0.5, 0, new Duration(TimeSpan.FromSeconds(1)));

            AnimationClock opacityClock = opacityAnimation.CreateClock();

            this.mouseFill.ApplyAnimationClock(Path.OpacityProperty, opacityClock);
        }

        private void UpdateTextValues(bool isActive)
        {
            if (isActive && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                this.leftButtonText.Text = "Select";
                this.wheelText.Text = "Clip";
                this.rightButtonText.Text = "Reset";
            }
            else if (isActive && Keyboard.Modifiers == ModifierKeys.Control)
            {
                this.leftButtonText.Text = "Deselect";
                this.wheelText.Text = "Slab";
                this.rightButtonText.Text = "Move";
            }
            else
            {
                this.leftButtonText.Text = "Rotate";
                this.wheelText.Text = "Zoom";
                this.rightButtonText.Text = "Menu";
            }
        }

        private void UpdateIndicatorFills(bool isActive)
        {
            if (isActive && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                this.shiftBorder.Visibility = Visibility.Visible;
            else
                this.shiftBorder.Visibility = Visibility.Hidden;

            if (isActive && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                this.controlBorder.Visibility = Visibility.Visible;
            else
                this.controlBorder.Visibility = Visibility.Hidden;

            if (isActive && Mouse.LeftButton == MouseButtonState.Pressed)
                this.leftButtonFill.Visibility = Visibility.Visible;
            else
                this.leftButtonFill.Visibility = Visibility.Hidden;

            if (isActive && Mouse.RightButton == MouseButtonState.Pressed)
                this.rightButtonFill.Visibility = Visibility.Visible;
            else
                this.rightButtonFill.Visibility = Visibility.Hidden;
        }
    }
}