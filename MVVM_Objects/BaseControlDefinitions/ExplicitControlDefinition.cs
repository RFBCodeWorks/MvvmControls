using RFBCodeWorks.MVVMObjects.ControlInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.MVVMObjects.BaseControlDefinitions
{
    /// <summary>
    /// Base class that explicitly implements the following:
    /// <br/> <br/> - Protected setters exist for this explicit interface - <see cref="IControlDefinition"/>
    /// <br/> <br/> - ToolTip value is null by default! - <see cref="IToolTipProvider"/> 
    /// <br/> <br/> - DisplayText Value is null by default! - <see cref="IDisplayTextProvider"/>
    /// </summary>
    public class ExplicitControlDefinition : ObservableObject, ControlInterfaces.IControlDefinition, ControlInterfaces.IDisplayTextProvider
    {


        /// <summary>
        /// Property used to set the value for <see cref="IControlDefinition.Visibility"/>
        /// </summary>
        protected Visibility Visibility
        {
            get { return VisibilityField; }
            set { 
                var wasSet = SetProperty(ref VisibilityField, value, nameof(Visibility));
                if (wasSet) OnPropertyChanged(nameof(IsVisible));
            }
        }
        private Visibility VisibilityField;

        /// <summary>
        /// Property used to set the value for <see cref="IControlDefinition.IsVisible"/>
        /// </summary>
        /// <remarks>Default implementation toggles between Visible and Collapsed</remarks>
        protected virtual bool IsVisible
        {
            get { return Visibility == Visibility.Visible; }
            set { Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        Visibility IControlDefinition.Visibility { get => this.Visibility; set => this.Visibility = value; }
        bool IControlDefinition.IsVisible { get => IsVisible; set => IsVisible = value; }

        string IToolTipProvider.ToolTip => null;

        string IDisplayTextProvider.DisplayText => null;
    }
}
