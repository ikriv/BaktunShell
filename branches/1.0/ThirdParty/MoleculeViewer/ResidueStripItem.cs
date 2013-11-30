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
using System.Windows.Controls;
using System.Windows.Media;

namespace MoleculeViewer
{
    /// <summary>
    /// Visual element representing a residue (amino acid) as a color coded 
    /// single character to be displayed with others in a strip to represent 
    /// an entire molecule.
    /// </summary>
    /// <remarks>
    /// The control supports selection and display of partial 
    /// and full selection and contiguous selection with its neighbors.
    /// </remarks>
    internal class ResidueStripItem : Grid
    {
        /// <summary>
        /// Represents border rendering options for a strip item
        /// </summary>
        internal enum ActionBorderType { None, Closed, OpenBoth, OpenLeft, OpenRight }

        private Residue residue;
        private ActionBorderType actionBorder;
        private Label label;
        private Border border;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResidueStripItem"/> class and sets up visual elements.
        /// </summary>
        /// <param name="character">The character.</param>
        internal ResidueStripItem(string character)
        {
            this.label = new Label();
            this.label.Content = character;
            this.label.HorizontalContentAlignment = HorizontalAlignment.Center;
            this.label.VerticalAlignment = VerticalAlignment.Center;
            this.label.Foreground = Brushes.LightGray;
            this.label.FontFamily = new FontFamily("Lucida Console");
            this.label.Padding = new Thickness(0);

            this.Children.Add(label);

            this.border = new Border();
            this.border.BorderBrush = Brushes.White;
            this.Children.Add(this.border);
        }

        /// <summary>
        /// Gets or sets the residue represented by this control instance.
        /// </summary>
        /// <value>The residue.</value>
        internal Residue Residue
        {
            get { return this.residue; }
            set { this.residue = value; }
        }

        /// <summary>
        /// Gets the label displayed in the strip.
        /// </summary>
        /// <value>The label.</value>
        internal Label Label { get { return this.label; } }

        /// <summary>
        /// Gets or sets the size of the strip item.
        /// </summary>
        /// <value>The size.</value>
        internal Size Size
        {
            get
            {
                return new Size(this.Width, this.Height);
            }
            set
            {
                this.Width = value.Width;
                this.Height = value.Height;

                this.label.FontSize = value.Height;
            }
        }

        /// <summary>
        /// Gets or sets the type of border to display while performing an action that includes this strip item.
        /// </summary>
        /// <value>The type of border.</value>
        internal ActionBorderType ActionBorder
        {
            get
            {
                return this.actionBorder;
            }
            set
            {
                this.actionBorder = value;

                double left = this.actionBorder == ActionBorderType.Closed ||
                    this.actionBorder == ActionBorderType.OpenRight ? 1 : 0;
                double right = this.actionBorder == ActionBorderType.Closed ||
                    this.actionBorder == ActionBorderType.OpenLeft ? 1 : 0;
                double horizontal = this.actionBorder != ActionBorderType.None ? 1 : 0;

                this.border.BorderThickness = new Thickness(left, horizontal, right, horizontal);
            }
        }
    }
}
