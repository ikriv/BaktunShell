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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MoleculeViewer
{
    /// <summary>
    /// Control to render a residue (amino acid) in a molecule, 
    /// applying camera settings and transforms.
    /// </summary>
    internal class ResidueControl : MoleculeViewControl
    {
        private const int itemWidth = 9;
        private const int itemHeight = 12;

        private int firstVisibleColumn;
        private int visibleColumnCount;
        private int totalColumnCount;
        private double? actionMouseStartX;
        private Grid identifierGrid;
        private Grid thumbGrid;
        private Thumb thumb;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResidueControl"/> class and sets up visual elements for rendering.
        /// </summary>
        internal ResidueControl()
        {
            this.Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
            this.SnapsToDevicePixels = true;

            this.SizeChanged += this.ResidueStripSizeChanged;
            this.MouseLeftButtonDown += this.CaptureMouseLeftButtonDown;
            this.MouseLeftButtonUp += this.CaptureMouseLeftButtonUp;
            this.MouseMove += this.CaptureMouseMove;

            this.RowDefinitions.Add(new RowDefinition());
            this.RowDefinitions.Add(new RowDefinition());

            this.identifierGrid = new Grid();
            this.identifierGrid.HorizontalAlignment = HorizontalAlignment.Left;
            this.Children.Add(this.identifierGrid);

            this.identifierGrid.RowDefinitions.Add(new RowDefinition());
            this.identifierGrid.RowDefinitions.Add(new RowDefinition());

            this.thumbGrid = new Grid();
            this.thumbGrid.Background = new SolidColorBrush(Color.FromArgb(128, 64, 64, 64));
            this.thumbGrid.SetValue(Grid.RowProperty, 1);
            this.thumbGrid.Visibility = Visibility.Collapsed;
            this.Children.Add(this.thumbGrid);

            this.thumb = new Thumb();
            this.thumb.Height = 0.5 * ResidueControl.itemHeight;
            this.thumb.HorizontalAlignment = HorizontalAlignment.Left;
            this.thumb.DragDelta += this.ThumbDragDelta;
            this.thumbGrid.Children.Add(this.thumb);

            FrameworkElementFactory visualTree = new FrameworkElementFactory(typeof(Rectangle));
            visualTree.SetValue(Rectangle.FillProperty,
                new SolidColorBrush(Color.FromArgb(128, 192, 192, 192)));
            visualTree.SetValue(Rectangle.RadiusXProperty, this.thumb.Height / 2);
            visualTree.SetValue(Rectangle.RadiusYProperty, this.thumb.Height / 2);

            ControlTemplate thumbTemplate = new ControlTemplate();
            thumbTemplate.VisualTree = visualTree;
            this.thumb.Template = thumbTemplate;
        }

        /// <summary>
        /// Performs hit testing for the control.
        /// </summary>
        /// <returns>The active <see cref="Residue"/> if any.</returns>
        internal HoverObject HoverHitTest()
        {
            foreach (ResidueStripItem residueStripItem in this.identifierGrid.Children)
            {
                if (residueStripItem.Visibility == Visibility.Visible &&
                    residueStripItem.Residue != null && residueStripItem.IsMouseOver)
                {
                    return residueStripItem.Residue;
                }
            }

            return null;
        }

        /// <summary>
        /// Called when the molecule is changed.
        /// </summary>
        protected override void OnMoleculeChanged()
        {
            this.firstVisibleColumn = 0;
            this.totalColumnCount = 0;

            this.identifierGrid.Width = 0;
            this.identifierGrid.ColumnDefinitions.Clear();
            this.identifierGrid.Children.Clear();

            if (Molecule != null)
            {
                this.CreateIdentifiers();

                this.identifierGrid.Width = this.totalColumnCount * ResidueControl.itemWidth;

                this.UpdateSize();
            }
        }

        private void ResidueStripSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateSize();
        }

        private void CaptureMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (ResidueStripItem residueStripItem in this.identifierGrid.Children)
            {
                if (Grid.GetRow(residueStripItem) == 1 && residueStripItem.IsMouseOver)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        this.PdbViewer.ActionType = PdbActionType.SelectResidues;
                    else if (Keyboard.Modifiers == ModifierKeys.Control)
                        this.PdbViewer.ActionType = PdbActionType.DeselectResidues;
                    else
                        this.PdbViewer.ActionType = PdbActionType.ToggleResidues;

                    this.actionMouseStartX = e.GetPosition(this).X;
                    this.identifierGrid.CaptureMouse();

                    break;
                }
            }
        }

        private void CaptureMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (ResidueStripItem residueStripItem in this.identifierGrid.Children)
            {
                residueStripItem.ActionBorder = ResidueStripItem.ActionBorderType.None;

                if (residueStripItem.Residue != null)
                    residueStripItem.Residue.Selection = residueStripItem.Residue.ShowAsSelection;
            }

            this.PdbViewer.ActionType = PdbActionType.None;
            this.actionMouseStartX = null;
            this.identifierGrid.ReleaseMouseCapture();
        }

        private void CaptureMouseMove(object sender, MouseEventArgs e)
        {
            if (this.actionMouseStartX != null)
            {
                double positionX = e.GetPosition(this).X;

                double lowX = Math.Min(this.actionMouseStartX.Value, positionX);
                double highX = Math.Max(this.actionMouseStartX.Value, positionX);

                int lowColumn = Int32.MaxValue;
                int highColumn = Int32.MinValue;

                foreach (ResidueStripItem residueStripItem in this.identifierGrid.Children)
                {
                    if (Grid.GetRow(residueStripItem) == 1 &&
                        residueStripItem.Visibility == Visibility)
                    {
                        int residueColumn = Grid.GetColumn(residueStripItem);
                        double residuePositionX =
                            (residueColumn - this.firstVisibleColumn) * itemWidth;

                        if (residuePositionX + ResidueControl.itemWidth >= lowX &&
                            residuePositionX <= highX)
                        {
                            lowColumn = Math.Min(lowColumn, residueColumn);
                            highColumn = Math.Max(highColumn, residueColumn);
                        }
                    }
                }

                Dictionary<Residue, Residue.SelectionType> showAsStates =
                    new Dictionary<Residue, Residue.SelectionType>();

                foreach (ResidueStripItem residueStripItem in this.identifierGrid.Children)
                {
                    if (residueStripItem.Visibility == Visibility.Visible &&
                        Grid.GetRow(residueStripItem) == 1)
                    {
                        int column = Grid.GetColumn(residueStripItem);

                        if (column == lowColumn && column == highColumn)
                            residueStripItem.ActionBorder =
                                ResidueStripItem.ActionBorderType.Closed;
                        else if (column == lowColumn)
                            residueStripItem.ActionBorder =
                                ResidueStripItem.ActionBorderType.OpenRight;
                        else if (column == highColumn)
                            residueStripItem.ActionBorder =
                                ResidueStripItem.ActionBorderType.OpenLeft;
                        else if (column > lowColumn && column < highColumn)
                            residueStripItem.ActionBorder =
                                ResidueStripItem.ActionBorderType.OpenBoth;
                        else
                            residueStripItem.ActionBorder =
                                ResidueStripItem.ActionBorderType.None;

                        if (residueStripItem.Residue != null)
                        {
                            if (residueStripItem.ActionBorder ==
                                ResidueStripItem.ActionBorderType.None)
                            {
                                if (!showAsStates.ContainsKey(residueStripItem.Residue))
                                    showAsStates.Add(residueStripItem.Residue,
                                        residueStripItem.Residue.Selection);
                            }
                            else
                            {
                                if (this.PdbViewer.ActionType ==
                                    PdbActionType.ToggleResidues &&
                                    residueStripItem.Residue.Selection ==
                                    Residue.SelectionType.Full)
                                    showAsStates[residueStripItem.Residue] =
                                        Residue.SelectionType.None;
                                else if (this.PdbViewer.ActionType ==
                                    PdbActionType.ToggleResidues)
                                    showAsStates[residueStripItem.Residue] =
                                        Residue.SelectionType.Full;
                                else if (this.PdbViewer.ActionType ==
                                    PdbActionType.SelectResidues)
                                    showAsStates[residueStripItem.Residue] =
                                        Residue.SelectionType.Full;
                                else if (this.PdbViewer.ActionType ==
                                    PdbActionType.DeselectResidues)
                                    showAsStates[residueStripItem.Residue] =
                                        Residue.SelectionType.None;
                            }
                        }
                    }
                }

                foreach (Residue residue in showAsStates.Keys)
                    residue.ShowAsSelection = showAsStates[residue];
            }
        }

        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            double thumbLeft = Math.Max(0, Math.Min(ActualWidth - this.thumb.ActualWidth,
                this.thumb.Margin.Left + e.HorizontalChange));
            this.thumb.Margin = new Thickness(thumbLeft, 0, 0, 0);

            this.firstVisibleColumn = (int)Math.Round(
                this.totalColumnCount * thumbLeft / this.ActualWidth);

            this.UpdateItemVisibilities();
        }

        private void CreateIdentifiers()
        {
            foreach (Chain chain in this.Molecule.Chains)
            {
                this.AddResidueStripItem("/", 0, this.totalColumnCount);
                this.AddResidueStripItem("", 1, this.totalColumnCount - 1);

                if (chain.ChainIdentifier.Length == 1)
                {
                    this.AddResidueStripItem(chain.ChainIdentifier, 0, this.totalColumnCount);
                    this.AddResidueStripItem("", 1, this.totalColumnCount - 1);
                }

                this.AddResidueStripItem("/", 0, this.totalColumnCount);
                this.AddResidueStripItem("", 1, this.totalColumnCount - 1);

                bool spaceNeeded = false;
                bool firstResidue = true;

                foreach (Residue residue in chain.Residues)
                {
                    if (spaceNeeded || (!firstResidue && residue.ResidueStripItems.Count > 1))
                        this.AddResidueStripItem("", 1, this.totalColumnCount);

                    foreach (ResidueStripItem residueStripItem in residue.ResidueStripItems)
                        this.AddResidueStripItem(residueStripItem, 1, this.totalColumnCount);

                    spaceNeeded = residue.ResidueStripItems.Count > 1;
                    firstResidue = false;
                }

                foreach (Residue residue in chain.Residues)
                {
                    if (residue.ResidueSequenceNumber % 5 == 1)
                    {
                        int column = Grid.GetColumn(residue.ResidueStripItems[0]);
                        string sequenceNumberString = residue.ResidueSequenceNumber.ToString();

                        if (column + sequenceNumberString.Length > this.totalColumnCount) break;

                        bool showNumber = true;

                        for (int testColumn = column; testColumn < column +
                            sequenceNumberString.Length; testColumn++)
                        {
                            foreach (ResidueStripItem residueStripItem in
                                this.identifierGrid.Children)
                            {
                                if (Grid.GetColumn(residueStripItem) == testColumn &&
                                    (residueStripItem.Label.Content.ToString() == "" ||
                                    Grid.GetRow(residueStripItem) == 0))
                                {
                                    showNumber = false;
                                    break;
                                }
                            }
                        }

                        if (showNumber)
                            foreach (char digit in sequenceNumberString)
                                this.AddResidueStripItem(digit.ToString(), 0, column++);
                    }
                }
            }
        }

        private void AddResidueStripItem(string character, int row, int column)
        {
            this.AddResidueStripItem(new ResidueStripItem(character), row, column);
        }

        private void AddResidueStripItem(ResidueStripItem residueStripItem, int row, int column)
        {
            while (column >= this.totalColumnCount)
            {
                this.identifierGrid.ColumnDefinitions.Add(new ColumnDefinition());
                this.totalColumnCount++;
            }

            residueStripItem.Size = new Size(ResidueControl.itemWidth, ResidueControl.itemHeight);
            residueStripItem.SetValue(Grid.RowProperty, row);
            residueStripItem.SetValue(Grid.ColumnProperty, column);
            this.identifierGrid.Children.Add(residueStripItem);
        }

        private void UpdateSize()
        {
            this.visibleColumnCount = (int)Math.Floor(
                this.ActualWidth / ResidueControl.itemWidth);

            if (this.visibleColumnCount >= this.totalColumnCount)
            {
                this.thumbGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.thumbGrid.Visibility = Visibility.Visible;

                this.thumb.Width = this.ActualWidth *
                    this.visibleColumnCount / this.totalColumnCount;

                this.firstVisibleColumn = Math.Max(0, Math.Min(this.totalColumnCount -
                    this.visibleColumnCount, this.firstVisibleColumn));

                double thumbLeft = this.ActualWidth *
                    this.firstVisibleColumn / this.totalColumnCount;
                this.thumb.Margin = new Thickness(thumbLeft, 0, 0, 0);
            }

            this.UpdateItemVisibilities();
        }

        private void UpdateItemVisibilities()
        {
            double identifierGridLeft = -ResidueControl.itemWidth * this.firstVisibleColumn;
            this.identifierGrid.Margin = new Thickness(
                identifierGridLeft, 0.2 * ResidueControl.itemHeight, 0, 0);

            foreach (ResidueStripItem residueStripItem in this.identifierGrid.Children)
            {
                int column = Grid.GetColumn(residueStripItem);

                bool visible = column >= this.firstVisibleColumn &&
                    column < this.firstVisibleColumn + this.visibleColumnCount;

                residueStripItem.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}
