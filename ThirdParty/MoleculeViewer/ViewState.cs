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
using System.Xml;

namespace MoleculeViewer
{
    /// <summary>
    /// Abstract base class to represent the state of a rendered view.
    /// </summary>
    internal abstract class ViewState
    {
        /// <summary>
        /// Creates the smooth tween of the two specified ViewStates.
        /// </summary>
        /// <param name="startViewState">Start <see cref="ViewState"/> of the tween.</param>
        /// <param name="endViewState">End <see cref="ViewState"/> of the tween.</param>
        /// <param name="t">The interpolation coefficient.</param>
        /// <returns>A <see cref="ViewState"/> representing the tween of the specifed ViewStates.</returns>
        internal static ViewState CreateSmoothTween(
            ViewState startViewState, ViewState endViewState, double t)
        {
            t = 3 * Math.Pow(t, 2) - 2 * Math.Pow(t, 3);
            
            return startViewState.CreateTween(endViewState, t);
        }

        /// <summary>
        /// Creates the XML node representing the ViewState.
        /// </summary>
        /// <returns>An XML node representing this instance.</returns>
        internal abstract XmlNode CreateXmlNode();

        /// <summary>
        /// Creates the tween of the instance and the specified end ViewState.
        /// </summary>
        /// <param name="endViewState">End <see cref="ViewState"/> of the tween.</param>
        /// <param name="t">The interpolation coefficient.</param>
        /// <returns>
        /// A <see cref="ViewState"/> representing the tween of this instance and the specifed end ViewState.
        /// </returns>
        protected abstract ViewState CreateTween(ViewState endViewState, double t);
    }
}
