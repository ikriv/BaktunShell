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

using System.Windows.Media;

namespace MoleculeViewer
{
    /// <summary>
    /// <see cref="Structure "/> subclass for sheet structures.
    /// </summary>
    internal class Sheet : Structure
    {
        /// <summary>
        /// Returns the color to use for sheet structures.
        /// </summary>
        internal override Color Color { get { return Colors.Orange; } }

        /// <summary>
        /// Returns the PDB file sheet structure record column for the chain identifier.
        /// </summary>
        protected override int ChainIdentifierColumn { get { return 21; } }

        /// <summary>
        /// Returns the PDB file sheet structure record column for the starting residue sequence
        /// number.
        /// </summary>
        protected override int StartResidueSequenceNumberColumn { get { return 22; } }

        /// <summary>
        /// Returns the PDB file sheet structure record column for the ending residue sequence
        /// number.
        /// </summary>
        protected override int EndResidueSequenceNumberColumn { get { return 33; } }
    }
}
