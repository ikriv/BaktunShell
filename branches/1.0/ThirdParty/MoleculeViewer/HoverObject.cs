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

using System.Collections.Generic;

namespace MoleculeViewer
{
    /// <summary>
    /// Abstract base class for objects that support changing their visual state when hovered and
    /// displaying <see cref="HoverPopup" /> windows.
    /// </summary>
    internal abstract class HoverObject
    {
        private bool isHovered;

        /// <summary>
        /// Label used for atom tooltips.
        /// </summary>
        internal virtual string DisplayName
        {
            get { return ""; }
        }

        /// <summary>
        /// Gets or sets a boolean indicating whether or not the mouse pointer is currently
        /// hovering over the object.
        /// </summary>
        internal bool IsHovered
        {
            get
            {
                return this.isHovered;
            }
            set
            {
                this.isHovered = value;
                this.OnIsHoveredChanged();
            }
        }

        /// <summary>
        /// Abstract method for subclasses to update their visual appearance based on the hovered
        /// state.
        /// </summary>
        protected abstract void OnIsHoveredChanged();
    }
}
