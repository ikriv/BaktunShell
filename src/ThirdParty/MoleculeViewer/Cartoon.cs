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
    /// Creates the 3D model for a particular residue when being displayed in cartoon mode.
    /// </summary>
    internal class Cartoon
    {
        private const int radialSegmentCount = 10;
        private const double turnWidth = 0.2;
        private const double helixWidth = 1.4;
        private const double helixHeight = 0.25;
        private const double sheetWidth = 1.2;
        private const double sheetHeight = 0.25;
        private const double arrowWidth = 1.6;

        private Residue residue;
        private Model3DGroup model;
        private MaterialGroup materialGroup;
        private DiffuseMaterial diffuseMaterial;
        private List<Point3D> ribbonPoints = new List<Point3D>();
        private List<Vector3D> torsionVectors = new List<Vector3D>();
        private List<Vector3D> normalVectors = new List<Vector3D>();

        /// <summary>
        /// Builds the 3D model for the cartoon view a the given residue.
        /// </summary>
        /// <param name="residue">A residue.</param>
        /// <param name="initialColor">The residue's current color.</param>
        internal Cartoon(Residue residue, Color initialColor)
        {
            this.residue = residue;

            this.materialGroup = new MaterialGroup();

            this.diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(initialColor));
            this.materialGroup.Children.Add(diffuseMaterial);

            SpecularMaterial specularMaterial = new SpecularMaterial();
            specularMaterial.Brush = new SolidColorBrush(Color.FromArgb(192, 255, 255, 255));
            specularMaterial.SpecularPower = 50;
            this.materialGroup.Children.Add(specularMaterial);

            this.model = new Model3DGroup();

            this.residue.Ribbon.GetResidueSpline(this.residue, out this.ribbonPoints,
                out this.torsionVectors, out this.normalVectors);

            if (this.residue.IsHelix)
            {
                this.AddTube(Cartoon.helixWidth, Cartoon.helixHeight);

                if (this.residue.IsStructureStart)
                    this.AddTubeCap(Cartoon.helixWidth, Cartoon.helixHeight);

                if (this.residue.IsStructureEnd)
                    this.AddTubeCap(Cartoon.helixWidth, Cartoon.helixHeight);
            }
            else if (this.residue.IsSheet)
            {
                this.AddSheet();

                if (this.residue.IsStructureStart || this.residue.IsStructureEnd)
                    this.AddSheetCap();
            }
            else
            {
                this.AddTube(Cartoon.turnWidth, Cartoon.turnWidth);

                if (this.residue.IsStructureStart)
                    this.AddTubeCap(Cartoon.turnWidth, Cartoon.turnWidth);

                if (this.residue.IsStructureEnd)
                    this.AddTubeCap(Cartoon.turnWidth, Cartoon.turnWidth);
            }
        }

        /// <summary>
        /// The 3D model for this cartoon mesh.
        /// </summary>
        internal Model3DGroup Model { get { return this.model; } }

        /// <summary>
        /// Gets and sets the color of the model.
        /// </summary>
        internal Color Color
        {
            get { return ((SolidColorBrush)this.diffuseMaterial.Brush).Color; }
            set { ((SolidColorBrush)this.diffuseMaterial.Brush).Color = value; }
        }

        /// <summary>
        /// Performs hit testing for this cartoon mesh.
        /// </summary>
        /// <param name="rayHitTestResult">A 3D mesh hit test result from the WPF visual tree hit
        /// testing framework</param>
        /// <returns>
        /// True if the mesh hit belongs to this residue, otherwise false.
        /// </returns>
        internal bool HoverHitTest(RayMeshGeometry3DHitTestResult rayHitTestResult)
        {
            return this.model.Children.Contains(rayHitTestResult.ModelHit);
        }

        /// <summary>
        /// Creates a cylindrical tube along the spline path.
        /// </summary>
        /// <param name="width">The x-radius of the extrusion ellipse.</param>
        /// <param name="height">The y-radius of the extrusion ellipse.</param>
        private void AddTube(double width, double height)
        {
            GeometryModel3D tubeModel = new GeometryModel3D();
            tubeModel.Material = this.materialGroup;
            this.model.Children.Add(tubeModel);

            MeshGeometry3D tubeMesh = new MeshGeometry3D();
            tubeModel.Geometry = tubeMesh;

            for (int i = 0; i < this.ribbonPoints.Count; i++)
            {
                for (int j = 0; j < Cartoon.radialSegmentCount; j++)
                {
                    double t = 2 * Math.PI * j / Cartoon.radialSegmentCount;

                    Vector3D radialVector = width * Math.Cos(t) * this.torsionVectors[i] +
                        height * Math.Sin(t) * this.normalVectors[i];
                    tubeMesh.Positions.Add(this.ribbonPoints[i] + radialVector);

                    Vector3D normalVector = height * Math.Cos(t) * this.torsionVectors[i] +
                        width * Math.Sin(t) * this.normalVectors[i];
                    normalVector.Normalize();
                    tubeMesh.Normals.Add(normalVector);
                }
            }

            int rsc = Cartoon.radialSegmentCount;

            for (int i = 0; i < this.ribbonPoints.Count - 1; i++)
            {
                for (int j = 0; j < Cartoon.radialSegmentCount; j++)
                {
                    tubeMesh.TriangleIndices.Add(i * rsc + j);
                    tubeMesh.TriangleIndices.Add((i + 1) * rsc + (j + 1) % rsc);
                    tubeMesh.TriangleIndices.Add(i * rsc + (j + 1) % rsc);

                    tubeMesh.TriangleIndices.Add(i * rsc + j);
                    tubeMesh.TriangleIndices.Add((i + 1) * rsc + j);
                    tubeMesh.TriangleIndices.Add((i + 1) * rsc + (j + 1) % rsc);
                }
            }
        }

        /// <summary>
        /// Creates an elliptical cap for a tube along the spline path.
        /// </summary>
        /// <param name="width">The x-radius of the cap ellipse.</param>
        /// <param name="height">The y-radius of the cap ellipse.</param>
        private void AddTubeCap(double width, double height)
        {
            GeometryModel3D capModel = new GeometryModel3D();
            capModel.Material = this.materialGroup;
            this.model.Children.Add(capModel);

            MeshGeometry3D capMesh = new MeshGeometry3D();
            capModel.Geometry = capMesh;

            Vector3D normalVector = Vector3D.CrossProduct(
                this.torsionVectors[0], this.normalVectors[0]);

            if (this.residue.IsStructureEnd) normalVector.Negate();

            int offset = this.residue.IsStructureStart ? 0 : this.ribbonPoints.Count - 1;

            capMesh.Positions.Add(this.ribbonPoints[offset]);
            capMesh.Normals.Add(normalVector);

            for (int i = 0; i < Cartoon.radialSegmentCount; i++)
            {
                double t = 2 * Math.PI * i / Cartoon.radialSegmentCount;

                Vector3D radialVector = width * Math.Cos(t) * this.torsionVectors[offset] +
                    height * Math.Sin(t) * this.normalVectors[offset];
                capMesh.Positions.Add(this.ribbonPoints[offset] + radialVector);

                capMesh.Normals.Add(normalVector);

                capMesh.TriangleIndices.Add(0);

                if (this.residue.IsStructureStart)
                {
                    capMesh.TriangleIndices.Add(i + 1);
                    capMesh.TriangleIndices.Add((i + 1) % Cartoon.radialSegmentCount + 1);
                }
                else
                {
                    capMesh.TriangleIndices.Add((i + 1) % Cartoon.radialSegmentCount + 1);
                    capMesh.TriangleIndices.Add(i + 1);
                }
            }
        }

        /// <summary>
        /// Creates a rectangular solid sheet along the spline path.
        /// </summary>
        private void AddSheet()
        {
            GeometryModel3D sheetModel = new GeometryModel3D();
            sheetModel.Material = this.materialGroup;
            this.model.Children.Add(sheetModel);

            MeshGeometry3D sheetMesh = new MeshGeometry3D();
            sheetModel.Geometry = sheetMesh;

            double offsetLength = 0;

            if (this.residue.IsStructureEnd)
            {
                Vector3D lengthVector = this.ribbonPoints[this.ribbonPoints.Count - 1] -
                    this.ribbonPoints[0];
                offsetLength = Cartoon.arrowWidth / lengthVector.Length;
            }

            for (int i = 0; i < this.ribbonPoints.Count; i++)
            {
                double actualWidth = !this.residue.IsStructureEnd ? Cartoon.sheetWidth :
                    Cartoon.arrowWidth * (1 - (double)i / (this.ribbonPoints.Count - 1));

                Vector3D horizontalVector = actualWidth * this.torsionVectors[i];
                Vector3D verticalVector = Cartoon.sheetHeight * this.normalVectors[i];

                sheetMesh.Positions.Add(this.ribbonPoints[i] + horizontalVector + verticalVector);
                sheetMesh.Positions.Add(this.ribbonPoints[i] - horizontalVector + verticalVector);
                sheetMesh.Positions.Add(this.ribbonPoints[i] - horizontalVector + verticalVector);
                sheetMesh.Positions.Add(this.ribbonPoints[i] - horizontalVector - verticalVector);
                sheetMesh.Positions.Add(this.ribbonPoints[i] - horizontalVector - verticalVector);
                sheetMesh.Positions.Add(this.ribbonPoints[i] + horizontalVector - verticalVector);
                sheetMesh.Positions.Add(this.ribbonPoints[i] + horizontalVector - verticalVector);
                sheetMesh.Positions.Add(this.ribbonPoints[i] + horizontalVector + verticalVector);

                Vector3D normalOffset = new Vector3D();

                if (this.residue.IsStructureEnd)
                {
                    normalOffset = offsetLength * Vector3D.CrossProduct(
                        this.normalVectors[i], this.torsionVectors[i]);
                }

                sheetMesh.Normals.Add(this.normalVectors[i]);
                sheetMesh.Normals.Add(this.normalVectors[i]);
                sheetMesh.Normals.Add(-this.torsionVectors[i] + normalOffset);
                sheetMesh.Normals.Add(-this.torsionVectors[i] + normalOffset);
                sheetMesh.Normals.Add(-this.normalVectors[i]);
                sheetMesh.Normals.Add(-this.normalVectors[i]);
                sheetMesh.Normals.Add(this.torsionVectors[i] + normalOffset);
                sheetMesh.Normals.Add(this.torsionVectors[i] + normalOffset);
            }

            for (int i = 0; i < this.ribbonPoints.Count - 1; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    sheetMesh.TriangleIndices.Add(i * 8 + 2 * j);
                    sheetMesh.TriangleIndices.Add((i + 1) * 8 + 2 * j + 1);
                    sheetMesh.TriangleIndices.Add(i * 8 + 2 * j + 1);

                    sheetMesh.TriangleIndices.Add(i * 8 + 2 * j);
                    sheetMesh.TriangleIndices.Add((i + 1) * 8 + 2 * j);
                    sheetMesh.TriangleIndices.Add((i + 1) * 8 + 2 * j + 1);
                }
            }
        }

        /// <summary>
        /// Creates a flat cap or an arrow head cap for a sheet.
        /// </summary>
        private void AddSheetCap()
        {
            GeometryModel3D capModel = new GeometryModel3D();
            capModel.Material = this.materialGroup;
            this.model.Children.Add(capModel);

            MeshGeometry3D capMesh = new MeshGeometry3D();
            capModel.Geometry = capMesh;

            Vector3D horizontalVector = Cartoon.sheetWidth * this.torsionVectors[0];
            Vector3D verticalVector = Cartoon.sheetHeight * this.normalVectors[0];

            Point3D p1 = this.ribbonPoints[0] + horizontalVector + verticalVector;
            Point3D p2 = this.ribbonPoints[0] - horizontalVector + verticalVector;
            Point3D p3 = this.ribbonPoints[0] - horizontalVector - verticalVector;
            Point3D p4 = this.ribbonPoints[0] + horizontalVector - verticalVector;

            if (this.residue.IsStructureStart)
            {
                this.AddSheetCapSection(capMesh, p1, p2, p3, p4);
            }
            else
            {
                Vector3D arrowHorizontalVector = Cartoon.arrowWidth * this.torsionVectors[0];

                Point3D p5 = this.ribbonPoints[0] + arrowHorizontalVector + verticalVector;
                Point3D p6 = this.ribbonPoints[0] - arrowHorizontalVector + verticalVector;
                Point3D p7 = this.ribbonPoints[0] - arrowHorizontalVector - verticalVector;
                Point3D p8 = this.ribbonPoints[0] + arrowHorizontalVector - verticalVector;

                this.AddSheetCapSection(capMesh, p5, p1, p4, p8);
                this.AddSheetCapSection(capMesh, p2, p6, p7, p3);
            }
        }

        /// <summary>
        /// Helper method to add a quadrilateral surface for a sheet cap.
        /// </summary>
        /// <param name="capMesh">The mesh to add the triangles to.</param>
        /// <param name="p1">Point 1.</param>
        /// <param name="p2">Point 2.</param>
        /// <param name="p3">Point 3.</param>
        /// <param name="p4">Point 4.</param>
        private void AddSheetCapSection(
            MeshGeometry3D capMesh, Point3D p1, Point3D p2, Point3D p3, Point3D p4)
        {
            int indexOffset = capMesh.Positions.Count;

            capMesh.Positions.Add(p1);
            capMesh.Positions.Add(p2);
            capMesh.Positions.Add(p3);
            capMesh.Positions.Add(p4);

            capMesh.TriangleIndices.Add(indexOffset + 0);
            capMesh.TriangleIndices.Add(indexOffset + 1);
            capMesh.TriangleIndices.Add(indexOffset + 2);

            capMesh.TriangleIndices.Add(indexOffset + 2);
            capMesh.TriangleIndices.Add(indexOffset + 3);
            capMesh.TriangleIndices.Add(indexOffset + 0);

            Vector3D normalVector = Vector3D.CrossProduct(p2 - p1, p4 - p1);
            normalVector.Normalize();

            for (int i = 0; i < 4; i++)
                capMesh.Normals.Add(normalVector);
        }
    }
}
