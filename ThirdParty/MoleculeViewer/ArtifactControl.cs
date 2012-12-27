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

using System.Windows.Controls;
using System.Xml;
using System.Windows.Media;

namespace MoleculeViewer
{
    /// <summary>
    /// Provides a UI container for a view of a PDB that can switch the displayed entity.
    /// </summary>
    public class ArtifactControl : ContentControl
    {
        private Border border;
        private PdbViewer pdbViewer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactControl"/> class.
        /// </summary>
        public ArtifactControl()
        {
            this.border = new Border();
            this.border.Background = Brushes.Black;
            this.Content = this.border;

            this.pdbViewer = new PdbViewer();
            this.border.Child = this.pdbViewer;
        }

        /// <summary>
        /// Gets the xml state data for the control's current view.
        /// </summary>
        /// <value>The view state xml.</value>
        public XmlNode ViewState
        {
            get
            {
                ViewState viewState = this.pdbViewer.GetViewState();
                return viewState != null ? viewState.CreateXmlNode() : null;
            }
        }

        /// <summary>
        /// Opens the specified entity.
        /// </summary>
        /// <param name="entity">The entity to open.</param>
        public void OpenArtifact(LocalEntity entity)
        {
            this.CloseArtifact();

            this.pdbViewer.Entity = entity;
        }

        /// <summary>
        /// Closes the currently displayed artifact.
        /// </summary>
        public void CloseArtifact()
        {
            this.pdbViewer.Entity = null;
        }
    }
}
