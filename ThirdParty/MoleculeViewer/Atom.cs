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
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MoleculeViewer
{
    /// <summary>
    /// Abstract base class for all types of atoms.
    /// </summary>
    /// <remarks>
    /// Handles the majority of atom-related display logic.
    /// </remarks>
    internal abstract class Atom : HoverObject
    {
        private Molecule molecule;
        private string atomName;
        private string residueName;
        private string chainIdentifier;
        private int residueSequenceNumber;
        private Point3D position;
        private double temperatureFactor;
        private Color atomColor;
        private Color structureColor;
        private Color temperatureColor;
        private Dictionary<Atom, double> bonds;
        private Residue residue;
        private Model3DGroup model;
        private TranslateTransform3D translationTransform;
        private DiffuseMaterial diffuseMaterial;
        private GeometryModel3D selectionModel;
        private Model3DGroup atomModel;
        private ColorScheme colorScheme;
        private bool isSelected;
        private bool showAsSelected;

        /// <summary>
        /// Label used for atom tooltips.
        /// </summary>
        internal override string DisplayName
        {
            get { return this.residue.DisplayName + " : " + this.atomName; }
        }

        /// <summary>
        /// Abreviated type name for the atom.
        /// </summary>
        internal string AtomName { get { return this.atomName; } }

        /// <summary>
        /// Abbreviated residue name for the amino acid atom belongs to.
        /// </summary>
        internal string ResidueName { get { return this.residueName; } }

        /// <summary>
        /// Alphanumeric chain identifier for the chain atom belongs to.
        /// </summary>
        internal string ChainIdentifier { get { return this.chainIdentifier; } }
        
        /// <summary>
        /// Index number for the amino acid the atom belongs to.
        /// </summary>
        internal int ResidueSequenceNumber { get { return this.residueSequenceNumber; } }

        /// <summary>
        /// 3D coordinate of this atom in angstroms.
        /// </summary>
        internal Point3D Position { get { return this.position; } }

        /// <summary>
        /// Covalently bonded atoms along with distance in angstroms.
        /// </summary>
        internal Dictionary<Atom, double> Bonds { get { return this.bonds; } }

        /// <summary>
        /// The 3D model for this atom.
        /// </summary>
        internal Model3DGroup Model { get { return this.model; } }

        /// <summary>
        /// Reference to the amino acid this atom belongs to.
        /// </summary>
        internal Residue Residue
        {
            get { return this.residue; }
            set { this.residue = value; }
        }

        /// <summary>
        /// Color to use for the structure coloring method.
        /// </summary>
        internal Color StructureColor
        {
            get { return this.structureColor; }
            set { this.structureColor = value; }
        }

        /// <summary>
        /// Currently used coloring method.
        /// </summary>
        internal ColorScheme ColorScheme
        {
            get
            {
                return this.colorScheme;
            }
            set
            {
                if (this.colorScheme != value)
                {
                    this.colorScheme = value;
                    this.UpdateColorView();
                }
            }
        }

        /// <summary>
        /// Gets and sets a boolean value indicating the current selection state.
        /// </summary>
        internal bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.showAsSelected = this.isSelected;
                    this.UpdateSelectionView();

                    this.residue.UpdateForAtomSelectionChange();
                }
            }
        }

        /// <summary>
        /// Gets and sets a boolean value indicating if the atom is rendered as selected. For
        /// certain operations such as rubber-banding an atom might be rendered as though it were
        /// selected even though it's not.
        /// </summary>
        internal bool ShowAsSelected
        {
            get
            {
                return this.showAsSelected;
            }
            set
            {
                if (this.showAsSelected != value)
                {
                    this.showAsSelected = value;
                    this.UpdateSelectionView();

                    this.residue.UpdateForAtomSelectionChange();
                }
            }
        }

        /// <summary>
        /// Static method for parsing atom entries in a pdb file and instantiating the correct
        /// <see cref="Atom" /> subclass.
        /// </summary>
        /// <param name="molecule">The molecule this atom belongs to.</param>
        /// <param name="pdbLine">An atom entry from a pdb file.</param>
        /// <returns>An instance of an <see cref="Atom" /> subclass.</returns>
        internal static Atom CreateAtom(Molecule molecule, string pdbLine)
        {
            Atom atom;

            string atomName = pdbLine.Substring(12, 4).Trim();
            string residueName = pdbLine.Substring(17, 3).Trim();

            if (Residue.IsAminoName(residueName))
            {
                if (atomName == "CA") atom = new CAlpha();
                else atom = new ChainAtom();
            }
            else
            {
                if (residueName == "HOH") atom = new Water();
                else atom = new HetAtom();
            }

            atom.molecule = molecule;

            atom.bonds = new Dictionary<Atom, double>();

            atom.atomName = pdbLine.Substring(12, 4).Trim();
            atom.residueName = pdbLine.Substring(17, 3).Trim();

            atom.residueSequenceNumber = Convert.ToInt32(pdbLine.Substring(22, 4));

            atom.chainIdentifier = pdbLine.Substring(21, 1);
            if (atom.residueName == "HOH") atom.chainIdentifier = "";
            else if (atom.chainIdentifier == " ") atom.chainIdentifier = "1";

            double x = Double.Parse(pdbLine.Substring(30, 8));
            double y = Double.Parse(pdbLine.Substring(38, 8));
            double z = Double.Parse(pdbLine.Substring(46, 8));

            atom.position = new Point3D(x, y, z);

            atom.temperatureFactor = Double.Parse(pdbLine.Substring(60, 6));

            if (atom.atomName.StartsWith("C")) atom.atomColor = Colors.LightGray;
            else if (atom.atomName.StartsWith("N")) atom.atomColor = Colors.Blue;
            else if (atom.atomName.StartsWith("O")) atom.atomColor = Colors.Red;
            else if (atom.atomName.StartsWith("H")) atom.atomColor = Colors.Purple;
            else if (atom.atomName.StartsWith("S")) atom.atomColor = Colors.Yellow;
            else atom.atomColor = Colors.Green;

            atom.structureColor = atom.atomColor;

            atom.colorScheme = ColorScheme.Structure;

            atom.diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(atom.atomColor));

            atom.model = new Model3DGroup();

            atom.translationTransform = new TranslateTransform3D(
                atom.position.X, atom.position.Y, atom.position.Z);

            atom.CreateSelectionSphere();

            return atom;
        }

        /// <summary>
        /// Static method that sets colors for the temperature coloring method for a list of atoms
        /// by normalizing the temperature values across the list.
        /// </summary>
        /// <param name="atoms">The list of atoms.</param>
        internal static void SetBFactorColors(List<Atom> atoms)
        {
            if (atoms.Count == 0) return;

            double minTemperature = atoms[0].temperatureFactor;
            double maxTemperature = atoms[0].temperatureFactor;

            foreach (Atom atom in atoms)
            {
                minTemperature = Math.Min(minTemperature, atom.temperatureFactor);
                maxTemperature = Math.Max(maxTemperature, atom.temperatureFactor);
            }

            double temperatureRange = maxTemperature - minTemperature;

            foreach (Atom atom in atoms)
            {
                double relativeTemperature = temperatureRange == 0 ? 0 :
                    (atom.temperatureFactor - minTemperature) / temperatureRange;

                if (relativeTemperature < 0.25)
                    atom.temperatureColor = Color.FromRgb(
                        0, (byte)(255 * (4 * relativeTemperature)), 255);
                else if (relativeTemperature < 0.5)
                    atom.temperatureColor = Color.FromRgb(
                        0, 255, (byte)(255 * (1 - 4 * (relativeTemperature - 0.25))));
                else if (relativeTemperature < 0.75)
                    atom.temperatureColor = Color.FromRgb(
                        (byte)(255 * (4 * (relativeTemperature - 0.5))), 255, 0);
                else
                    atom.temperatureColor = Color.FromRgb(
                        255, (byte)(255 * (1 - 4 * (relativeTemperature - 0.75))), 0);
            }
        }

        /// <summary>
        /// Used by a <see cref="Residue" /> to calculate it's temperature color.
        /// </summary>
        /// <param name="atoms">A list of atoms.</param>
        /// <returns>The average color.</returns>
        internal static Color GetAverageTemperateColor(List<Atom> atoms)
        {
            int r = 0, g = 0, b = 0;

            foreach (Atom atom in atoms)
            {
                r += atom.temperatureColor.R;
                g += atom.temperatureColor.G;
                b += atom.temperatureColor.B;
            }

            return Color.FromRgb((byte)(r / atoms.Count), (byte)(g / atoms.Count),
                (byte)(b / atoms.Count));
        }

        /// <summary>
        /// Static method to calculate the 3D bounding box for a list of atoms.
        /// </summary>
        /// <param name="atoms">A list of atoms.</param>
        /// <returns>The 3D bounding box.</returns>
        internal static Rect3D GetBounds(List<Atom> atoms)
        {
            if (atoms.Count == 0) return Rect3D.Empty;

            double x1 = atoms[0].position.X;
            double x2 = atoms[0].position.X;
            double y1 = atoms[0].position.Y;
            double y2 = atoms[0].position.Y;
            double z1 = atoms[0].position.Z;
            double z2 = atoms[0].position.Z;

            foreach (Atom atom in atoms)
            {
                x1 = Math.Min(x1, atom.position.X);
                x2 = Math.Max(x2, atom.position.X);
                y1 = Math.Min(y1, atom.position.Y);
                y2 = Math.Max(y2, atom.position.Y);
                z1 = Math.Min(z1, atom.position.Z);
                z2 = Math.Max(z2, atom.position.Z);
            }

            return new Rect3D(x1, y1, z1, x2 - x1, y2 - y1, z2 - z1);
        }

        /// <summary>
        /// Extra initialization for this class and subclasses that can't be done in the
        /// constructor since certain properties are expected to be set.
        /// </summary>
        internal virtual void Initialize()
        {
            this.UpdateColorView();
        }

        /// <summary>
        /// Performs hit testing for this atom.
        /// </summary>
        /// <param name="rayHitTestResult">A 3D mesh hit test result from the WPF visual tree hit
        /// testing framework</param>
        /// <returns>True if the mesh hit belongs to this atom, otherwise false.</returns>
        internal virtual bool HoverHitTest(RayMeshGeometry3DHitTestResult rayHitTestResult)
        {
            return (this.selectionModel == rayHitTestResult.ModelHit || (this.atomModel != null &&
                this.atomModel.Children.Contains(rayHitTestResult.ModelHit)));
        }

        /// <summary>
        /// The molecule this atom belongs to.
        /// </summary>
        protected Molecule Molecule { get { return this.molecule; } }

        /// <summary>
        /// Updates the 3D model to depict the correct selection state.
        /// </summary>
        protected virtual void UpdateSelectionView()
        {
            if (this.showAsSelected &&
                !this.model.Children.Contains(this.selectionModel))
            {
                this.model.Children.Add(this.selectionModel);
            }
            else if (!this.showAsSelected
                && this.model.Children.Contains(this.selectionModel))
            {
                this.model.Children.Remove(this.selectionModel);
            }
        }

        /// <summary>
        /// Updates the 3D model to depict the correct hovered state.
        /// </summary>
        protected override void OnIsHoveredChanged()
        {
            this.UpdateColorView();
        }

        /// <summary>
        /// Toggles visiblity of the atom. Also used to delay creation of the model until it's
        /// actually needed.
        /// </summary>
        /// <param name="show">True to show the atom, false to hide it.</param>
        protected void RenderAtomModel(bool show)
        {
            if (show && this.atomModel == null)
            {
                this.atomModel = new Model3DGroup();
                this.model.Children.Add(this.atomModel);

                if (this.bonds.Count == 0)
                    this.CreateUnbondedSphere();
                else
                    foreach (Atom atom in this.bonds.Keys)
                        this.CreateBondStick(this.atomModel, atom, this.bonds[atom]);
            }
            else if (show && !this.model.Children.Contains(this.atomModel))
            {
                this.model.Children.Add(this.atomModel);
            }
            else if (!show && this.atomModel != null &&
                this.model.Children.Contains(this.atomModel))
            {
                this.model.Children.Remove(this.atomModel);
            }
        }

        /// <summary>
        /// Creates the 3D model for stick that represents a bond.
        /// </summary>
        /// <param name="modelGroup">The Model3DGroup that will contain the stick.</param>
        /// <param name="atom">The other atom this atom is bonded with.</param>
        /// <param name="distance">The distance to the other bonded atom.</param>
        protected void CreateBondStick(Model3DGroup modelGroup, Atom atom, double distance)
        {
            GeometryModel3D stickModel = new GeometryModel3D();
            stickModel.Geometry = distance > 2 ? Stick.LongMesh : Stick.ShortMesh;
            stickModel.Material = this.diffuseMaterial;
            modelGroup.Children.Add(stickModel);

            Transform3DGroup transformGroup = new Transform3DGroup();
            stickModel.Transform = transformGroup;

            ScaleTransform3D scaleTransform = new ScaleTransform3D();
            scaleTransform.ScaleX = distance / 2;
            transformGroup.Children.Add(scaleTransform);

            RotateTransform3D rotationTransfrom = new RotateTransform3D();
            transformGroup.Children.Add(rotationTransfrom);

            Vector3D orientationVector = new Vector3D(1, 0, 0);
            Vector3D differenceVector = new Vector3D(atom.position.X - this.position.X,
                atom.position.Y - this.position.Y, atom.position.Z - this.position.Z);

            AxisAngleRotation3D rotation = new AxisAngleRotation3D();
            rotation.Angle = Vector3D.AngleBetween(orientationVector, differenceVector);
            rotation.Axis = Vector3D.CrossProduct(orientationVector, differenceVector);

            if (rotation.Axis.LengthSquared > 0)
                rotationTransfrom.Rotation = rotation;

            transformGroup.Children.Add(this.translationTransform);
        }

        /// <summary>
        /// Creates the 3D model for a ball that is used to visually identify a selected atom.
        /// </summary>
        private void CreateSelectionSphere()
        {
            this.selectionModel = new GeometryModel3D();
            this.selectionModel.Geometry = Sphere.Mesh;
            this.selectionModel.Material = this.diffuseMaterial;

            Transform3DGroup sphereTransformGroup = new Transform3DGroup();
            this.selectionModel.Transform = sphereTransformGroup;

            ScaleTransform3D scaleTransform = new ScaleTransform3D(0.4, 0.4, 0.4);
            sphereTransformGroup.Children.Add(scaleTransform);

            sphereTransformGroup.Children.Add(this.translationTransform);
        }

        /// <summary>
        /// Creates the 3D model for a small ball that is used to represent an unbonded atom since
        /// no sticks will be connected.
        /// </summary>
        private void CreateUnbondedSphere()
        {
            GeometryModel3D sphereModel = new GeometryModel3D();
            sphereModel.Geometry = Sphere.Mesh;
            sphereModel.Material = this.diffuseMaterial;
            this.atomModel.Children.Add(sphereModel);

            Transform3DGroup sphereTransformGroup = new Transform3DGroup();
            sphereModel.Transform = sphereTransformGroup;

            ScaleTransform3D scaleTransform = new ScaleTransform3D(0.2, 0.2, 0.2);
            sphereTransformGroup.Children.Add(scaleTransform);

            sphereTransformGroup.Children.Add(this.translationTransform);
        }

        /// <summary>
        /// Updates the material color for this atom based on the coloring method and the current
        /// hover state.
        /// </summary>
        private void UpdateColorView()
        {
            Color color = this.atomColor;

            if (this.colorScheme == ColorScheme.Structure)
                color = this.structureColor;
            else if (this.colorScheme == ColorScheme.Residue && this.residue != null)
                color = this.residue.ResidueColor;
            else if (this.colorScheme == ColorScheme.Chain && this.residue != null &&
                this.residue.Chain != null)
                color = this.residue.Chain.ChainColor;
            else if (this.colorScheme == ColorScheme.Temperature)
                color = this.temperatureColor;

            if (this.IsHovered)
            {
                byte r = (byte)(color.R + (255 - color.R) / 2);
                byte g = (byte)(color.G + (255 - color.G) / 2);
                byte b = (byte)(color.B + (255 - color.B) / 2);

                if (r == g && g == b) r = g = b = 255;

                color = Color.FromRgb(r, g, b);
            }

            ((SolidColorBrush)this.diffuseMaterial.Brush).Color = color;
        }
    }
}
