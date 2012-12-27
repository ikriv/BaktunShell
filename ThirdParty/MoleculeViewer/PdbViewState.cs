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
using System.Windows.Media.Media3D;
using System.Xml;

namespace MoleculeViewer
{
    /// <summary>
    /// Represents the state of camera settings and transformations of a 3D view of a PDB.
    /// </summary>
    internal class PdbViewState : ViewState
    {
        private Vector3D translation;
        private double scale;
        private Quaternion rotation;
        private double clip;
        private double slab;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdbViewState"/> class.
        /// </summary>
        internal PdbViewState()
        {
            this.translation = new Vector3D();
            this.scale = 1;
            this.rotation = Quaternion.Identity;
            this.clip = 1;
            this.slab = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdbViewState"/> class.
        /// </summary>
        /// <param name="xmlNode">The XML node representing the initial ViewState.</param>
        internal PdbViewState(XmlNode xmlNode)
        {
            try
            {
                this.translation =
                    Vector3D.Parse(xmlNode.SelectSingleNode("Translation").InnerText);
            }
            catch { this.translation = new Vector3D(); }

            try { this.scale = Double.Parse(xmlNode.SelectSingleNode("Scale").InnerText); }
            catch { this.scale = 1; }

            try {
                this.rotation =
                    Quaternion.Parse(xmlNode.SelectSingleNode("Rotation").InnerText);
            }
            catch { this.rotation = Quaternion.Identity; }

            try { this.clip = Double.Parse(xmlNode.SelectSingleNode("Clip").InnerText); }
            catch { this.clip = 1; }

            try { this.slab = Double.Parse(xmlNode.SelectSingleNode("Slab").InnerText); }
            catch { this.slab = 0; }
        }

        /// <summary>
        /// Gets or sets the translation.
        /// </summary>
        /// <value>The 3D translation vector.</value>
        internal Vector3D Translation
        {
            get { return this.translation; }
            set { this.translation = value; }
        }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale ratio.</value>
        internal double Scale
        {
            get { return this.scale; }
            set { this.scale = value; }
        }

        /// <summary>
        /// Gets or sets the rotation quaternion.
        /// </summary>
        /// <value>The 3D rotation quaternion.</value>
        internal Quaternion Rotation
        {
            get { return this.rotation; }
            set { this.rotation = value; }
        }

        /// <summary>
        /// Gets or sets the clip.
        /// </summary>
        /// <value>The clip value.</value>
        internal double Clip
        {
            get { return this.clip; }
            set { this.clip = value; }
        }

        /// <summary>
        /// Gets or sets the slab.
        /// </summary>
        /// <value>The slab value.</value>
        internal double Slab
        {
            get { return this.slab; }
            set { this.slab = value; }
        }

        /// <summary>
        /// Creates the XML node representing the ViewState.
        /// </summary>
        /// <returns>An XML node representing this instance.</returns>
        internal override XmlNode CreateXmlNode()
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement viewStateElement = xmlDocument.CreateElement("PdbViewState");

            XmlElement translationElement = xmlDocument.CreateElement("Translation");
            translationElement.InnerText = this.translation.ToString();
            viewStateElement.AppendChild(translationElement);

            XmlElement scaleElement = xmlDocument.CreateElement("Scale");
            scaleElement.InnerText = this.scale.ToString();
            viewStateElement.AppendChild(scaleElement);

            XmlElement rotationElement = xmlDocument.CreateElement("Rotation");
            rotationElement.InnerText = this.rotation.ToString();
            viewStateElement.AppendChild(rotationElement);

            XmlElement clipElement = xmlDocument.CreateElement("Clip");
            clipElement.InnerText = this.clip.ToString();
            viewStateElement.AppendChild(clipElement);

            XmlElement slabElement = xmlDocument.CreateElement("Slab");
            slabElement.InnerText = this.slab.ToString();
            viewStateElement.AppendChild(slabElement);

            return viewStateElement;
        }

        /// <summary>
        /// Creates the tween of the instance and the specified end ViewState.
        /// </summary>
        /// <param name="endViewState">End <see cref="ViewState"/> of the tween.</param>
        /// <param name="t">The interpolation coefficient.</param>
        /// <returns>
        /// A <see cref="ViewState"/> representing the tween of this instance and the specifed end ViewState.
        /// </returns>
        protected override ViewState CreateTween(ViewState endViewState, double t)
        {
            PdbViewState tweenViewState = new PdbViewState();
            PdbViewState otherViewState = endViewState as PdbViewState;

            if (otherViewState != null)
            {
                tweenViewState.translation = this.translation +
                    t * (otherViewState.translation - this.translation);
                tweenViewState.scale = this.scale + t * (otherViewState.scale - this.scale);
                tweenViewState.clip = this.clip + t * (otherViewState.clip - this.clip);
                tweenViewState.slab = this.slab + t * (otherViewState.slab - this.slab);

                tweenViewState.rotation = Quaternion.Slerp(
                    this.rotation, otherViewState.rotation, t);
            }

            return tweenViewState;
        }
    }
}
