﻿using System;

namespace RFBCodeWorks.Mvvm.XmlLinq.Controls
{
    /// <summary>
    /// Interface to provide requirements and descriptions for Controls that rely on the <see cref="RFBCodeWorks.Mvvm.XmlLinq.ValueSetters"/> objects
    /// </summary>
    internal interface IXValueControl
    {
        /// <summary>
        /// Occurs when the XML node this control is referencing is added/removed from the xml tree (or is referencing a different node than previously)
        /// <br/> Note that setting an XAttribute node to null will remove it from the tree.
        /// </summary>
        event EventHandler NodeChanged;
    }
}
