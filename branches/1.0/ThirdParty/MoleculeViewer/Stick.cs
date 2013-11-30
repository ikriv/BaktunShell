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
    /// Static class that generates the mesh for a cylindrical stick with pseudo-rounded ends the
    /// first time one is needed.
    /// </summary>
    internal static class Stick
    {
        private const int divisions = 8;
        private const double radius = 0.2;

        private static MeshGeometry3D shortMesh;
        private static MeshGeometry3D longMesh;

        /// <summary>
        /// Static constructor to generate the mesh when first needed.
        /// </summary>
        static Stick()
        {
            Stick.shortMesh = Stick.CreateMesh(Stick.radius);
            Stick.longMesh = Stick.CreateMesh(Stick.radius / 2);
        }

        /// <summary>
        /// Gets the short, fat mesh.
        /// </summary>
        internal static MeshGeometry3D ShortMesh { get { return Stick.shortMesh; } }

        /// <summary>
        /// Gets the long, skinny mesh.
        /// </summary>
        internal static MeshGeometry3D LongMesh { get { return Stick.longMesh; } }

        /// <summary>
        /// Populates the vertex and index buffers.
        /// </summary>
        /// <param name="capOffset">The distance from the end of the cylinder to the end of the
        /// pseudo-rounded end.</param>
        /// <returns>The mesh.</returns>
        private static MeshGeometry3D CreateMesh(double capOffset)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            int ic1 = 2 * divisions;
            int ic2 = ic1 + 1;

            for (int division = 0; division < divisions; division++)
            {
                double theta = 2 * Math.PI * division / divisions;

                double z = Stick.radius * Math.Cos(theta);
                double y = Stick.radius * Math.Sin(theta);

                mesh.Positions.Add(new Point3D(0, y, z));
                mesh.Positions.Add(new Point3D(1, y, z));

                mesh.Normals.Add(new Vector3D(0, y, z));
                mesh.Normals.Add(new Vector3D(0, y, z));

                int i1 = 2 * division;
                int i2 = i1 + 1;
                int i3 = 2 * ((division + 1) % divisions);
                int i4 = i3 + 1;

                mesh.TriangleIndices.Add(i1);
                mesh.TriangleIndices.Add(i2);
                mesh.TriangleIndices.Add(i3);

                mesh.TriangleIndices.Add(i3);
                mesh.TriangleIndices.Add(i2);
                mesh.TriangleIndices.Add(i4);

                mesh.TriangleIndices.Add(i2);
                mesh.TriangleIndices.Add(ic1);
                mesh.TriangleIndices.Add(i4);

                mesh.TriangleIndices.Add(i3);
                mesh.TriangleIndices.Add(ic2);
                mesh.TriangleIndices.Add(i1);
            }

            mesh.Positions.Add(new Point3D(1, 0, 0));
            mesh.Normals.Add(new Vector3D(1, 0, 0));

            mesh.Positions.Add(new Point3D(-capOffset, 0, 0));
            mesh.Normals.Add(new Vector3D(-1, 0, 0));

            mesh.Freeze();

            return mesh;
        }
    }
}
