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

    /*
     * XObject
     *  -> XAttrribute
     *      -- Only provides 'Value' changes, since can contain no children nodes or information
     *      -- This can only provide a value change when the node exists. 
     *          -- If node doesn't exist, setting the value will produce an 'Add' event on the parent XElement
     *          -- If node exists, setting the value produces a 'Value' event from this XAttribute
     *          -- Setting value to null produces a 'Remove' event on parent XElement
     *  -> XNode
     *      -> XText -- Represents some string value within the tree, may be string.empty - Requires parent XElement
     *      -> XComment -- Xml Comment within the tree - requires parent XElement
     *      -> XContainer
     *          -> XDocument
     *              - Contains the root element, can only contain 1 root element
     *              - Can contain XComments, XDocumentType, XProcessingInstructions
     *              - XElements with a parent value = null mean the parent is the XDocument
     *          -> XElement
     *              - Contains all other elements within this element
     *              - The Changed event bubbles up through the tree's XElement objects until it reaches the XDocument
     *              - The 'sender' of the Changed event will be the node that is changing/adding/removing within the tree
     */


    /// <summary>
    /// Base interface for the other IX... interfaces
    /// </summary>
    public interface IXObjectProvider
    {
        /// <summary>
        /// Occurs when this node is added to the tree.
        /// </summary>
        /// <remarks>
        /// This would be raised when the XML.Linq query changes from returning null to returning an object, and signals to children IXObjectProviders they should check for their existence within the tree.
        /// </remarks>
        event EventHandler Added;

        /// <summary>
        /// Occurs when this node is removed from the tree
        /// </summary>
        /// <remarks>
        /// This would be raised when the XML.Linq query changes from returning an object to returning null, causing all IXObjectProviders children to also return null.
        /// </remarks>
        event EventHandler Removed;

        /// <summary>
        /// Get the parent IXElementProvider object. 
        /// <br/> This will be null if the parent of the provider is an XDocument object, or if the object has no parent (does not reside within the tree)
        /// </summary>
        IXElementProvider Parent { get; }

        /// <summary>
        /// Gets the XObject from the provider (Typically either an XAttribute or an XElement)
        /// </summary>
        XObject? XObject { get; }

        /// <summary>
        /// Determine if the XObject is available to interact with
        /// </summary>
        bool IsNodeAvailable { get; }

        /// <summary>
        /// Setting to enable/disable automatic creation of missing XElements within the tree under various scenarios, 
        /// such as if an XAttributeProvider's value was set but the parent XElement does not exist
        /// </summary>
        /// <returns>TRUE if the XElement node can be created, otherwise false.</returns>
        bool CanBeCreated { get; set; }

        /// <inheritdoc cref="XNode.Remove"/>
        void Remove();
    }

    /// <summary>
    /// Interface for getting a raw XML Value from an XNode
    /// </summary>
    public interface IXValueObject : IXObjectProvider, INotifyPropertyChanged
    {
        /// <summary>
        /// The name of the XObject
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Occurs when the Value is changed
        /// </summary>
        event EventHandler ValueChanged;

        /// <summary>
        /// Property used to get/set the string value of the node. <br/>
        /// If the node does not exist, setting to a non-null value will attempt to create the node within the tree.
        /// </summary>
        string Value { get; set; }
    }

    /// <summary>
    /// Provides an <see cref="XElement"/> object
    /// </summary>
    public interface IXElementProvider : IXObjectProvider
    {
        /// <summary>
        /// The name of the XElement
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Occurs when this node received a 'Changed' event that indicates an ADD or REMOVE event from some descendant within the tree. <br/>
        /// The sender of the event should represent the node that was changed, not necessarily this object that hosts the event.
        /// </summary>
        /// <remarks>
        /// While the 'XObject.Changed' event bubbles up the tree (and is how this is raised), this is meant to tunnel down to notify downstream Xml.Linq objects.
        /// <br/> - If its determined that this XObject was Added/Removed To/From the tree, then those events should be raised, meaning this should only be notifying direct Xml.Linq children.
        /// </remarks>
        event EventHandler DescendantChanged;

        /// <summary>
        /// Get the XElement from the provider
        /// </summary>
        /// <returns>An XElement object, or null if the element does not exist</returns>
        new XElement? XObject { get; }

        /// <summary>
        /// Create the XElement within the tree if it is missing
        /// </summary>
        /// <returns>The XElement object, or null if the XElement was not created/does not exist.</returns>
        XElement CreateXElement();

    }

    /// <summary>
    /// Provides an <see cref="XAttribute"/> object
    /// </summary>
    public interface IXAttributeProvider : IXObjectProvider, IXValueObject
    {
        /// <summary>
        /// Get the XAttribute from the provider
        /// </summary>
        /// <returns>An XAttribute object, or null if the node does not exist</returns>
        new XAttribute? XObject { get; }

    }
#nullable disable
}
