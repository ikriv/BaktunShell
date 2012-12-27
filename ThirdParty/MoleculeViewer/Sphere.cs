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

using System.Windows.Media.Media3D;

namespace MoleculeViewer
{
    /// <summary>
    /// Static class that generates the mesh for a sphere the first time one is needed.
    /// </summary>
    /// <remarks>
    /// The tessellation alorithm is based on recursive octahedron subdivision to avoid noticable
    /// symmetry artifacts since the polygon count is low.
    /// </remarks>
    internal static class Sphere
    {
        private const int divisions = 3;

        private static MeshGeometry3D mesh;
        private static int nextIndex;

        /// <summary>
        /// Static constructor to generate the mesh when first needed. Creates the faces of the
        /// base octahedron and calls <see cref="Divide" /> to initiate the recursive subdivision.
        /// </summary>
        static Sphere()
        {
            Sphere.mesh = new MeshGeometry3D();

            Sphere.AddBaseTriangle(
                new Point3D(0, 0, 1), new Point3D(1, 0, 0), new Point3D(0, 1, 0));
            Sphere.AddBaseTriangle(
                new Point3D(1, 0, 0), new Point3D(0, 0, -1), new Point3D(0, 1, 0));
            Sphere.AddBaseTriangle(
                new Point3D(0, 0, -1), new Point3D(-1, 0, 0), new Point3D(0, 1, 0));
            Sphere.AddBaseTriangle(
                new Point3D(-1, 0, 0), new Point3D(0, 0, 1), new Point3D(0, 1, 0));
            Sphere.AddBaseTriangle(
                new Point3D(1, 0, 0), new Point3D(0, 0, 1), new Point3D(0, -1, 0));
            Sphere.AddBaseTriangle(
                new Point3D(0, 0, -1), new Point3D(1, 0, 0), new Point3D(0, -1, 0));
            Sphere.AddBaseTriangle(
                new Point3D(-1, 0, 0), new Point3D(0, 0, -1), new Point3D(0, -1, 0));
            Sphere.AddBaseTriangle(
                new Point3D(0, 0, 1), new Point3D(-1, 0, 0), new Point3D(0, -1, 0));

            for (int division = 1; division < Sphere.divisions; division++) Sphere.Divide();

            Sphere.mesh.Freeze();
        }

        /// <summary>
        /// Gets the spherical mesh.
        /// </summary>
        internal static MeshGeometry3D Mesh { get { return Sphere.mesh; } }

        /// <summary>
        /// Helper function to create a face for the base octahedron.
        /// </summary>
        /// <param name="p1">Vertex 1.</param>
        /// <param name="p2">Vertex 2.</param>
        /// <param name="p3">Vertex 3.</param>
        private static void AddBaseTriangle(Point3D p1, Point3D p2, Point3D p3)
        {
            Sphere.mesh.Positions.Add(p1);
            Sphere.mesh.Positions.Add(p2);
            Sphere.mesh.Positions.Add(p3);

            Sphere.mesh.Normals.Add(new Vector3D(p1.X, p1.Y, p1.Z));
            Sphere.mesh.Normals.Add(new Vector3D(p2.X, p2.Y, p2.Z));
            Sphere.mesh.Normals.Add(new Vector3D(p3.X, p3.Y, p3.Z));

            Sphere.mesh.TriangleIndices.Add(Sphere.nextIndex++);
            Sphere.mesh.TriangleIndices.Add(Sphere.nextIndex++);
            Sphere.mesh.TriangleIndices.Add(Sphere.nextIndex++);
        }

        /// <summary>
        /// Performs the recursive subdivision.
        /// </summary>
        private static void Divide()
        {
            int indexCount = Sphere.mesh.TriangleIndices.Count;

            for (int indexOffset = 0; indexOffset < indexCount; indexOffset += 3)
                Sphere.DivideTriangle(indexOffset);
        }

        /// <summary>
        /// Replaces a triange at a given index buffer offset and replaces it with four triangles
        /// that compose an equilateral subdivision.
        /// </summary>
        /// <param name="indexOffset">An offset into the index buffer.</param>
        private static void DivideTriangle(int indexOffset)
        {
            int i1 = Sphere.mesh.TriangleIndices[indexOffset];
            int i2 = Sphere.mesh.TriangleIndices[indexOffset + 1];
            int i3 = Sphere.mesh.TriangleIndices[indexOffset + 2];

            Point3D p1 = Sphere.mesh.Positions[i1];
            Point3D p2 = Sphere.mesh.Positions[i2];
            Point3D p3 = Sphere.mesh.Positions[i3];
            Point3D p4 = Sphere.GetNormalizedMidpoint(p1, p2);
            Point3D p5 = Sphere.GetNormalizedMidpoint(p2, p3);
            Point3D p6 = Sphere.GetNormalizedMidpoint(p3, p1);

            Sphere.mesh.Positions.Add(p4);
            Sphere.mesh.Positions.Add(p5);
            Sphere.mesh.Positions.Add(p6);

            Sphere.mesh.Normals.Add(new Vector3D(p4.X, p4.Y, p4.Z));
            Sphere.mesh.Normals.Add(new Vector3D(p5.X, p5.Y, p5.Z));
            Sphere.mesh.Normals.Add(new Vector3D(p6.X, p6.Y, p6.Z));

            int i4 = Sphere.nextIndex++;
            int i5 = Sphere.nextIndex++;
            int i6 = Sphere.nextIndex++;

            Sphere.mesh.TriangleIndices[indexOffset] = i4;
            Sphere.mesh.TriangleIndices[indexOffset + 1] = i5;
            Sphere.mesh.TriangleIndices[indexOffset + 2] = i6;

            Sphere.mesh.TriangleIndices.Add(i1);
            Sphere.mesh.TriangleIndices.Add(i4);
            Sphere.mesh.TriangleIndices.Add(i6);

            Sphere.mesh.TriangleIndices.Add(i4);
            Sphere.mesh.TriangleIndices.Add(i2);
            Sphere.mesh.TriangleIndices.Add(i5);

            Sphere.mesh.TriangleIndices.Add(i6);
            Sphere.mesh.TriangleIndices.Add(i5);
            Sphere.mesh.TriangleIndices.Add(i3);
        }

        /// <summary>
        /// Calculates the midpoint between two points on a unit sphere and projects the result
        /// back to the surface of the sphere.
        /// </summary>
        /// <param name="p1">Point 1.</param>
        /// <param name="p2">Point 2.</param>
        /// <returns>The normalized midpoint.</returns>
        private static Point3D GetNormalizedMidpoint(Point3D p1, Point3D p2)
        {
            Vector3D vector = new Vector3D(
                (p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2, (p1.Z + p2.Z) / 2);
            vector.Normalize();

            return new Point3D(vector.X, vector.Y, vector.Z);
        }
    }
}
