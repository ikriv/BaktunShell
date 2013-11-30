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
// THIS CODE AND INFORMATION ARE PROVIDED ""AS IS"" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//=============================================================================

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MoleculeViewer
{
    /// <summary>
    /// Control to render a structural element of a molecule in 3D, 
    /// applying camera settings and transforms.
    /// </summary>
    internal class StructureControl : MoleculeViewControl
    {
        private const double cameraOffset = 4;

        private PdbViewer pdbViewer;
        private double clip;
        private double slab;
        private Point previousMousePoint;
        private Vector3D previousMouseVector;
        private UIElement captureElement;
        private Viewport3D viewport;
        private PerspectiveCamera camera;
        private ModelVisual3D moleculeVisual;
        private Rectangle selectionRectangle;
        private TranslateTransform3D translateTransform;
        private ScaleTransform3D scaleTransform;
        private RotateTransform3D rotateTransform;

        private Label testLabel;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureControl"/> class and sets up visual elements for rendering.
        /// </summary>
        /// <param name="pdbViewer">The PDB viewer.</param>
        internal StructureControl(PdbViewer pdbViewer)
        {
            this.pdbViewer = pdbViewer;

            NameScope.SetNameScope(this, new NameScope());

            this.viewport = new Viewport3D();
            this.viewport.ClipToBounds = true;
            this.Children.Add(this.viewport);

            this.camera = new PerspectiveCamera();
            this.camera.Position = new Point3D(0, 0, cameraOffset);
            this.viewport.Camera = this.camera;

            ModelVisual3D lightingVisual = new ModelVisual3D();
            this.viewport.Children.Add(lightingVisual);

            Model3DGroup lightingModel = new Model3DGroup();
            lightingVisual.Content = lightingModel;

            PointLight pointLight = new PointLight(Colors.White, new Point3D(-4, 4, 8));
            lightingModel.Children.Add(pointLight);

            AmbientLight ambientLight = new AmbientLight(Color.FromRgb(32, 32, 32));
            lightingModel.Children.Add(ambientLight);

            this.moleculeVisual = new ModelVisual3D();
            viewport.Children.Add(this.moleculeVisual);

            Transform3DGroup transformGroup = new Transform3DGroup();
            this.moleculeVisual.Transform = transformGroup;

            this.translateTransform = new TranslateTransform3D();
            transformGroup.Children.Add(this.translateTransform);

            this.scaleTransform = new ScaleTransform3D();
            transformGroup.Children.Add(this.scaleTransform);

            this.rotateTransform = new RotateTransform3D();
            this.rotateTransform.Rotation = new QuaternionRotation3D();
            transformGroup.Children.Add(this.rotateTransform);

            this.selectionRectangle = new Rectangle();
            this.selectionRectangle.Stroke = Brushes.White;
            this.selectionRectangle.Fill = new SolidColorBrush(Color.FromArgb(32, 255, 255, 255));
            this.selectionRectangle.Visibility = Visibility.Hidden;
            this.Children.Add(this.selectionRectangle);

            this.testLabel = new Label();
            this.testLabel.Foreground = Brushes.White;
            this.testLabel.FontSize = 20;
            this.testLabel.HorizontalAlignment = HorizontalAlignment.Left;
            this.testLabel.VerticalAlignment = VerticalAlignment.Center;
            this.Children.Add(this.testLabel);

            this.clip = 1;
            this.slab = 0;
            this.UpdateClipping();
        }

        /// <summary>
        /// Gets or sets the capture element to associate with this control.
        /// </summary>
        /// <value>The capture element.</value>
        internal UIElement CaptureElement
        {
            get
            {
                return this.captureElement;
            }
            set
            {
                this.captureElement = value;

                this.captureElement.MouseLeftButtonDown += this.CaptureMouseLeftButtonDown;
                this.captureElement.MouseLeftButtonUp += this.CaptureMouseLeftButtonUp;
                this.captureElement.MouseRightButtonDown += this.CaptureMouseRightButtonDown;
                this.captureElement.MouseRightButtonUp += this.CaptureMouseRightButtonUp;
                this.captureElement.MouseMove += this.CaptureMouseMove;
                this.captureElement.MouseWheel += this.CaptureMouseWheel;
            }
        }

        /// <summary>
        /// Performs hit testing for the displayed molecule.
        /// </summary>
        /// <returns>The active <see cref="Atom"/> or <see cref="Residue"/> if any.</returns>
        internal HoverObject HoverHitTest()
        {
            if (!this.captureElement.IsEnabled) return null;

            HitTestResult hitTestResult =
                VisualTreeHelper.HitTest(this.viewport, Mouse.GetPosition(this));
            RayMeshGeometry3DHitTestResult rayHitTestResult =
                hitTestResult as RayMeshGeometry3DHitTestResult;

            if (rayHitTestResult != null)
            {
                foreach (Atom atom in this.Molecule.Atoms)
                    if (atom.HoverHitTest(rayHitTestResult))
                        return atom;

                foreach (Residue residue in this.Molecule.Residues)
                    if (residue.HoverHitTest(rayHitTestResult))
                        return residue;
            }

            return null;
        }

        /// <summary>
        /// Gets the current <see cref="ViewState"/> of the control instance.
        /// </summary>
        /// <returns>The current <see cref="PdbViewState"/>.</returns>
        internal PdbViewState GetViewState()
        {
            PdbViewState viewState = new PdbViewState();

            viewState.Translation = new Vector3D(this.translateTransform.OffsetX,
                this.translateTransform.OffsetY, this.translateTransform.OffsetZ);
            viewState.Rotation = ((QuaternionRotation3D)this.rotateTransform.Rotation).Quaternion;
            viewState.Scale = this.scaleTransform.ScaleX;
            viewState.Clip = this.clip;
            viewState.Slab = this.slab;

            return viewState;
        }

        /// <summary>
        /// Starts the view state animation.
        /// </summary>
        internal void StartViewStateAnimation()
        {
            this.captureElement.IsEnabled = false;

            this.pdbViewer.ActionType = PdbActionType.Animating;
        }

        /// <summary>
        /// Indicates whether the ViewState animation is in progress.
        /// </summary>
        /// <returns>true if the viewer is animating, otherwise false</returns>
        internal bool ContinueViewStateAnimation()
        {
            return this.pdbViewer.ActionType == PdbActionType.Animating;
        }

        /// <summary>
        /// Stops the view state animation.
        /// </summary>
        internal void StopViewStateAnimation()
        {
            this.pdbViewer.PerformHoverHitTest();

            if (this.pdbViewer.ActionType == PdbActionType.Animating)
                this.pdbViewer.ActionType = PdbActionType.None;

            this.captureElement.IsEnabled = true;
        }

        /// <summary>
        /// Sets all camera and transform settings based on the specified <see cref="PdbViewState"/>.
        /// </summary>
        /// <param name="pdbViewState">The new state of the view.</param>
        internal void SetViewState(PdbViewState pdbViewState)
        {
            this.translateTransform.OffsetX = pdbViewState.Translation.X;
            this.translateTransform.OffsetY = pdbViewState.Translation.Y;
            this.translateTransform.OffsetZ = pdbViewState.Translation.Z;

            this.scaleTransform.ScaleX = pdbViewState.Scale;
            this.scaleTransform.ScaleY = pdbViewState.Scale;
            this.scaleTransform.ScaleZ = pdbViewState.Scale;

            QuaternionRotation3D rotation =
                (QuaternionRotation3D)this.rotateTransform.Rotation;
            rotation.Quaternion = pdbViewState.Rotation;

            this.clip = pdbViewState.Clip;
            this.slab = pdbViewState.Slab;

            this.UpdateClipping();
        }

        /// <summary>
        /// Called when the molecule is changed.
        /// </summary>
        protected override void OnMoleculeChanged()
        {
            if (this.Molecule != null)
            {
                this.moleculeVisual.Content = this.Molecule.Model;

                this.SetViewState(new PdbViewState());
            }
            else
            {
                this.moleculeVisual.Content = null;
            }

            this.captureElement.IsEnabled = true;
        }

        private void CaptureMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.pdbViewer.ActionType == PdbActionType.None)
            {
                this.CaptureElement.CaptureMouse();

                this.previousMousePoint = e.GetPosition(this);
                this.previousMouseVector = this.ProjectToTrackball(this.previousMousePoint);

                if (Keyboard.Modifiers == ModifierKeys.Shift)
                    this.pdbViewer.ActionType = PdbActionType.Select;
                else if (Keyboard.Modifiers == ModifierKeys.Control)
                    this.pdbViewer.ActionType = PdbActionType.Deselect;
                else
                    this.pdbViewer.ActionType = PdbActionType.Rotate;
            }
        }

        private void CaptureMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.pdbViewer.ActionType == PdbActionType.Rotate ||
                this.pdbViewer.ActionType == PdbActionType.Select ||
                this.pdbViewer.ActionType == PdbActionType.Deselect)
            {

                if (this.pdbViewer.ActionType == PdbActionType.Select ||
                    this.pdbViewer.ActionType == PdbActionType.Deselect)
                {
                    if (this.selectionRectangle.Visibility == Visibility.Hidden)
                    {
                        HoverObject hoverObject = this.HoverHitTest();

                        if (hoverObject is Atom)
                            ((Atom)hoverObject).IsSelected = this.pdbViewer.ActionType ==
                                PdbActionType.Select;

                        if (hoverObject is Residue)
                            ((Residue)hoverObject).Selection = this.pdbViewer.ActionType ==
                                PdbActionType.Select ? Residue.SelectionType.Full :
                                Residue.SelectionType.None;
                    }
                    else
                    {
                        this.selectionRectangle.Visibility = Visibility.Hidden;

                        foreach (Atom atom in this.Molecule.Atoms)
                            atom.IsSelected = atom.ShowAsSelected;
                    }
                }

                this.pdbViewer.ActionType = PdbActionType.None;
                this.CaptureElement.ReleaseMouseCapture();
            }
        }

        private void CaptureMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.pdbViewer.ActionType == PdbActionType.None)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    this.pdbViewer.ActionType = PdbActionType.Reset;
                }
                else if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    this.CaptureElement.CaptureMouse();

                    this.previousMousePoint = e.GetPosition(this);

                    this.pdbViewer.ActionType = PdbActionType.Translate;
                }
            }
        }

        private void CaptureMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.pdbViewer.ActionType == PdbActionType.Reset)
            {
                this.pdbViewer.ActionType = PdbActionType.None;
                this.pdbViewer.AnimateToViewState(new PdbViewState());
            }
            else if (this.pdbViewer.ActionType == PdbActionType.Translate)
            {
                this.pdbViewer.ActionType = PdbActionType.None;
                this.CaptureElement.ReleaseMouseCapture();
            }
        }

        private void CaptureMouseMove(object sender, MouseEventArgs e)
        {
            Point currentMousePoint = e.GetPosition(this);

            if (this.pdbViewer.ActionType == PdbActionType.Rotate)
            {
                Vector3D currentMouseVector3D = this.ProjectToTrackball(currentMousePoint);

                Vector3D axis = Vector3D.CrossProduct(
                    this.previousMouseVector, currentMouseVector3D);
                double angle = 2 * Vector3D.AngleBetween(
                    this.previousMouseVector, currentMouseVector3D);

                if (axis.LengthSquared > 0)
                {
                    QuaternionRotation3D rotation =
                        this.rotateTransform.Rotation as QuaternionRotation3D;

                    if (rotation != null)
                    {
                        rotation.Quaternion = new Quaternion(axis, angle) *
                            rotation.Quaternion;
                    }
                }

                this.previousMouseVector = currentMouseVector3D;
            }
            else if (this.pdbViewer.ActionType == PdbActionType.Select ||
                this.pdbViewer.ActionType == PdbActionType.Deselect &&
                currentMousePoint != this.previousMousePoint)
            {
                double left = Math.Min(this.previousMousePoint.X, currentMousePoint.X);
                double top = Math.Min(this.previousMousePoint.Y, currentMousePoint.Y);
                double right = this.ActualWidth -
                    Math.Max(this.previousMousePoint.X, currentMousePoint.X);
                double bottom = this.ActualHeight -
                    Math.Max(this.previousMousePoint.Y, currentMousePoint.Y);

                left = Math.Max(0, left);
                top = Math.Max(this.CaptureElement.TranslatePoint(new Point(), this).Y, top);
                right = Math.Max(0, right);
                bottom = Math.Max(0, bottom);

                this.selectionRectangle.Margin = new Thickness(left, top, right, bottom);
                this.selectionRectangle.Visibility = Visibility.Visible;

                Rect selectionRect = new Rect(this.previousMousePoint, currentMousePoint);

                foreach (Atom atom in this.Molecule.Atoms)
                {
                    Point? atomPoint = this.GetViewportCoordinatePoint3Ds(atom);

                    bool contained = atomPoint != null && selectionRect.Contains(atomPoint.Value);

                    if (contained)
                    {
                        if (this.pdbViewer.ActionType == PdbActionType.Select)
                            atom.ShowAsSelected = true;
                        else if (this.pdbViewer.ActionType == PdbActionType.Deselect)
                            atom.ShowAsSelected = false;
                    }
                    else
                    {
                        atom.ShowAsSelected = atom.IsSelected;
                    }
                }
            }
            else if (this.pdbViewer.ActionType == PdbActionType.Translate)
            {
                double multiplier = 2 * Math.Tan(Math.PI / 8) * cameraOffset /
                    this.scaleTransform.ScaleX;

                double deltaX = (currentMousePoint.X - this.previousMousePoint.X) /
                    this.viewport.ActualWidth * multiplier;
                double deltaY = -(currentMousePoint.Y - this.previousMousePoint.Y) /
                    this.viewport.ActualHeight * multiplier;

                Vector3D deltaVector = new Vector3D(deltaX, deltaY, 0);

                QuaternionRotation3D rotation =
                    (QuaternionRotation3D)this.rotateTransform.Rotation;

                Matrix3D matrix = new Matrix3D();
                matrix.Rotate(rotation.Quaternion);
                matrix.Invert();

                deltaVector = matrix.Transform(deltaVector);

                this.translateTransform.OffsetX += deltaVector.X;
                this.translateTransform.OffsetY += deltaVector.Y;
                this.translateTransform.OffsetZ += deltaVector.Z;

                this.previousMousePoint = currentMousePoint;
            }
        }

        private void CaptureMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.pdbViewer.ActionType == PdbActionType.None)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    this.pdbViewer.ActionType = PdbActionType.Clip;

                    this.clip = Math.Max(0, Math.Min(1, this.clip + (double)e.Delta / 1200));
                    this.UpdateClipping();

                    this.pdbViewer.ActionType = PdbActionType.None;
                }
                else if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    this.pdbViewer.ActionType = PdbActionType.Slab;

                    this.slab = Math.Max(-1, Math.Min(1, this.slab + (double)e.Delta / 600));
                    this.UpdateClipping();

                    this.pdbViewer.ActionType = PdbActionType.None;
                }
                else
                {
                    this.pdbViewer.ActionType = PdbActionType.Zoom;

                    double multiplier = Math.Exp((double)e.Delta / -1000);
                    double scale = Math.Max(0.01, Math.Min(200,
                        this.scaleTransform.ScaleX * multiplier));

                    this.scaleTransform.ScaleX = scale;
                    this.scaleTransform.ScaleY = scale;
                    this.scaleTransform.ScaleZ = scale;

                    this.UpdateClipping();

                    this.pdbViewer.ActionType = PdbActionType.None;
                }
            }
        }

        private Vector3D ProjectToTrackball(Point point)
        {
            double x = 2 * point.X / this.viewport.ActualWidth - 1;
            double y = 1 - 2 * point.Y / this.viewport.ActualHeight;

            double zSquared = 1 - x * x - y * y;
            double z = zSquared > 0 ? Math.Sqrt(zSquared) : 0;

            return new Vector3D(x, y, z);
        }

        private Point? GetViewportCoordinatePoint3Ds(Atom atom)
        {
            Point3D position = this.Molecule.MoleculeTransform.Value.Transform(atom.Position);
            position = this.moleculeVisual.Transform.Value.Transform(position);

            if (position.Z > cameraOffset -
                ((ProjectionCamera)this.viewport.Camera).NearPlaneDistance)
                return null;

            if (position.Z < cameraOffset -
                ((ProjectionCamera)this.viewport.Camera).FarPlaneDistance)
                return null;

            double a = Math.Tan(Math.PI / 8) * (cameraOffset - position.Z);

            double x = (position.X / a + 1) * this.ActualWidth / 2;
            double y = (1 - position.Y / a * this.ActualWidth / this.ActualHeight) *
                this.ActualHeight / 2;

            return new Point(x, y);
        }

        private void UpdateClipping()
        {
            double clipRadius = 1.75 * this.clip * this.scaleTransform.ScaleX;
            double clipOffset = 1.75 * (1 - this.clip) * this.slab * this.scaleTransform.ScaleX;

            this.camera.NearPlaneDistance =
                Math.Max(0.125, cameraOffset + clipOffset - clipRadius);
            this.camera.FarPlaneDistance = cameraOffset + clipOffset + clipRadius;
        }
    }
}
