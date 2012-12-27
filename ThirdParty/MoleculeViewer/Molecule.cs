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
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MoleculeViewer
{
    /// <summary>
    /// Represents a molecule and is constructed from a PDB file stream.
    /// </summary>
    /// <remarks>
    /// This class contains references to the other business objects that make up 
    /// a molecule and centralizes the functionality to generate the necessary 3D meshes.
    /// </remarks>
    internal class Molecule
    {
        private List<Atom> atoms;
        private List<Residue> residues;
        private List<Chain> chains;
        private List<Structure> structures;
        private List<Ribbon> ribbons;
        private Transform3DGroup moleculeTransformGroup;
        private TranslateTransform3D translateTransform;
        private ScaleTransform3D scaleTransform;
        private bool showCartoon;
        private bool showBackbone;
        private bool showFullChain;
        private bool showHetAtoms;
        private bool showWaters;
        private ColorScheme colorScheme;
        private Model3DGroup model;
        private Model3DGroup annotationMarkerModel;

        /// <summary>
        /// Parses a PDB stream and build the constituent objects.
        /// </summary>
        /// <param name="pdbStream">The PDB stream.</param>
        internal Molecule(Stream pdbStream)
        {
            this.CreateAtomsAndStructures(pdbStream);
            this.CreateBackbone();
            this.CreateBonds();
            this.CreateResidues();
            this.CreateChains();
            this.CreateMoleculeTransform();

            Atom.SetBFactorColors(this.atoms);

            this.SetStructureInfo();
            this.CreateRibbons();

            foreach (Atom atom in this.atoms) atom.Initialize();

            this.ShowCartoon = true;

            this.CreateModel();
        }

        /// <summary>
        /// Allows part of the molecule to hide or show parts related to the cartoon view.
        /// </summary>
        internal event EventHandler ShowCartoonChanged;

        /// <summary>
        /// Allows part of the molecule to hide or show parts related to the backbone view.
        /// </summary>
        internal event EventHandler ShowBackboneChanged;

        /// <summary>
        /// Allows part of the molecule to hide or show parts related to the full chain view.
        /// </summary>
        internal event EventHandler ShowFullChainChanged;

        /// <summary>
        /// Allows part of the molecule to hide or show parts related to het atoms.
        /// </summary>
        internal event EventHandler ShowHetAtomsChanged;

        /// <summary>
        /// Allows part of the molecule to hide or show parts related to waters.
        /// </summary>
        internal event EventHandler ShowWatersChanged;

        /// <summary>
        /// All of the atoms contained in the molecule.
        /// </summary>
        internal List<Atom> Atoms { get { return this.atoms; } }

        /// <summary>
        /// All of the residues (amino acids) contained in the molecule.
        /// </summary>
        internal List<Residue> Residues { get { return this.residues; } }

        /// <summary>
        /// All of the chains of amino acids contained in the molecule.
        /// </summary>
        internal List<Chain> Chains { get { return this.chains; } }

        /// <summary>
        /// The current orientation of the molecule. This is used by a
        /// <see cref="StructureControl"> to translate atom positions from world space to screen
        /// space.</see>
        /// </summary>
        internal Transform3D MoleculeTransform { get { return this.moleculeTransformGroup; } }

        /// <summary>
        /// The Model3D for the molecule which contains all constituent parts. This can be added to
        /// a Visual3D and then diplayed in a Viewport3D.
        /// </summary>
        internal Model3DGroup Model { get { return this.model; } }

        /// <summary>
        /// Determines whether parts of the molecule related to the cartoon view are shown.
        /// </summary>
        internal bool ShowCartoon
        {
            get { return this.showCartoon; }
            set
            {
                if (this.showCartoon != value)
                {
                    this.showCartoon = value;

                    if (this.ShowCartoonChanged != null)
                        this.ShowCartoonChanged(this, EventArgs.Empty);

                    if (this.showCartoon) this.ShowBackbone = false;
                    if (this.showCartoon) this.ShowFullChain = false;
                }
            }
        }

        /// <summary>
        /// Determines whether parts of the molecule related to the backbone view are shown.
        /// </summary>
        internal bool ShowBackbone
        {
            get { return this.showBackbone; }
            set
            {
                if (this.showBackbone != value)
                {
                    this.showBackbone = value;

                    if (this.ShowBackboneChanged != null)
                        this.ShowBackboneChanged(this, EventArgs.Empty);

                    if (this.showBackbone) this.ShowCartoon = false;
                    if (this.showBackbone) this.ShowFullChain = false;
                }
            }
        }

        /// <summary>
        /// Determines whether parts of the molecule related to the full chain view are shown.
        /// </summary>
        internal bool ShowFullChain
        {
            get { return this.showFullChain; }
            set
            {
                if (this.showFullChain != value)
                {
                    this.showFullChain = value;

                    if (this.ShowFullChainChanged != null)
                        this.ShowFullChainChanged(this, EventArgs.Empty);

                    if (this.showFullChain) this.ShowBackbone = false;
                    if (this.showFullChain) this.ShowCartoon = false;
                }
            }
        }

        /// <summary>
        /// Determines whether parts of the molecule related to het atoms are shown.
        /// </summary>
        internal bool ShowHetAtoms
        {
            get { return this.showHetAtoms; }
            set
            {
                if (this.showHetAtoms != value)
                {
                    this.showHetAtoms = value;

                    if (this.ShowHetAtomsChanged != null)
                        this.ShowHetAtomsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Determines whether parts of the molecule related to waters are shown.
        /// </summary>
        internal bool ShowWaters
        {
            get { return this.showWaters; }
            set
            {
                if (this.showWaters != value)
                {
                    this.showWaters = value;

                    if (this.ShowWatersChanged != null)
                        this.ShowWatersChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// The current molecule coloring method.
        /// </summary>
        internal ColorScheme ColorScheme
        {
            get { return this.colorScheme; }
            set
            {
                if (this.colorScheme != value)
                {
                    this.colorScheme = value;

                    foreach (Residue residue in this.residues)
                        residue.ColorScheme = this.colorScheme;
                    foreach (Atom atom in this.atoms)
                        atom.ColorScheme = this.colorScheme;
                }
            }
        }

        /// <summary>
        /// Provides a list of all currently selected atoms.
        /// </summary>
        internal List<Atom> SelectedAtoms
        {
            get
            {
                List<Atom> selectedAtoms = new List<Atom>();

                foreach (Atom atom in this.atoms)
                    if (atom.IsSelected) selectedAtoms.Add(atom);

                return selectedAtoms;
            }
        }

        /// <summary>
        /// Calculates a decent orientation of the molecule based on the bounding box of the
        /// currently selected atoms. Used for animating to the center of a selection.
        /// </summary>
        /// <returns>The generated view state representing the desired orientation.</returns>
        internal PdbViewState GetSelectionViewState()
        {
            Vector3D center = new Vector3D();

            foreach (Atom atom in this.SelectedAtoms)
            {
                center.X += atom.Position.X;
                center.Y += atom.Position.Y;
                center.Z += atom.Position.Z;
            }

            center.X /= this.SelectedAtoms.Count;
            center.Y /= this.SelectedAtoms.Count;
            center.Z /= this.SelectedAtoms.Count;

            double radius = 16;

            foreach (Atom atom in this.SelectedAtoms)
            {
                Vector3D vector = new Vector3D(atom.Position.X - center.X,
                    atom.Position.Y - center.Y, atom.Position.Z - center.Z);

                radius = Math.Max(radius, vector.LengthSquared);
            }

            radius = Math.Sqrt(radius);

            center.X += this.translateTransform.OffsetX;
            center.Y += this.translateTransform.OffsetY;
            center.Z += this.translateTransform.OffsetZ;

            double moleculeRadius = 1.25 / this.scaleTransform.ScaleX;

            double scale = 0.8 * moleculeRadius / radius;

            Vector3D cameraVector = new Vector3D(0, 0, 1);

            Vector3D axis = Vector3D.CrossProduct(center, cameraVector);
            double angle = Vector3D.AngleBetween(center, cameraVector);

            PdbViewState pdbViewState = new PdbViewState();

            pdbViewState.Scale = scale;

            pdbViewState.Translation = -center * this.scaleTransform.ScaleX;

            if (axis.LengthSquared > 0)
                pdbViewState.Rotation = new Quaternion(axis, angle);

            return pdbViewState;
        }

        /// <summary>
        /// Called by the contructor to parse the portions of the PDB file related to atoms and
        /// secondary structures.
        /// </summary>
        /// <param name="pdbStream">The PDB stream.</param>
        private void CreateAtomsAndStructures(Stream pdbStream)
        {
            this.atoms = new List<Atom>();
            this.structures = new List<Structure>();

            using (StreamReader pdbReader = new StreamReader(pdbStream))
            {
                string pdbLine = pdbReader.ReadLine();

                while (pdbLine != null)
                {
                    if (pdbLine.StartsWith("ENDMDL")) break;

                    if (pdbLine.StartsWith("HELIX") || pdbLine.StartsWith("SHEET"))
                        this.structures.Add(Structure.CreateStructure(pdbLine));

                    if (pdbLine.StartsWith("ATOM") || pdbLine.StartsWith("HETATM"))
                        this.atoms.Add(Atom.CreateAtom(this, pdbLine));

                    pdbLine = pdbReader.ReadLine();
                }
            }
        }

        /// <summary>
        /// Called by the contructor after creating the <see cref="Atom"/> objects to identify the
        /// backbone atoms and connect them via referneces.
        /// </summary>
        private void CreateBackbone()
        {
            CAlpha previousCAlpha = null;

            foreach (Atom atom in this.atoms)
            {
                CAlpha nextCAlpha = atom as CAlpha;

                if (nextCAlpha != null)
                {
                    if (previousCAlpha != null &&
                        nextCAlpha.ChainIdentifier == previousCAlpha.ChainIdentifier)
                    {
                        previousCAlpha.NextCAlpha = nextCAlpha;
                        nextCAlpha.PreviousCAlpha = previousCAlpha;
                    }

                    previousCAlpha = nextCAlpha;
                }
            }
        }

        /// <summary>
        /// Called by the contructor after creating the <see cref="Atom"/> objects to identify
        /// covalently bonded atoms. Uses a simple distance heuristic of six angstroms.
        /// </summary>
        private void CreateBonds()
        {
            for (int i = 0; i < this.atoms.Count - 1; i++)
            {
                Atom atom1 = this.atoms[i];

                if (atom1 is Water) continue;

                double x1 = atom1.Position.X;
                double y1 = atom1.Position.Y;
                double z1 = atom1.Position.Z;

                for (int j = i + 1; j < this.atoms.Count; j++)
                {
                    Atom atom2 = this.atoms[j];

                    if (atom2 is Water) continue;

                    double distanceSquared = Math.Pow(x1 - atom2.Position.X, 2);
                    if (distanceSquared > 3.6) continue;

                    distanceSquared += Math.Pow(y1 - atom2.Position.Y, 2);
                    if (distanceSquared > 3.6) continue;

                    distanceSquared += Math.Pow(z1 - atom2.Position.Z, 2);
                    if (distanceSquared > 3.6) continue;

                    double distance = Math.Sqrt(distanceSquared);

                    atom1.Bonds.Add(atom2, distance);
                    atom2.Bonds.Add(atom1, distance);
                }
            }

            Atom.SetBFactorColors(this.atoms);
        }

        /// <summary>
        /// Called by the constructor to create <see cref="Residue"/> objects and group their
        /// constituent atoms.
        /// </summary>
        private void CreateResidues()
        {
            this.residues = new List<Residue>();

            Residue residue = null;

            foreach (Atom atom in this.atoms)
            {
                if (residue == null || atom.ResidueSequenceNumber != residue.ResidueSequenceNumber ||
                    atom.ChainIdentifier != residue.ChainIdentifier)
                {
                    residue = new Residue(this, atom);
                    this.residues.Add(residue);
                }
                else
                {
                    residue.Atoms.Add(atom);
                }

                atom.Residue = residue;
            }
        }

        /// <summary>
        /// Called by the constructor to create <see cref="Chain"/> objects and group their
        /// constituent residues (amino acids).
        /// </summary>
        private void CreateChains()
        {
            this.chains = new List<Chain>();

            Chain chain = null;
            Chain waters = null;

            foreach (Residue residue in this.residues)
            {
                if (residue.ChainIdentifier == "")
                {
                    if (waters == null) waters = new Chain("");

                    waters.Residues.Add(residue);
                    residue.Chain = waters;
                }
                else
                {
                    if (chain == null || residue.ChainIdentifier != chain.ChainIdentifier)
                    {
                        chain = new Chain(residue.ChainIdentifier);
                        this.chains.Add(chain);
                    }

                    chain.Residues.Add(residue);
                    residue.Chain = chain;
                }
            }

            if (waters != null) this.chains.Add(waters);

            Chain.SetChainColors(this.chains);
        }

        /// <summary>
        /// Called by the constructor to calculate the default scale and translation of the
        /// molecule based on the bounding box of the atoms.
        /// </summary>
        private void CreateMoleculeTransform()
        {
            Rect3D bounds = Atom.GetBounds(this.atoms);

            this.moleculeTransformGroup = new Transform3DGroup();

            this.translateTransform = new TranslateTransform3D();
            this.translateTransform.OffsetX = -(bounds.X + bounds.SizeX / 2);
            this.translateTransform.OffsetY = -(bounds.Y + bounds.SizeY / 2);
            this.translateTransform.OffsetZ = -(bounds.Z + bounds.SizeZ / 2);
            this.moleculeTransformGroup.Children.Add(this.translateTransform);

            double scale = 2.5 / Math.Max(bounds.SizeX, Math.Max(bounds.SizeY, bounds.SizeZ));

            this.scaleTransform = new ScaleTransform3D(scale, scale, scale);
            this.moleculeTransformGroup.Children.Add(this.scaleTransform);
        }

        /// <summary>
        /// Called by the constructor to set secondary structure related properties on the residues
        /// and atoms that compose each structure.
        /// </summary>
        private void SetStructureInfo()
        {
            foreach (Atom atom in this.atoms)
                if (atom is ChainAtom) atom.StructureColor = Colors.LightGray;

            foreach (Residue residue in this.residues)
            {
                foreach (Structure structure in this.structures)
                {
                    if (residue.ChainIdentifier == structure.ChainIdentifier &&
                        residue.ResidueSequenceNumber >= structure.StartResidueSequenceNumber &&
                        residue.ResidueSequenceNumber <= structure.EndResidueSequenceNumber)
                    {
                        if (structure is Sheet) residue.IsSheet = true;
                        else if (structure is Helix) residue.IsHelix = true;

                        residue.StructureColor = structure.Color;

                        foreach (Atom atom in residue.Atoms)
                            atom.StructureColor = structure.Color;

                        break;
                    }
                }
            }

            Residue previousResidue = null;

            foreach (Residue residue in this.residues)
            {
                CAlpha cAlpha = null;
                ChainAtom carbonylOxygen = null;

                foreach (Atom atom in residue.Atoms)
                    if (atom is CAlpha)
                        cAlpha = (CAlpha)atom;

                if (cAlpha != null)
                {
                    foreach (Atom atom in residue.Atoms)
                        if (atom is ChainAtom && atom.AtomName == "O")
                            carbonylOxygen = (ChainAtom)atom;
                }

                if (cAlpha == null || carbonylOxygen == null)
                {
                    if (previousResidue != null)
                    {
                        previousResidue.IsStructureEnd = true;
                        previousResidue = null;
                    }

                    continue;
                }
                else
                {
                    residue.CAlphaPosition = cAlpha.Position;
                    residue.CarbonylOxygenPosition = carbonylOxygen.Position;
                }

                if (previousResidue != null && previousResidue.Chain != residue.Chain)
                {
                    previousResidue.IsStructureEnd = true;
                    previousResidue = null;
                }

                if (previousResidue != null)
                {
                    previousResidue.NextResidue = residue;
                    residue.PreviousResidue = previousResidue;

                    if (previousResidue.Chain != residue.Chain ||
                        previousResidue.IsSheet != residue.IsSheet ||
                        previousResidue.IsHelix != residue.IsHelix)
                    {
                        previousResidue.IsStructureEnd = true;
                        residue.IsStructureStart = true;
                    }
                }
                else
                {
                    residue.IsStructureStart = true;
                }

                previousResidue = residue;
            }

            if (previousResidue != null)
                previousResidue.IsStructureEnd = true;
        }

        /// <summary>
        /// Called by the constructor to create <see cref="Ribbon"/> objects which are used to
        /// compute the spline curves for secondary struction representations.
        /// </summary>
        private void CreateRibbons()
        {
            this.ribbons = new List<Ribbon>();

            Ribbon currentRibbon = null;
            Residue previousResidue = null;

            foreach (Residue residue in this.residues)
            {
                if (residue.CAlphaPosition == null)
                {
                    currentRibbon = null;
                }
                else 
                {
                    if (currentRibbon == null ||
                        residue.ChainIdentifier != previousResidue.ChainIdentifier)
                    {
                        currentRibbon = new Ribbon();
                        this.ribbons.Add(currentRibbon);
                    }

                    residue.Ribbon = currentRibbon;
                    currentRibbon.Residues.Add(residue);

                    previousResidue = residue;
                }
            }

            foreach (Ribbon ribbon in this.ribbons)
                ribbon.CreateControlPoints();
        }

        /// <summary>
        /// Called by the constructor to create the container Model3DGroup for the molecule's 3D
        /// model.
        /// </summary>
        private void CreateModel()
        {
            this.model = new Model3DGroup();
            this.model.Transform = this.moleculeTransformGroup;

            foreach (Atom atom in this.atoms)
                this.model.Children.Add(atom.Model);

            foreach (Residue residue in this.residues)
                this.model.Children.Add(residue.Model);

            this.annotationMarkerModel = new Model3DGroup();
            this.model.Children.Add(this.annotationMarkerModel);
        }
    }
}
