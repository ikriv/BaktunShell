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
using System.Windows.Media.Media3D;

namespace MoleculeViewer
{
    /// <summary>
    /// Calculates spline paths for all residues in a particular structure.
    /// </summary>
    internal class Ribbon
    {
        private const int linearSegmentCount = 10;

        private List<Residue> residues = new List<Residue>();
        private List<bool> isHelixList = new List<bool>();
        private List<Point3D> caList = new List<Point3D>();
        private List<Point3D> oList = new List<Point3D>();
        private List<Point3D> pList = new List<Point3D>();
        private List<Point3D> cList = new List<Point3D>();
        private List<Point3D> dList = new List<Point3D>();
        private List<Point3D> ribbonPoints = new List<Point3D>();
        private List<Vector3D> torsionVectors = new List<Vector3D>();
        private List<Vector3D> normalVectors = new List<Vector3D>();

        /// <summary>
        /// All of the residues in the secondary strucuture associated with this
        /// <see cref="Ribbon" />.
        /// </summary>
        internal List<Residue> Residues { get { return this.residues; } }

        /// <summary>
        /// Initiates the spine calculation logic for all constituent residues.
        /// </summary>
        internal void CreateControlPoints()
        {
            if (this.residues.Count < 4)
            {
                foreach (Residue residue in this.residues)
                    residue.Ribbon = null;

                return;
            }

            this.PopulateAtomLists();
            this.PopulateControlLists();
            this.PopulateSplineLists();
        }

        /// <summary>
        /// Gets all of the values that represent the spline for a particular residue.
        /// </summary>
        /// <param name="residue">A residue in the corresponding secondary structure.</param>
        /// <param name="residueRibbonPoints">A list control points for the spline.</param>
        /// <param name="residueTorsionVectors">A list of the torsion vectors for the
        /// spline.</param>
        /// <param name="residueNormalVectors">A list of the normal vectors for the spline.</param>
        internal void GetResidueSpline(Residue residue, out List<Point3D> residueRibbonPoints,
            out List<Vector3D> residueTorsionVectors, out List<Vector3D> residueNormalVectors)
        {
            residueRibbonPoints = new List<Point3D>();
            residueTorsionVectors = new List<Vector3D>();
            residueNormalVectors = new List<Vector3D>();

            int startIndex = this.residues.IndexOf(residue) * Ribbon.linearSegmentCount;

            for (int i = startIndex; i <= startIndex + Ribbon.linearSegmentCount; i++)
            {
                residueRibbonPoints.Add(this.ribbonPoints[i]);
                residueTorsionVectors.Add(this.torsionVectors[i]);
                residueNormalVectors.Add(this.normalVectors[i]);
            }
        }

        /// <summary>
        /// Helper function used by <see cref="CreateControlPoints" /> to populate the data
        /// stuctures which refence certain atom types.
        /// </summary>
        private void PopulateAtomLists()
        {
            foreach (Residue residue in this.residues)
            {
                this.isHelixList.Add(residue.IsHelix);
                this.caList.Add(residue.CAlphaPosition.Value);
                this.oList.Add(residue.CarbonylOxygenPosition.Value);
            }

            this.isHelixList.Insert(0, this.isHelixList[0]);
            this.isHelixList.Insert(0, this.isHelixList[1]);

            this.isHelixList.Add(this.isHelixList[this.isHelixList.Count - 1]);
            this.isHelixList.Add(this.isHelixList[this.isHelixList.Count - 2]);

            this.caList.Insert(0, this.Reflect(this.caList[0], this.caList[1], 0.4));
            this.caList.Insert(0, this.Reflect(this.caList[1], this.caList[2], 0.6));

            this.caList.Add(this.Reflect(
                this.caList[this.caList.Count - 1], this.caList[this.caList.Count - 2], 0.4));
            this.caList.Add(this.Reflect(
                this.caList[this.caList.Count - 2], this.caList[this.caList.Count - 3], 0.6));

            this.oList.Insert(0, this.Reflect(this.oList[0], this.oList[1], 0.4));
            this.oList.Insert(0, this.Reflect(this.oList[1], this.oList[2], 0.6));

            this.oList.Add(this.Reflect(
                this.oList[this.oList.Count - 1], this.oList[this.oList.Count - 2], 0.4));
            this.oList.Add(this.Reflect(
                this.oList[this.oList.Count - 2], this.oList[this.oList.Count - 3], 0.6));
        }

        /// <summary>
        /// Helper function used by <see cref="CreateControlPoints" /> to populate the data
        /// stuctures which hold control point data.
        /// </summary>
        private void PopulateControlLists()
        {
            Vector3D previousD = new Vector3D();

            for (int i = 0; i < this.caList.Count - 1; i++)
            {
                Point3D ca1 = this.caList[i];
                Point3D o1 = this.oList[i];
                Point3D ca2 = this.caList[i + 1];

                Point3D p = new Point3D((ca1.X + ca2.X) / 2, (ca1.Y + ca2.Y) / 2,
                    (ca1.Z + ca2.Z) / 2);

                Vector3D a = ca2 - ca1;
                Vector3D b = o1 - ca1;

                Vector3D c = Vector3D.CrossProduct(a, b);
                Vector3D d = Vector3D.CrossProduct(c, a);

                c.Normalize();
                d.Normalize();

                if (this.isHelixList[i] && this.isHelixList[i + 1])
                    p.Offset(1.5 * c.X, 1.5 * c.Y, 1.5 * c.Z);

                if (i > 0 && Vector3D.AngleBetween(d, previousD) > 90) d.Negate();
                previousD = d;

                this.pList.Add(p);
                this.dList.Add(p + d);
            }
        }

        /// <summary>
        /// Helper function used by <see cref="CreateControlPoints" /> to populate the data
        /// stuctures which hold the spline data.
        /// </summary>
        private void PopulateSplineLists()
        {
            Point3D previousRibbonPoint = new Point3D();
            Point3D ribbonPoint;
            Point3D torsionPoint;

            for (int i = 0; i < this.residues.Count; i++)
            {
                Point3D p1 = pList[i];
                Point3D p2 = pList[i + 1];
                Point3D p3 = pList[i + 2];
                Point3D p4 = pList[i + 3];

                Point3D d1 = dList[i];
                Point3D d2 = dList[i + 1];
                Point3D d3 = dList[i + 2];
                Point3D d4 = dList[i + 3];

                for (int j = 1; j <= Ribbon.linearSegmentCount; j++)
                {
                    double t = (double)j / Ribbon.linearSegmentCount;

                    if (t < 0.5)
                    {
                        ribbonPoint = this.Spline(p1, p2, p3, t + 0.5);
                        torsionPoint = this.Spline(d1, d2, d3, t + 0.5);
                    }
                    else
                    {
                        ribbonPoint = this.Spline(p2, p3, p4, t - 0.5);
                        torsionPoint = this.Spline(d2, d3, d4, t - 0.5);
                    }

                    if (i == 0 && j == 1)
                    {
                        previousRibbonPoint = this.Spline(p1, p2, p3, 0.5);

                        Point3D previousTorsionPoint = this.Spline(d1, d2, d3, 0.5);

                        Point3D extrapolatedRibbonPoint =
                            this.Reflect(previousRibbonPoint, ribbonPoint, 1);

                        this.AddSplineNode(extrapolatedRibbonPoint, previousRibbonPoint,
                            previousTorsionPoint);
                    }

                    this.AddSplineNode(previousRibbonPoint, ribbonPoint, torsionPoint);

                    previousRibbonPoint = ribbonPoint;
                }
            }
        }

        /// <summary>
        /// Helper function used by <see cref="PopulateSplineLists" /> to populate the data
        /// stuctures for a particular point along the spline.
        /// </summary>
        private void AddSplineNode(
            Point3D previousRibbonPoint, Point3D ribbonPoint, Point3D torsionPoint)
        {
            this.ribbonPoints.Add(ribbonPoint);

            Vector3D torsionVector = torsionPoint - ribbonPoint;
            torsionVector.Normalize();
            this.torsionVectors.Add(torsionVector);

            Vector3D ribbonVector = ribbonPoint - previousRibbonPoint;
            Vector3D normalVector = Vector3D.CrossProduct(torsionVector, ribbonVector);
            normalVector.Normalize();
            this.normalVectors.Add(normalVector);
        }

        /// <summary>
        /// Reflects one point across another by a specified amount.
        /// </summary>
        /// <param name="p1">Point 1.</param>
        /// <param name="p2">Point 2.</param>
        /// <param name="amount">The reflection scaling factor.</param>
        /// <returns></returns>
        private Point3D Reflect(Point3D p1, Point3D p2, double amount)
        {
            double x = p1.X - amount * (p2.X - p1.X);
            double y = p1.Y - amount * (p2.Y - p1.Y);
            double z = p1.Z - amount * (p2.Z - p1.Z);

            return new Point3D(x, y, z);
        }

        /// <summary>
        /// Calculates the actual spline position.
        /// </summary>
        /// <param name="p1">Point 1.</param>
        /// <param name="p2">Point 2.</param>
        /// <param name="p3">Point 3.</param>
        /// <param name="t">The parametric value along the spline section.</param>
        /// <returns></returns>
        private Point3D Spline(Point3D p1, Point3D p2, Point3D p3, double t)
        {
            double a = Math.Pow(1 - t, 2) / 2;
            double c = Math.Pow(t, 2) / 2;
            double b = 1 - a - c;

            double x = a * p1.X + b * p2.X + c * p3.X;
            double y = a * p1.Y + b * p2.Y + c * p3.Y;
            double z = a * p1.Z + b * p2.Z + c * p3.Z;

            return new Point3D(x, y, z);
        }
    }
}
