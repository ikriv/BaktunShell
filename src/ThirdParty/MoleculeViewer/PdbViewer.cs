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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;
using Microsoft.Win32;

namespace MoleculeViewer
{
    /// <summary>
    /// Provides a transformable view of a molecule as defined in a PDB file.
    /// </summary>
    internal class PdbViewer : ArtifactViewer
    {
        private const string moleculeAnnotationString = "Entire molecule";
        private const string selectionAnnotationString = "Current selection";
        private const string atomAnnotationString = "This atom";
        private const string residueAnnotationString = "This residue";

        private PdbActionType actionType;
        private Molecule molecule;
        private StructureControl structureControl;
        private ResidueControl residueControl;
        private ActionIndicator actionIndicator;
        private DispatcherTimer actionPreviewTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdbViewer"/> class and 
        /// sets up its child controls and other visual elements.
        /// </summary>
        internal PdbViewer()
        {
            Grid layoutGrid = new Grid();
            this.Content = layoutGrid;

            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = GridLength.Auto;
            layoutGrid.RowDefinitions.Add(rowDefinition);

            layoutGrid.RowDefinitions.Add(new RowDefinition());

            this.structureControl = new StructureControl(this);
            this.structureControl.PdbViewer = this;
            this.structureControl.SetValue(Grid.RowSpanProperty, 2);
            layoutGrid.Children.Add(this.structureControl);

            this.residueControl = new ResidueControl();
            this.residueControl.PdbViewer = this;
            this.residueControl.VerticalAlignment = VerticalAlignment.Top;
            layoutGrid.Children.Add(this.residueControl);

            Border strctureCaptureBorder = new Border();
            strctureCaptureBorder.Background = Brushes.Transparent;
            strctureCaptureBorder.SetValue(Grid.RowProperty, 1);
            layoutGrid.Children.Add(strctureCaptureBorder);
            this.structureControl.CaptureElement = strctureCaptureBorder;

            this.actionIndicator = new ActionIndicator();
            this.actionIndicator.HorizontalAlignment = HorizontalAlignment.Left;
            this.actionIndicator.VerticalAlignment = VerticalAlignment.Bottom;
            this.actionIndicator.Margin = new Thickness(10);
            this.actionIndicator.IsHitTestVisible = false;
            this.actionIndicator.SetValue(Grid.RowSpanProperty, 2);
            layoutGrid.Children.Add(this.actionIndicator);

            this.actionPreviewTimer = new DispatcherTimer();
            this.actionPreviewTimer.Interval = TimeSpan.FromMilliseconds(100);
            this.actionPreviewTimer.Tick += this.ActionPreviewTimerTick;
        }

        /// <summary>
        /// Gets or sets the type of the action to perform on the view.
        /// </summary>
        /// <value>The type of the action.</value>
        internal PdbActionType ActionType
        {
            get
            {
                return this.actionType;
            }
            set
            {
                this.actionType = value;

                this.actionIndicator.Update(true);

                if (this.actionType == PdbActionType.Zoom ||
                    this.actionType == PdbActionType.Clip ||
                    this.actionType == PdbActionType.Slab)
                {
                    this.actionIndicator.FlashWheel();
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="ViewState"/> of the viewer.
        /// </summary>
        /// <returns>A <see cref="ViewState"/> representing the current view.</returns>
        internal override ViewState GetViewState()
        {
            return this.structureControl.GetViewState();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is working.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is working; otherwise, <c>false</c>.
        /// </value>
        protected override bool IsWorking
        {
            get { return this.ActionType != PdbActionType.None; }
        }

        /// <summary>
        /// Performs hit testing for the displayed molecule.
        /// </summary>
        /// <returns>The active <see cref="HoverObject"/> if any.</returns>
        protected override HoverObject HoverHitTest()
        {
            if (this.IsWorking)
                return null;
            else if (this.residueControl.IsMouseOver)
                return this.residueControl.HoverHitTest();
            else
                return this.structureControl.HoverHitTest();
        }

        /// <summary>
        /// Called when the entity is changed.
        /// </summary>
        protected override void OnEntityChanged()
        {
            if (this.Entity != null)
            {
                using (Stream pdbStream = this.Entity.GetStream())
                {
                    this.molecule = new Molecule(pdbStream);

                    this.structureControl.Molecule = this.molecule;
                    this.residueControl.Molecule = this.molecule;
                }

                this.actionPreviewTimer.Start();
            }
            else
            {
                this.molecule = null;

                this.structureControl.Molecule = null;
                this.residueControl.Molecule = null;

                this.actionPreviewTimer.Stop();
            }
        }

        /// <summary>
        /// Builds the context menu based on the current state of the viewer.
        /// </summary>
        /// <param name="contextMenu">The context menu to apply the menu items to.</param>
        protected override void ConfigureContextMenu(ContextMenu contextMenu)
        {
            if (this.molecule == null) return;

            if (this.molecule.SelectedAtoms.Count > 0)
            {
                MenuItem clearMenuItem = new MenuItem();
                clearMenuItem.Header = "Clear Selection";
                contextMenu.Items.Add(clearMenuItem);

                clearMenuItem.Click += delegate
                {
                    foreach (Atom atom in this.molecule.Atoms)
                        atom.IsSelected = false;
                };

                MenuItem centerMenuItem = new MenuItem();
                centerMenuItem.Header = "Center Selection";
                contextMenu.Items.Add(centerMenuItem);

                centerMenuItem.Click += delegate
                {
                    this.AnimateToViewState(this.molecule.GetSelectionViewState());
                };
            }

            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Reset View";
            contextMenu.Items.Add(menuItem);

            menuItem.Click += delegate
            {
                this.AnimateToViewState(new PdbViewState());
            };

            contextMenu.Items.Add(new Separator());

            menuItem = new MenuItem();
            menuItem.Header = "Show Cartoon";
            menuItem.IsCheckable = true;
            menuItem.IsChecked = this.molecule.ShowCartoon;
            contextMenu.Items.Add(menuItem);

            menuItem.Click += delegate
            {
                this.molecule.ShowCartoon = !this.molecule.ShowCartoon;
            };

            menuItem = new MenuItem();
            menuItem.Header = "Show Backbone";
            menuItem.IsCheckable = true;
            menuItem.IsChecked = this.molecule.ShowBackbone;
            contextMenu.Items.Add(menuItem);

            menuItem.Click += delegate
            {
                this.molecule.ShowBackbone = !this.molecule.ShowBackbone;
            };

            menuItem = new MenuItem();
            menuItem.Header = "Show Full Chain";
            menuItem.IsCheckable = true;
            menuItem.IsChecked = this.molecule.ShowFullChain;
            contextMenu.Items.Add(menuItem);

            menuItem.Click += delegate
            {
                this.molecule.ShowFullChain = !this.molecule.ShowFullChain;
            };

            menuItem = new MenuItem();
            menuItem.Header = "Show Het Atoms";
            menuItem.IsCheckable = true;
            menuItem.IsChecked = this.molecule.ShowHetAtoms;
            contextMenu.Items.Add(menuItem);

            menuItem.Click += delegate
            {
                this.molecule.ShowHetAtoms = !this.molecule.ShowHetAtoms;
            };

            menuItem = new MenuItem();
            menuItem.Header = "Show Waters";
            menuItem.IsCheckable = true;
            menuItem.IsChecked = this.molecule.ShowWaters;
            contextMenu.Items.Add(menuItem);

            menuItem.Click += delegate
            {
                this.molecule.ShowWaters = !this.molecule.ShowWaters;
            };

            MenuItem headerItem = new MenuItem();
            headerItem.Header = "Color scheme";
            contextMenu.Items.Add(headerItem);

            menuItem = new MenuItem();
            menuItem.Header = "Structure";
            menuItem.IsCheckable = true;
            menuItem.IsChecked = this.molecule.ColorScheme == ColorScheme.Structure;
            headerItem.Items.Add(menuItem);

            menuItem.Checked += delegate
            {
                this.molecule.ColorScheme = ColorScheme.Structure;
            };

            menuItem = new MenuItem();
            menuItem.Header = "Atom";
            menuItem.IsCheckable = true;
            menuItem.IsChecked = this.molecule.ColorScheme == ColorScheme.Atom;
            headerItem.Items.Add(menuItem);

            menuItem.Checked += delegate
            {
                this.molecule.ColorScheme = ColorScheme.Atom;
            };

            menuItem = new MenuItem();
            menuItem.Header = "Residue";
            menuItem.IsCheckable = true;
            menuItem.IsChecked = this.molecule.ColorScheme == ColorScheme.Residue;
            headerItem.Items.Add(menuItem);

            menuItem.Checked += delegate
            {
                this.molecule.ColorScheme = ColorScheme.Residue;
            };

            menuItem = new MenuItem();
            menuItem.Header = "Chain";
            menuItem.IsCheckable = true;
            menuItem.IsChecked = this.molecule.ColorScheme == ColorScheme.Chain;
            headerItem.Items.Add(menuItem);

            menuItem.Checked += delegate
            {
                this.molecule.ColorScheme = ColorScheme.Chain;
            };

            menuItem = new MenuItem();
            menuItem.Header = "Temperature";
            menuItem.IsCheckable = true;
            menuItem.IsChecked = this.molecule.ColorScheme == ColorScheme.Temperature;
            headerItem.Items.Add(menuItem);

            menuItem.Checked += delegate
            {
                this.molecule.ColorScheme = ColorScheme.Temperature;
            };
        }

        /// <summary>
        /// Starts the view state animation.
        /// </summary>
        protected override void StartViewStateAnimation()
        {
            this.structureControl.StartViewStateAnimation();
        }

        /// <summary>
        /// Indicates whether the ViewState animation is in progress.
        /// </summary>
        /// <returns>
        /// true if the viewer is animating, otherwise false
        /// </returns>
        protected override bool ContinueViewStateAnimation()
        {
            return this.structureControl.ContinueViewStateAnimation();
        }

        /// <summary>
        /// Stops the view state animation.
        /// </summary>
        protected override void StopViewStateAnimation()
        {
            this.structureControl.StopViewStateAnimation();
        }

        /// <summary>
        /// Sets the view state.
        /// </summary>
        /// <param name="viewState">New <see cref="ViewState"/> to apply to the view.</param>
        protected override void SetViewState(ViewState viewState)
        {
            PdbViewState pdbViewState = viewState as PdbViewState;

            if (pdbViewState != null)
                this.structureControl.SetViewState(pdbViewState);
        }

        private void ActionPreviewTimerTick(object sender, EventArgs e)
        {
            if (this.actionType == PdbActionType.None)
                this.actionIndicator.Update(this.IsMouseOver || this.IsPopupOpen);
        }
    }
}
