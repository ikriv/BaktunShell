/* Copyright 2009 Ivan Krivyakov

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Ikriv.Surfaces
{
    public abstract class Surface : ModelVisual3D
    {
        public Surface()
        {
            Content = _content;
            _content.Geometry = CreateMesh();
        }

        public static PropertyHolder<Material, Surface> MaterialProperty =
            new PropertyHolder<Material,Surface>("Material", null, OnMaterialChanged);

        public static PropertyHolder<Material, Surface> BackMaterialProperty =
            new PropertyHolder<Material, Surface>("BackMaterial", null, OnBackMaterialChanged);

        public static PropertyHolder<bool, Surface> VisibleProperty =
            new PropertyHolder<bool, Surface>("Visible", true, OnVisibleChanged);

        public Material Material
        {
            get { return MaterialProperty.Get(this); }
            set { MaterialProperty.Set(this, value); }
        }

        public Material BackMaterial
        {
            get { return BackMaterialProperty.Get(this); }
            set { BackMaterialProperty.Set(this, value); }
        }

        public bool Visible
        {
            get { return VisibleProperty.Get(this); }
            set { VisibleProperty.Set(this, value); }
        }

        private static void OnMaterialChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            ((Surface)sender).OnMaterialChanged();
        }

        private static void OnBackMaterialChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            ((Surface)sender).OnBackMaterialChanged();
        }

        private static void OnVisibleChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            ((Surface)sender).OnVisibleChanged();
        }

        protected static void OnGeometryChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            ((Surface)sender).OnGeometryChanged();
        }

        private void OnMaterialChanged()
        {
            SetContentMaterial();
        }

        private void OnBackMaterialChanged()
        {
            SetContentBackMaterial();
        }

        private void OnVisibleChanged()
        {
            SetContentMaterial();
            SetContentBackMaterial();
        }

        private void SetContentMaterial()
        {
            _content.Material = Visible ? Material : null;
        }

        private void SetContentBackMaterial()
        {
            _content.BackMaterial = Visible ? BackMaterial : null;
        }

        private void OnGeometryChanged()
        {
            _content.Geometry = CreateMesh();
        }

        protected abstract Geometry3D CreateMesh();

        private readonly GeometryModel3D _content = new GeometryModel3D();
    }
}
