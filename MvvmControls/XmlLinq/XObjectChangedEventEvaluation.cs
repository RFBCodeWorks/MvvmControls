using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RFBCodeWorks.MvvmControls.XmlLinq
{
    /// <summary>
    /// Contains helper methods to process the <see cref="XObject.Changed"/> method
    /// </summary>
    public static class XObjectChangedEventEvaluation
    {
        /// <summary>
        /// Helper enum to be returned by the methods that evaluate XObject.Changed events
        /// </summary>
        public enum ChangeType
        {
            /// <summary>
            /// Occurs when no significant change is reported to the node in question
            /// </summary>
            None,
            /// <summary>
            /// Occurs when a descendant is added to an XElement
            /// </summary>
            DescendantAdded,
            /// <summary>
            /// Occurs when a descendant is removed from an XElement
            /// </summary>
            DescendantRemoved,
            /// <summary>
            /// Occurs when the value of an XElement or XAttribute is updated
            /// </summary>
            ValueChanged,
            /// <summary>
            /// Occurs when the name of an XElement is updated
            /// </summary>
            NameChanged
        }

        /// <summary>
        /// React to the XObject being changed.
        /// </summary>
        /// <param name="sender">
        /// The sender is the node that initiated the change. This could be an XText object for a string value, or an XElement object for a node being added, as well as any other XObject
        /// </param>
        /// <param name="e">
        /// Add/Remove: 
        /// <br/> - Add/Remove changes occur for children being added/removed, but also for 'Value' changes.
        /// <br/> - Value changes will have a type of <see cref="XText"/>
        /// Value:
        /// <br/> - Value events occur when the string value is set to string.empty, after the value has been removed.
        /// <br/> - If a string value is not set, it will only call the 'ADD' event when setting a string value.
        /// <br/> - If a string value is being replaced, it will first create a 'Remove' event to remove the previous value, then raise an 'Add' event to add the new value.
        /// <br/> - If a string value is being removed, it will first create a 'Remove' event, then it will create a 'Value' event.
        /// </param>
        /// <param name="node">The node whose event is subscribed to. This is used to determine if this node is changing, or if a a descendant is changing.</param>
        /// <param name="discriminateDescendants">
        /// If FALSE (default), this will return Added/Removed when any descendant is added/removed. <br/>
        /// If TRUE, this will return Added/Removed only when a direct child node is added/removed.
        /// </param>
        /// <returns>
        /// - <see cref="ChangeType.None"/> -- when this node has no direct change applied to it.
        /// <br/> - <see cref="ChangeType.NameChanged"/> -- when this node's name has changed
        /// <br/> - <see cref="ChangeType.ValueChanged"/> -- when this node's string value has changed
        /// <br/> - <see cref="ChangeType.DescendantAdded"/> -- when a descendant of the node has been added to the tree
        /// <br/> - <see cref="ChangeType.DescendantRemoved"/> -- when a descendant of the node has been removed from the tree
        /// </returns>
        public static ChangeType XElementChanged(object sender, XObjectChangeEventArgs e, XElement node, bool discriminateDescendants = false)
        {
            if (node is null) throw new ArgumentNullException(nameof(node));
            var XSender = sender as XObject;
            if (XSender is null) throw new ArgumentException("sender is not a type of XObject");
            if (e is null) throw new ArgumentNullException(nameof(e));

            bool isSelf = XSender == node;
            bool isParent = !isSelf && (!discriminateDescendants || XSender.Parent == node);
            
            switch (e.ObjectChange)
            {
                case XObjectChange.Name:
                    if (isSelf)
                        return ChangeType.NameChanged;
                    else
                        return ChangeType.None;

                case XObjectChange.Value:
                    if (isSelf)
                        return ChangeType.ValueChanged;
                    else
                        return ChangeType.None;

                case XObjectChange.Add:
                    if (sender is XText && isParent)
                        return ChangeType.ValueChanged;
                    else if (isParent)
                        return ChangeType.DescendantAdded;
                    else
                        return ChangeType.None;

                case XObjectChange.Remove:
                    if (sender is XText)
                        // do nothing, since an XText Remove event is always followed by either an 'Add' event or a 'Value' event
                        return ChangeType.None;
                    else 
                        // Cannot discriminate against the 'Removed' event, since the sender will never report the current node as its parent.
                        return ChangeType.DescendantRemoved;

                default:
                    throw new NotImplementedException($"Unknown XObjectChangeEventArg value -- {e.ObjectChange}");
            }
        }

        /// <summary>
        /// React to the XAttribute being changed.
        /// </summary>
        /// <param name="sender">
        /// The sender will always be the XAttribute node.
        /// </param>
        /// <param name="e">
        /// - Name: Name changes are not allowed, and thus will never occur. 
        /// <br/> - Add/Remove: These cannot have children nodes, and thus will never occur.
        /// <br/> - Value: Occurs when the value of this node changes.
        /// </param>
        /// <returns>Should always return <see cref="ChangeType.ValueChanged"/> since that is all that XAttributes support</returns>
        public static ChangeType XAttributeChanged(object sender, XObjectChangeEventArgs e)
        {
            if (!(sender is XAttribute)) throw new Exception("Sender is not an XAttribute node");
            switch (e.ObjectChange)
            {
                // these will never occur
                case XObjectChange.Name:
                case XObjectChange.Add:
                case XObjectChange.Remove:
                    throw new NotImplementedException("This type of change should never occur");

                case XObjectChange.Value:
                    return ChangeType.ValueChanged;
                
                default:
                    throw new NotImplementedException($"Unknown XObjectChangeEventArg value -- {e.ObjectChange}");
            }
        }
    }
}
