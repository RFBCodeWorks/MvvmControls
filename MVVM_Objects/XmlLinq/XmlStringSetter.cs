using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFBCodeWorks.MVVMObjects.ControlInterfaces;

namespace RFBCodeWorks.MVVMObjects.XmlLinq
{
    /// <summary>
    /// Provides a way to interact with an <see cref="IXValueObject"/> to set it to a string value
    /// </summary>
    /// <remarks>
    /// <br/> - Explicitly implements <see cref="ControlInterfaces.IControlDefinition"/> 
    /// <br/> - Explicitly implements <see cref="ControlInterfaces.IDisplayTextProvider"/> 
    /// <br/> - Explicitly implements <see cref="ControlInterfaces.IToolTipProvider"/> 
    /// </remarks>
    public class XmlStringSetter : BaseControlDefinitions.ExplicitControlDefinition, IDisplayTextProvider, ControlInterfaces.IToolTipProvider
    {

        /// <summary>
        /// Create the XML Integer Setter
        /// </summary>
        /// <param name="xValueProvider">The object that will provide the node whose value needs to be set</param>
        public XmlStringSetter(IXValueObject xValueProvider)
        {
            XValueProvider = xValueProvider ?? throw new ArgumentNullException(nameof(xValueProvider));
            XValueProvider.Added += XValueProvider_XNodeChanged;
            XValueProvider.Removed += XValueProvider_XNodeChanged;
            XValueProvider.ValueChanged += XValueProvider_ValueChanged;
        }

        /// <inheritdoc cref="XmlBooleanSetter.XValueProvider"/>
        public IXValueObject XValueProvider { get; }
        
        /// <summary>
        /// Gets and Sets the value of the <see cref="IXValueObject"/>
        /// </summary>
        public string Value
        {
            get { return IsEnabled ? XValueProvider.Value : null; }
            set
            {
                if (IsEnabled && Value != value)
                {
                    XValueProvider.Value = value;
                }
            }
        }

        /// <inheritdoc cref="XmlBooleanSetter.IsEnabled"/>
        public bool IsEnabled => XValueProvider.XObject != null;

        string IToolTipProvider.ToolTip => null;

        string IDisplayTextProvider.DisplayText => Value;

        /// <summary>
        /// Occurs when the <see cref="XValueProvider"/> node is changed
        /// </summary>
        protected virtual void XValueProvider_XNodeChanged(object sender, EventArgs e)
        {
            //OnPropertyChanged(nameof(Value));
            //OnPropertyChanged(nameof(IDisplayTextProvider.DisplayText));
            OnPropertyChanged(nameof(IsEnabled));
        }

        private void XValueProvider_ValueChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(IDisplayTextProvider.DisplayText));
        }
    }
}
