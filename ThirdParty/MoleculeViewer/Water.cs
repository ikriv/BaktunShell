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

namespace MoleculeViewer
{
    /// <summary>
    /// <see cref="Atom" /> subclass for water atoms.
    /// </summary>
    /// <remarks>
    /// Adds functionality to toggle visibility based on the <see cref="Molecule.ShowWaters" /> property.
    /// </remarks>
    class Water : Atom
    {
        /// <summary>
        /// Attaches an event handedler to <see cref="Molecule.ShowWatersChanged" />.
        /// </summary>
        internal override void Initialize()
        {
            base.Initialize();

            this.Molecule.ShowWatersChanged += this.MoleculeShowWatersChanged;
        }

        /// <summary>
        /// Toggles visibility based on the <see cref="Molecule.ShowWaters" /> property.
        /// </summary>
        /// <param name="sender">The molecule.</param>
        /// <param name="e">Empty event args.</param>
        private void MoleculeShowWatersChanged(object sender, EventArgs e)
        {
            this.RenderAtomModel(this.Molecule.ShowWaters);
        }
    }
}
