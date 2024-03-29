﻿using System.Xml.Linq;

#nullable enable

namespace RFBCodeWorks.Mvvm.XmlLinq
{
    /// <summary>
    /// Extensions for working with the XmlLinq objects easier
    /// </summary>
    public static class XObjectExtensions
    {

        /// <summary>
        /// Gets the <see cref="XName.LocalName"/> from either an <see langword="XAttribue"/> or <see langword="XElement"/> object
        /// </summary>
        /// <param name="xObj"></param>
        /// <returns>Returns null if unable to get the XName.LocalName</returns>
        public static string? GetName(this XObject xObj)
        {
            if (xObj is XAttribute attr)
                return attr.Name.LocalName;
            if (xObj is XElement el)
                return el.Name.LocalName;
            return null;
        }

        /// <inheritdoc cref="GetName(XObject)"/>
        public static string? GetName(this IXObjectProvider xObj)
        {
            if (xObj is XObject obj && !(xObj is XDocument))
                return GetName(obj);
            return xObj.XObject?.GetName();
        }

        /// <summary>
        /// Wraps an XElement into an <see cref="XElementWrapper"/>
        /// </summary>
        /// <param name="element">the element to wrap</param>
        /// <returns>a new <see cref="XElementWrapper"/></returns>
        /// <inheritdoc cref="XElementWrapper.XElementWrapper(XElement)"/>
        public static XElementWrapper Wrap(this XElement element) => new XElementWrapper(element);

        ///// <summary>
        ///// Wraps an XElement into an <see cref="XAttributeWrapper"/>
        ///// </summary>
        ///// <param name="element">the element to wrap</param>
        ///// <returns>a new <see cref="XAttributeWrapper"/></returns>
        ///// <inheritdoc cref="XAttributeWrapper.XAttributeWrapper(XAttribute)"/>
        //public static XAttributeWrapper Wrap(this XAttribute element) => new XAttributeWrapper(element);

        /// <inheritdoc cref="XElementProvider.XElementProvider(string, IXElementProvider)"/>
        public static XElementProvider CreateChildProvider(this IXElementProvider parent, string elementName)
        {
            return new XElementProvider(elementName, parent);
        }

        /// <inheritdoc cref="XElementProvider.GetXElementProvider(IXElementProvider, string, string, string)"/>
        public static XElementProvider CreateChildProvider(this IXElementProvider parent, string elementName, string attributeName, string attributeValue)
        {
            return XElementProvider.GetXElementProvider(parent, elementName, attributeName, attributeValue);
        }

        /// <inheritdoc cref="XAttributeRetriever.XAttributeRetriever(string, IXElementProvider)"/>
        public static XAttributeRetriever CreateAttributeProvider(this IXElementProvider parent, string attributeName)
        {
            return new XAttributeRetriever(attributeName, parent);
        }
    }

}
#nullable disable
