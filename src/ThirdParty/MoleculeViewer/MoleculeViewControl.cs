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
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

namespace MoleculeViewer
{
    /// <summary>
    /// Abstract base control for viewing a part of a molecule in a <see cref="PdbViewer"/>.
    /// </summary>
    internal abstract class MoleculeViewControl : Grid
    {
        private PdbViewer pdbViewer;
        private Molecule molecule;

        /// <summary>
        /// Gets or sets the PDB viewer that owns this control.
        /// </summary>
        /// <value>The PDB viewer.</value>
        internal PdbViewer PdbViewer
        {
            get { return this.pdbViewer; }
            set { this.pdbViewer = value; }
        }

        /// <summary>
        /// Gets or sets the molecule being viewed.
        /// </summary>
        /// <value>The molecule.</value>
        internal Molecule Molecule
        {
            get
            {
                return this.molecule;
            }
            set
            {
                this.molecule = value;
                this.OnMoleculeChanged();
            }
        }

        /// <summary>
        /// Called when the molecule is changed.
        /// </summary>
        protected abstract void OnMoleculeChanged();
    }
}
