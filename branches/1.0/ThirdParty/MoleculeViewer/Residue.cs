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
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MoleculeViewer
{
    /// <summary>
    /// Represents a residue in a molecule. 
    /// </summary>
    /// <remarks>
    /// Sometimes referred to as an amino acid. Generates WPF content to display the 
    /// residue in cartoon mode as well as in the identifier strip at the
    /// top of the screen. Aggregates all constituent atoms.
    /// </remarks>
    internal class Residue : HoverObject
    {
        /// <summary>
        /// 
        /// </summary>
        internal enum SelectionType { None, Partial, Full };

        private Molecule molecule;
        private string residueName;
        private string chainIdentifier;
        private int residueSequenceNumber;
        private string residueIdentifier;
        private Color residueColor;
        private Color structureColor;
        private Color color;
        private List<Atom> atoms;
        private bool isSheet;
        private bool isHelix;
        private bool isStructureStart;
        private bool isStructureEnd;
        private Residue previousResidue;
        private Residue nextResidue;
        private Point3D? cAlphaPosition;
        private Point3D? carbonylOxygenPosition;
        private Chain chain;
        private Ribbon ribbon;
        private List<ResidueStripItem> residueStripItems;
        private Model3DGroup model;
        private ColorScheme colorScheme;
        private SelectionType selection;
        private SelectionType showAsSelection;
        private bool updatingSelectionProperty;
        private Cartoon cartoon;

        /// <summary>
        /// Creates a new <see cref="Residue" /> object.
        /// </summary>
        /// <param name="molecule">The molecule this residue belongs to.</param>
        /// <param name="atom">An atom in the residue. This is needed to obtain residue properties
        /// since there is no corresponding PDB file record.</param>
        internal Residue(Molecule molecule, Atom atom)
        {
            this.molecule = molecule;
            this.molecule.ShowCartoonChanged += this.MoleculeShowCartoonChanged;

            this.residueName = atom.ResidueName;
            this.chainIdentifier = atom.ChainIdentifier;
            this.residueSequenceNumber = atom.ResidueSequenceNumber;

            this.atoms = new List<Atom>();
            this.atoms.Add(atom);

            this.residueIdentifier = Residue.GetResidueIdentifier(this.residueName);
            this.residueColor = Residue.GetResidueColor(this.residueName);

            this.structureColor = this.residueIdentifier != "O" ? Colors.LightGray : Colors.Red;

            this.colorScheme = ColorScheme.Structure;

            this.residueStripItems = new List<ResidueStripItem>();
            foreach (char character in this.residueIdentifier)
            {
                ResidueStripItem residueStripItem = new ResidueStripItem(character.ToString());
                residueStripItem.Residue = this;
                this.residueStripItems.Add(residueStripItem);
            }

            this.model = new Model3DGroup();

            this.UpdateColorView();
        }

        /// <summary>
        /// Label used for atom tooltips.
        /// </summary>
        internal override string DisplayName
        {
            get
            {
                return "[" + this.residueSequenceNumber + "] " + this.residueName;
            }
        }

        /// <summary>
        /// The multi-character abbreviation for the residue. For chain-based amino acids, this is
        /// a three letter code.
        /// </summary>
        internal string ResidueName { get { return this.residueName; } }

        /// <summary>
        /// Alphanumeric chain identifier for the chain residue belongs to.
        /// </summary>
        internal string ChainIdentifier { get { return this.chainIdentifier; } }

        /// <summary>
        /// Index number for this amino acid.
        /// </summary>
        internal int ResidueSequenceNumber { get { return this.residueSequenceNumber; } }

        /// <summary>
        /// Shortened abbreviation for the residue. For chain-based amino acids, this is a single
        /// letter.
        /// </summary>
        internal string ResidueIdentifier { get { return this.residueIdentifier; } }

        /// <summary>
        /// The color used for this residue when using the residue-based coloring method.
        /// </summary>
        internal Color ResidueColor { get { return this.residueColor; } }

        /// <summary>
        /// The constituent atoms.
        /// </summary>
        internal List<Atom> Atoms { get { return this.atoms; } }

        /// <summary>
        /// The 3D model for this residue.
        /// </summary>
        internal Model3DGroup Model { get { return this.model; } }

        /// <summary>
        /// Gets and sets a boolean value indicating if this residue is part of a sheet.
        /// </summary>
        internal bool IsSheet
        {
            get { return this.isSheet; }
            set { this.isSheet = value; }
        }

        /// <summary>
        /// Gets and sets a boolean value indicating if this residue is part of a helix.
        /// </summary>
        internal bool IsHelix
        {
            get { return this.isHelix; }
            set { this.isHelix = value; }
        }

        /// <summary>
        /// Gets and sets a boolean value indicating if this residue is the first residue in a
        /// secondary structure.
        /// </summary>
        internal bool IsStructureStart
        {
            get { return this.isStructureStart; }
            set { this.isStructureStart = value; }
        }

        /// <summary>
        /// Gets and sets a boolean value indicating if this residue is the last residue in a
        /// secondary structure.
        /// </summary>
        internal bool IsStructureEnd
        {
            get { return this.isStructureEnd; }
            set { this.isStructureEnd = value; }
        }

        /// <summary>
        /// Reference to the previous residue in the current chain.
        /// </summary>
        internal Residue PreviousResidue
        {
            get { return this.previousResidue; }
            set { this.previousResidue = value; }
        }

        /// <summary>
        /// Reference to the next residue in the current chain.
        /// </summary>
        internal Residue NextResidue
        {
            get { return this.nextResidue; }
            set { this.nextResidue = value; }
        }

        /// <summary>
        /// If residue belongs to a standard protein amino acid this will contain the 3D location
        /// of the alpha carbon atom.
        /// </summary>
        internal Point3D? CAlphaPosition
        {
            get { return this.cAlphaPosition; }
            set { this.cAlphaPosition = value; }
        }

        /// <summary>
        /// If residue belongs to a standard protein amino acid this will contain the 3D location
        /// of the carbonyl oxygen atom.
        /// </summary>
        internal Point3D? CarbonylOxygenPosition
        {
            get { return this.carbonylOxygenPosition; }
            set { this.carbonylOxygenPosition = value; }
        }

        /// <summary>
        /// The chain this residue belongs to.
        /// </summary>
        internal Chain Chain
        {
            get { return this.chain; }
            set { this.chain = value; }
        }

        /// <summary>
        /// Reference to the <see cref="Ribbon" /> object that calculates spline paths for this
        /// residue.
        /// </summary>
        internal Ribbon Ribbon
        {
            get { return this.ribbon; }
            set { this.ribbon = value; }
        }

        /// <summary>
        /// The color to use for this residue when using the structure-based coloring method.
        /// </summary>
        internal Color StructureColor
        {
            get { return this.structureColor; }
            set
            {
                this.structureColor = value;
                this.UpdateColorView();
            }
        }

        /// <summary>
        /// All of the <see cref="ResidueStripItem" /> controls for this residue. For protein
        /// amino acids, there is only one item in this list.
        /// </summary>
        internal List<ResidueStripItem> ResidueStripItems
        {
            get { return this.residueStripItems; }
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
        /// Gets and sets a <see cref="SelectionType" /> enumeration value indicating the current
        /// selection state.
        /// </summary>
        internal SelectionType Selection
        {
            get
            {
                return this.selection;
            }
            set
            {
                if (this.selection != value)
                {
                    this.selection = value;
                    this.showAsSelection = this.selection;
                    this.UpdateView();

                    this.updatingSelectionProperty = true;

                    foreach (Atom atom in this.atoms)
                    {
                        if (this.selection == SelectionType.None)
                            atom.IsSelected = false;
                        else if (this.selection == SelectionType.Full)
                            atom.IsSelected = true;
                    }

                    this.updatingSelectionProperty = false;
                }
            }
        }

        /// <summary>
        /// Gets and sets a <see cref="SelectionType" /> enumeration value indicating if the
        /// residue is rendered as selected. For certain operations such as rubber-banding a
        /// residue might be rendered as though it were selected even though it's not.
        /// </summary>
        internal SelectionType ShowAsSelection
        {
            get
            {
                return this.showAsSelection;
            }
            set
            {
                if (this.showAsSelection != value)
                {
                    this.showAsSelection = value;
                    this.UpdateView();

                    this.updatingSelectionProperty = true;

                    foreach (Atom atom in this.atoms)
                    {
                        if (this.showAsSelection == SelectionType.None)
                            atom.ShowAsSelected = false;
                        else if (this.showAsSelection == SelectionType.Partial)
                            atom.ShowAsSelected = atom.IsSelected;
                        else if (this.showAsSelection == SelectionType.Full)
                            atom.ShowAsSelected = true;
                    }

                    this.updatingSelectionProperty = false;
                }
            }
        }

        /// <summary>
        /// Updates the selection state based on the selection states of the constituent atoms.
        /// </summary>
        internal void UpdateForAtomSelectionChange()
        {
            if (this.updatingSelectionProperty) return;

            bool dirty = false;

            bool fullSelected = true;
            bool partialSelected = false;

            foreach (Atom atom in this.atoms)
            {
                if (atom.IsSelected) partialSelected = true;
                else fullSelected = false;
            }

            if (fullSelected && this.selection != SelectionType.Full)
            {
                this.selection = SelectionType.Full;
                dirty = true;
            }
            else if (!fullSelected && partialSelected && this.selection != SelectionType.Partial)
            {
                this.selection = SelectionType.Partial;
                dirty = true;
            }
            else if (!partialSelected && this.selection != SelectionType.None)
            {
                this.selection = SelectionType.None;
                dirty = true;
            }

            bool fullShowAsSelected = true;
            bool partialShowAsSelected = false;

            foreach (Atom atom in this.atoms)
            {
                if (atom.ShowAsSelected) partialShowAsSelected = true;
                else fullShowAsSelected = false;
            }

            if (fullShowAsSelected && this.showAsSelection != SelectionType.Full)
            {
                this.showAsSelection = SelectionType.Full;
                dirty = true;
            }
            else if (!fullShowAsSelected && partialShowAsSelected &&
                this.showAsSelection != SelectionType.Partial)
            {
                this.showAsSelection = SelectionType.Partial;
                dirty = true;
            }
            else if (!partialShowAsSelected && this.showAsSelection != SelectionType.None)
            {
                this.showAsSelection = SelectionType.None;
                dirty = true;
            }

            if (dirty) this.UpdateView();
        }

        /// <summary>
        /// Performs hit testing for this residue.
        /// </summary>
        /// <param name="rayHitTestResult">A 3D mesh hit test result from the WPF visual tree hit
        /// testing framework</param>
        /// <returns>True if the mesh hit belongs to this residue, otherwise false.</returns>
        internal virtual bool HoverHitTest(RayMeshGeometry3DHitTestResult rayHitTestResult)
        {
            if (this.cartoon != null)
                return this.cartoon.HoverHitTest(rayHitTestResult);
            else
                return false;
        }

        /// <summary>
        /// Determines if a particular residue name refers to an amino acid.
        /// </summary>
        /// <param name="residueName">A multi-character residue abbreviation.</param>
        /// <returns>True if and only if the residue name refers to an amino acid.</returns>
        internal static bool IsAminoName(string residueName)
        {
            if (residueName == "ALA") return true;
            else if (residueName == "ARG") return true;
            else if (residueName == "ASP") return true;
            else if (residueName == "CYS") return true;
            else if (residueName == "GLN") return true;
            else if (residueName == "GLU") return true;
            else if (residueName == "GLY") return true;
            else if (residueName == "HIS") return true;
            else if (residueName == "ILE") return true;
            else if (residueName == "LEU") return true;
            else if (residueName == "LYS") return true;
            else if (residueName == "MET") return true;
            else if (residueName == "PHE") return true;
            else if (residueName == "PRO") return true;
            else if (residueName == "SER") return true;
            else if (residueName == "THR") return true;
            else if (residueName == "TRP") return true;
            else if (residueName == "TYR") return true;
            else if (residueName == "VAL") return true;
            else if (residueName == "ASN") return true;
            else return false;
        }

        /// <summary>
        /// Updates the 3D model to depict the correct hovered state.
        /// </summary>
        protected override void OnIsHoveredChanged()
        {
            this.UpdateView();
        }

        /// <summary>
        /// Static method to obtain the single character abbreviation of an amino acid.
        /// </summary>
        /// <param name="residueName">A multi-character residue abbreviation.</param>
        /// <returns>A single character abbreviation if one is available, othewise return the input
        /// abbreviation.</returns>
        private static string GetResidueIdentifier(string residueName)
        {
            if (residueName == "HOH") return "O";
            else if (residueName == "ALA") return "A";
            else if (residueName == "ARG") return "R";
            else if (residueName == "ASP") return "D";
            else if (residueName == "CYS") return "C";
            else if (residueName == "GLN") return "Q";
            else if (residueName == "GLU") return "E";
            else if (residueName == "GLY") return "G";
            else if (residueName == "HIS") return "H";
            else if (residueName == "ILE") return "I";
            else if (residueName == "LEU") return "L";
            else if (residueName == "LYS") return "K";
            else if (residueName == "MET") return "M";
            else if (residueName == "PHE") return "F";
            else if (residueName == "PRO") return "P";
            else if (residueName == "SER") return "S";
            else if (residueName == "THR") return "T";
            else if (residueName == "TRP") return "W";
            else if (residueName == "TYR") return "Y";
            else if (residueName == "VAL") return "V";
            else if (residueName == "ASN") return "N";
            else return residueName;
        }

        /// <summary>
        /// Selects a color based on the residue type.
        /// </summary>
        /// <param name="residueName">A multi-character residue abbreviation.</param>
        /// <returns>A color for the residue.</returns>
        private static Color GetResidueColor(string residueName)
        {
            if (residueName == "HOH") return Colors.Red;
            else if (residueName == "ALA") return Color.FromRgb(199, 199, 199);
            else if (residueName == "ARG") return Color.FromRgb(229, 10, 10);
            else if (residueName == "CYS") return Color.FromRgb(229, 229, 0);
            else if (residueName == "GLN") return Color.FromRgb(0, 229, 229);
            else if (residueName == "GLU") return Color.FromRgb(229, 10, 10);
            else if (residueName == "GLY") return Color.FromRgb(234, 234, 234);
            else if (residueName == "HIS") return Color.FromRgb(130, 130, 209);
            else if (residueName == "ILE") return Color.FromRgb(15, 130, 15);
            else if (residueName == "LEU") return Color.FromRgb(15, 130, 15);
            else if (residueName == "LYS") return Color.FromRgb(20, 90, 255);
            else if (residueName == "MET") return Color.FromRgb(229, 229, 0);
            else if (residueName == "PHE") return Color.FromRgb(50, 50, 169);
            else if (residueName == "PRO") return Color.FromRgb(219, 149, 130);
            else if (residueName == "SER") return Color.FromRgb(249, 149, 0);
            else if (residueName == "THR") return Color.FromRgb(249, 149, 0);
            else if (residueName == "TRP") return Color.FromRgb(179, 90, 179);
            else if (residueName == "TYR") return Color.FromRgb(50, 50, 169);
            else if (residueName == "VAL") return Color.FromRgb(15, 130, 15);
            else if (residueName == "ASN") return Color.FromRgb(0, 229, 229);
            else return Colors.Green;
        }

        /// <summary>
        /// Toggles visibility of 3D model components based on the
        /// <see cref="Molecule.ShowCartoon" /> property.
        /// </summary>
        /// <param name="sender">The molecule.</param>
        /// <param name="e">Empty event args.</param>
        private void MoleculeShowCartoonChanged(object sender, EventArgs e)
        {
            if (this.ribbon != null)
            {
                if (this.molecule.ShowCartoon && this.cartoon == null)
                {
                    this.cartoon = new Cartoon(this, this.color);
                }

                if (this.molecule.ShowCartoon && !this.Model.Children.Contains(this.cartoon.Model))
                {
                    this.model.Children.Add(this.cartoon.Model);
                }
                else if (!this.molecule.ShowCartoon &&
                    this.model.Children.Contains(this.cartoon.Model))
                {
                    this.model.Children.Remove(this.cartoon.Model);
                }
            }
        }

        /// <summary>
        /// Selects the material color for this residue based on the coloring method.
        /// </summary>
        private void UpdateColorView()
        {
            if (this.colorScheme == ColorScheme.Structure)
                this.color = this.structureColor;
            else if (this.colorScheme == ColorScheme.Atom && this.residueIdentifier == "O")
                this.color = Colors.Red;
            else if (this.colorScheme == ColorScheme.Atom)
                this.color = Colors.LightGray;
            else if (this.colorScheme == ColorScheme.Residue)
                this.color = this.residueColor;
            else if (this.colorScheme == ColorScheme.Chain && this.chain != null)
                this.color = this.chain.ChainColor;
            else if (this.colorScheme == ColorScheme.Temperature)
                this.color = Atom.GetAverageTemperateColor(this.atoms);
            else
                this.color = Colors.LightGray;
            
            this.UpdateView();
        }

        /// <summary>
        /// Updates the material color for this atom based on the coloring method and the current
        /// hover state.
        /// </summary>
        private void UpdateView()
        {
            Color actualColor = this.color;

            if (this.IsHovered)
            {
                byte r = (byte)(color.R + (255 - color.R) / 2);
                byte g = (byte)(color.G + (255 - color.G) / 2);
                byte b = (byte)(color.B + (255 - color.B) / 2);

                if (r == g && g == b) r = g = b = 255;

                actualColor = Color.FromRgb(r, g, b);
            }

            SolidColorBrush foreground = new SolidColorBrush(actualColor);
            SolidColorBrush background = Brushes.Transparent;

            if (this.showAsSelection == SelectionType.Partial)
            {
                foreground = new SolidColorBrush(actualColor);
                background = new SolidColorBrush(
                    Color.FromArgb(96, actualColor.R, actualColor.G, actualColor.B));
            }
            else if (this.showAsSelection == SelectionType.Full)
            {
                foreground = Brushes.Black;
                background = new SolidColorBrush(actualColor);
            }

            foreach (ResidueStripItem residueStripItem in this.residueStripItems)
            {
                residueStripItem.Label.Foreground = foreground;
                residueStripItem.Label.Background = background;
            }

            if (this.cartoon != null)
                this.cartoon.Color = actualColor;
        }
    }
}
