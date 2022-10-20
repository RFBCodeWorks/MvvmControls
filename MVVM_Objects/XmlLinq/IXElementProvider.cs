using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RFBCodeWorks.MVVMObjects.XmlLinq
{
    #nullable enable

    /// <summary>
    /// Interface for getting a raw XML Value from an XNode
    /// </summary>
    public interface IXValueProvider: INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when the reference to underlying XNode object is updated
        /// </summary>
        event EventHandler XNodeChanged;
        
        /// <summary>
        /// Property used to get/set the value of the node
        /// </summary>
        string XmlValue { get; set; }

    }

    /// <summary>
    /// Provides an <see cref="XElement"/> object
    /// </summary>
    public interface IXElementProvider
    {
        /// <summary>
        /// Occurs when the XElement changes to a new object, so consumers can call <see cref="GetXElement"/> to refresh themselves.
        /// </summary>
        event EventHandler XElementChanged;

        /// <summary>
        /// Get the XElement from the provider
        /// </summary>
        /// <returns>An XElement object, or null if the element does not exist</returns>
        XElement? GetXElement();
    }

    /// <summary>
    /// Provides an <see cref="XAttribute"/> object
    /// </summary>
    public interface IXAttributeProvider : IXValueProvider
    {
        /// <summary>
        /// Occurs when the XAttribute changes to a new object, so consumers can call <see cref="GetXAttribute"/> to refresh themselves.
        /// </summary>
        event EventHandler XAttributeChanged;
        
        /// <summary>
        /// Get the XAttribute from the provider
        /// </summary>
        /// <returns>An XAttribute object, or null if the node does not exist</returns>
        XAttribute? GetXAttribute();

    }
    #nullable disable
}
