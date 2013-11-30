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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using Microsoft.Win32;

namespace MoleculeViewer
{
    /// <summary>
    /// A abstract base control representing a transformable view of an entity.
    /// </summary>
    internal abstract class ArtifactViewer : ContentControl
    {
        private const int hoverPopupDelay = 750;
        private const int viewStateTransitionDuration = 2000;

        private LocalEntity entity;
        private HoverObject hoverObject;
        private HoverObject contextMenuHoverObject;
        private DispatcherTimer hoverTimer;
        private DateTime? hoverStartTime;
        private bool isPopupOpen;
        private DateTime viewStateChangeStartTime;
        private ViewState startViewState;
        private ViewState endViewState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactViewer"/> class.
        /// </summary>
        public ArtifactViewer()
        {
            this.IsEnabled = false;
            this.AllowDrop = true;

            this.MouseLeave += this.ArtifactViewerHoverEvent;
            this.MouseLeftButtonDown += this.ArtifactViewerHoverEvent;
            this.MouseLeftButtonUp += this.ArtifactViewerHoverEvent;
            this.MouseMove += this.ArtifactViewerHoverEvent;
            this.MouseWheel += this.ArtifactViewerHoverEvent;
            this.PreviewMouseRightButtonUp += this.ArtifactViewerPreviewMouseRightButtonUp;

            this.hoverTimer = new DispatcherTimer();
            this.hoverTimer.Interval = TimeSpan.FromMilliseconds(50);
            this.hoverTimer.Tick += this.HoverTimerTick;
        }

        /// <summary>
        /// Gets or sets the entity to display in the viewer.
        /// </summary>
        /// <value>The entity containing the artifact data.</value>
        internal LocalEntity Entity
        {
            get
            {
                return this.entity;
            }
            set
            {
                this.entity = value;
                this.IsEnabled = this.entity != null;

                this.OnEntityChanged();

                this.PerformHoverHitTest();
            }
        }

        /// <summary>
        /// Gets the current <see cref="ViewState"/> of the artifact instance.
        /// </summary>
        /// <returns>The current <see cref="ViewState"/>.</returns>
        internal abstract ViewState GetViewState();

        // TODO: 
        internal void PerformHoverHitTest()
        {
            HoverObject newHoverObject = this.HoverHitTest();

            if (this.hoverObject != newHoverObject)
            {
                if (this.hoverObject != null) this.hoverObject.IsHovered = false;
                this.hoverObject = newHoverObject;
                if (this.hoverObject != null) this.hoverObject.IsHovered = true;
            }

            if (this.hoverObject != null)
            {
                this.hoverStartTime = DateTime.Now;
                this.hoverTimer.Start();
            }
            else
            {
                this.hoverTimer.Stop();
                this.hoverStartTime = null;
            }
        }

        /// <summary>
        /// Changes the view camera and transform settings to the specified <see cref="ViewState"/> 
        /// through an animation.
        /// </summary>
        /// <param name="viewState">The target <see cref="ViewState"/> to animate to.</param>
        internal void AnimateToViewState(ViewState viewState)
        {
            this.StartViewStateAnimation();

            this.startViewState = this.GetViewState();
            this.endViewState = viewState;

            viewStateChangeStartTime = DateTime.Now;

            DispatcherTimer viewStateTimer = new DispatcherTimer();
            viewStateTimer.Interval = TimeSpan.FromMilliseconds(20);
            viewStateTimer.Tick += this.ViewStateTimerTick;
            viewStateTimer.Start();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is working.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is working; otherwise, <c>false</c>.
        /// </value>
        protected abstract bool IsWorking { get; }

        /// <summary>
        /// Gets the <see cref="HoverObject"/> for this artifact instance.
        /// </summary>
        /// <value>The hover object.</value>
        protected HoverObject HoverObject
        {
            get { return this.hoverObject; }
        }

        /// <summary>
        /// Gets the <see cref="HoverObject"/> for this instance's context menu.
        /// </summary>
        /// <value>The context menu's hover object.</value>
        protected HoverObject ContextMenuHoverObject
        {
            get { return this.contextMenuHoverObject; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a popup open.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has a popup open; otherwise, <c>false</c>.
        /// </value>
        protected bool IsPopupOpen
        {
            get { return this.isPopupOpen; }
        }

        // TODO: 
        protected abstract HoverObject HoverHitTest();

        /// <summary>
        /// Called when the entity is changed.
        /// </summary>
        protected abstract void OnEntityChanged();

        /// <summary>
        /// Builds the context menu based on the current state of the viewer.
        /// </summary>
        /// <param name="contextMenu">The context menu to apply the menu items to.</param>
        protected abstract void ConfigureContextMenu(ContextMenu contextMenu);

        /// <summary>
        /// Starts the view state animation.
        /// </summary>
        protected abstract void StartViewStateAnimation();

        /// <summary>
        /// Indicates whether the ViewState animation is in progress.
        /// </summary>
        /// <returns>True if the viewer is animating, otherwise false</returns>
        protected abstract bool ContinueViewStateAnimation();

        /// <summary>
        /// Stops the view state animation.
        /// </summary>
        protected abstract void StopViewStateAnimation();

        /// <summary>
        /// Sets the view state for this viewer instance.
        /// </summary>
        /// <param name="viewState">New <see cref="ViewState"/> to apply to the view.</param>
        protected abstract void SetViewState(ViewState viewState);

        private void ArtifactViewerHoverEvent(object sender, EventArgs e)
        {
            if (!this.isPopupOpen) this.PerformHoverHitTest();
        }

        private void ArtifactViewerPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.isPopupOpen && !this.IsWorking)
            {
                this.hoverTimer.Stop();
                this.hoverStartTime = null;

                this.isPopupOpen = true;

                ContextMenu contextMenu = this.CreateContextMenu();

                if (contextMenu != null)
                {
                    contextMenu.Closed += this.ArtifactContextMenuClosed;
                    contextMenu.IsOpen = true;
                }
            }
        }

        private void HoverTimerTick(object sender, EventArgs e)
        {
            if (!this.isPopupOpen && !this.IsWorking &&
                this.hoverStartTime.Value.AddMilliseconds(PdbViewer.hoverPopupDelay) <
                DateTime.Now)
            {
                this.hoverTimer.Stop();
                this.hoverStartTime = null;

                if (this.hoverObject.DisplayName != "")
                {
                    this.isPopupOpen = true;

                    HoverPopup hoverPopup = new HoverPopup(this.hoverObject, this);
                    hoverPopup.Closed += this.ArtifactContextMenuClosed;
                    hoverPopup.IsOpen = true;
                }
            }
        }

        private void ViewStateTimerTick(object sender, EventArgs e)
        {
            TimeSpan elapsedTimeSpan = DateTime.Now - this.viewStateChangeStartTime;
            double elapsedMilliseconds = elapsedTimeSpan.TotalMilliseconds;

            if (elapsedMilliseconds < ArtifactViewer.viewStateTransitionDuration)
            {
                if (this.ContinueViewStateAnimation())
                {
                    double t = elapsedMilliseconds / ArtifactViewer.viewStateTransitionDuration;

                    this.SetViewState(ViewState.CreateSmoothTween(
                        this.startViewState, this.endViewState, t));
                }
                else
                {
                    ((DispatcherTimer)sender).Stop();
                    this.StopViewStateAnimation();
                }
            }
            else
            {
                if (this.ContinueViewStateAnimation())
                    this.SetViewState(this.endViewState);

                ((DispatcherTimer)sender).Stop();
                this.StopViewStateAnimation();
            }
        }

        private void ArtifactContextMenuClosed(object sender, EventArgs e)
        {
            this.isPopupOpen = false;

            this.PerformHoverHitTest();
        }

        private ContextMenu CreateContextMenu()
        {
            this.contextMenuHoverObject = this.hoverObject;

            ContextMenu contextMenu = new ContextMenu();

            this.ConfigureContextMenu(contextMenu);

            return contextMenu.Items.Count > 0 ? contextMenu : null;
        }
    }
}
