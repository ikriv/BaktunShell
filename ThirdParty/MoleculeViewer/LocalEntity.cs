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

using System.IO;

namespace MoleculeViewer
{
    /// <summary>
    /// Represents entity data that can be displayed in a viewer.
    /// </summary>
    /// <remarks>
    /// Entity data is stored in a file in the local file system.
    /// </remarks>
    public class LocalEntity
    {
        private string fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalEntity"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file storing the entity's data.</param>
        public LocalEntity(string fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        /// Gets the name of the entity to display.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return Path.GetFileName(this.fileName); }
        }

        /// <summary>
        /// Gets the stream to read from the entity's data file.
        /// </summary>
        /// <returns>A read only stream.</returns>
        public Stream GetStream()
        {
            return File.OpenRead(this.fileName);
        }
    }
}
