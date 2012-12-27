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
using System.Windows.Media;

namespace MoleculeViewer
{
    /// <summary>
    /// Container object to group residues by chain and set chain-based temperature colors.
    /// </summary>
    internal class Chain
    {
        private string chainIdentifier;
        private Color chainColor;
        private List<Residue> residues;
      
        /// <summary>
        /// Creates the chain container object.
        /// </summary>
        /// <param name="chainIdentifier"></param>
        internal Chain(string chainIdentifier)
        {
            this.chainIdentifier = chainIdentifier;
            this.residues = new List<Residue>();
        }
        
        /// <summary>
        /// Alphanumeric chain identifier.
        /// </summary>
        internal string ChainIdentifier { get { return this.chainIdentifier; } }

        /// <summary>
        /// Color used for this chain.
        /// </summary>
        internal Color ChainColor { get { return this.chainColor; } }

        /// <summary>
        /// A list of the constituent residues.
        /// </summary>
        internal List<Residue> Residues { get { return this.residues; } }

        /// <summary>
        /// Assigns colors to all the chains for a molecule.
        /// </summary>
        /// <param name="chains">A list of chains.</param>
        internal static void SetChainColors(List<Chain> chains)
        {
            for (int index = 0; index < chains.Count; index++)
            {
                if (chains[index].chainIdentifier == "")
                    chains[index].chainColor = Colors.Red;
                else
                    chains[index].chainColor = GetChainColor(index);
            }
        }

        /// <summary>
        /// Selects one of five chain colors.
        /// </summary>
        /// <param name="index">A chain color index.</param>
        /// <returns>A color.</returns>
        private static Color GetChainColor(int index)
        {
            index = index % 5;

            if (index == 0) return Colors.Blue;
            else if (index == 1) return Colors.Yellow;
            else if (index == 2) return Colors.Green;
            else if (index == 3) return Colors.Orange;
            else return Colors.Purple;
        }
    }
}
