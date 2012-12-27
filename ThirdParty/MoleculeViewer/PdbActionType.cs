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

namespace MoleculeViewer
{
    /// <summary>
    /// Represents the actions that can be applied to a PDB entity
    /// </summary>
    internal enum PdbActionType
    {
        /// <summary>
        /// No action
        /// </summary>
        None,
        /// <summary>
        /// Animation action
        /// </summary>
        Animating,
        /// <summary>
        /// Change residue state
        /// </summary>
        ToggleResidues,
        /// <summary>
        /// Select residues
        /// </summary>
        SelectResidues,
        /// <summary>
        /// De-select residues
        /// </summary>
        DeselectResidues,
        /// <summary>
        /// Apply a rotation transform
        /// </summary>
        Rotate,
        /// <summary>
        /// Zoom the view
        /// </summary>
        Zoom,
        /// <summary>
        /// Show a menu
        /// </summary>
        Menu,
        /// <summary>
        /// Select the pdb
        /// </summary>
        Select,
        /// <summary>
        /// Apply a clip value
        /// </summary>
        Clip,
        /// <summary>
        /// Reset all actions
        /// </summary>
        Reset,
        /// <summary>
        /// De-select the pdb
        /// </summary>
        Deselect,
        /// <summary>
        /// Apply a slab value
        /// </summary>
        Slab,
        /// <summary>
        /// Apply a translate transform
        /// </summary>
        Translate
    }
}
