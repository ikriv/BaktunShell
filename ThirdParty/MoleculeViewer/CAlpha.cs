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
using System.Windows.Media.Media3D;

namespace MoleculeViewer
{
    /// <summary>
    /// <see cref="Atom "/> subclass for alpha carbon protein backbone atoms. 
    /// </summary>
    /// <remarks>
    /// Adds functionality to manage the atom's backbone 3D model.
    /// </remarks>
    internal class CAlpha : ChainAtom
    {
        private CAlpha previousCAlpha;
        private CAlpha nextCAlpha;
        private Model3DGroup backboneModel;

        /// <summary>
        /// Gets and sets a reference to the previous <see cref="CAlpha" /> in the backbone.
        /// </summary>
        internal CAlpha PreviousCAlpha
        {
            get { return this.previousCAlpha; }
            set { this.previousCAlpha = value; }
        }

        /// <summary>
        /// Gets and sets a reference to the next <see cref="CAlpha" /> in the backbone.
        /// </summary>
        internal CAlpha NextCAlpha
        {
            get { return this.nextCAlpha; }
            set { this.nextCAlpha = value; }
        }

        /// <summary>
        /// Attaches an event handedler to <see cref="Molecule.ShowBackboneChanged" />.
        /// </summary>
        internal override void Initialize()
        {
            base.Initialize();

            this.Molecule.ShowBackboneChanged += this.MoleculeShowBackboneChanged;
        }

        /// <summary>
        /// Performs hit testing for the backbone 3D model.
        /// </summary>
        /// <param name="rayHitTestResult">A 3D mesh hit test result from the WPF visual tree hit
        /// testing framework</param>
        /// <returns>True if the mesh hit belongs to the backbone, otherwise false.</returns>
        internal override bool HoverHitTest(RayMeshGeometry3DHitTestResult rayHitTestResult)
        {
            return (this.backboneModel != null &&
                this.backboneModel.Children.Contains(rayHitTestResult.ModelHit)) ||
                base.HoverHitTest(rayHitTestResult);
        }

        /// <summary>
        /// Toggles visibility based on the <see cref="Molecule.ShowBackbone" /> property.
        /// </summary>
        /// <param name="sender">The molecule.</param>
        /// <param name="e">Empty event args.</param>
        private void MoleculeShowBackboneChanged(object sender, EventArgs e)
        {
            if (this.Molecule.ShowBackbone && this.backboneModel == null)
            {
                this.backboneModel = new Model3DGroup();
                this.Model.Children.Add(this.backboneModel);

                if (this.previousCAlpha != null)
                    this.CreateBackboneBond(this.previousCAlpha);

                if (this.nextCAlpha != null)
                    this.CreateBackboneBond(this.nextCAlpha);
            }
            
            if (this.Molecule.ShowBackbone && !this.Model.Children.Contains(this.backboneModel))
            {
                this.Model.Children.Add(this.backboneModel);
            }
            else if (!this.Molecule.ShowBackbone &&
                this.Model.Children.Contains(this.backboneModel))
            {
                this.Model.Children.Remove(this.backboneModel);
            }
        }

        /// <summary>
        /// Creates the 3D model for the stick that represents backbone segment with another
        /// <see cref="CAlpha" />.
        /// </summary>
        /// <param name="cAlpha"></param>
        private void CreateBackboneBond(CAlpha cAlpha)
        {
            double x = cAlpha.Position.X - this.Position.X;
            double y = cAlpha.Position.Y - this.Position.Y;
            double z = cAlpha.Position.Z - this.Position.Z;

            double distance = Math.Sqrt(x * x + y * y + z * z);

            this.CreateBondStick(this.backboneModel, cAlpha, distance);
        }
    }
}
