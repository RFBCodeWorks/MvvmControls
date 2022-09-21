using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.MVVMObjects.BaseControlDefinitions
{
    /// <summary>
    /// Contains the basic bindings all controls have
    /// </summary>
    public class BaseControlDefinition : ObservableObject, IToolTipProvider
    {

        #region < OnVisibilityChanged >

        /// <summary>
        /// Occurs when <see cref="IsVisible"/> has been updated
        /// </summary>
        public event PropertyOfTypeChangedEventHandler<bool> VisibilityChanged;

        /// <summary> Raises the OnVisibilityChanged event </summary>
        protected virtual void OnOnVisibilityChanged(bool oldValue, bool newValue)
        {
            var e = new PropertyOfTypeChangedEventArgs<bool>(oldValue, newValue, nameof(IsVisible));
            VisibilityChanged?.Invoke(this, e);
            OnPropertyChanged(e);
        }

        #endregion


        /// <inheritdoc/>
        /// <remarks> May or may not be set! </remarks>
        public virtual string ToolTip
        {
            get => toolTip;
            set => base.SetProperty(ref toolTip, value, nameof(ToolTip));
        }
        private string toolTip;


        /// <summary>
        /// Get/Set the visibility of the control. This is the primary toggle for visibility that ViewModel should interact with.
        /// <br/> - Setting this to TRUE sets <see cref="Visibility"/> to VISIBLE.
        /// <br/> - Setting this to FALSE sets <see cref="Visibility"/> to <see cref="HiddenMode"/>.
        /// </summary>
        public bool IsVisible
        {
            get { return Visibility == Visibility.Visible; }
            set
            {
                if (IsVisibleField != value)
                {
                    bool oldValue = IsVisibleField;
                    IsVisibleField = value;
                    if (!ChangingVisibility) OnOnVisibilityChanged(oldValue, value); // Raise event if not also updating Visibility property
                }
                if (ChangingVisibility) return;
                ChangingVisibility = true;
                Visibility = value ? Visibility.Visible : HiddenMode;
                ChangingVisibility = false;
                OnOnVisibilityChanged(!IsVisible, IsVisible);
            }
        }
        private bool IsVisibleField = true;
        
        /// <summary>
        /// Flag turned on within the Set methods for IsVisible and Visibility to avoid loops.
        /// </summary>
        protected bool ChangingVisibility;

        /// <summary>
        /// Get/Set the visibility of the control. This field is 
        /// </summary>
        public Visibility Visibility
        {
            get { return VisibilityField; }
            set
            {
                SetProperty(ref VisibilityField, value, nameof(Visibility));
                if (ChangingVisibility) return;
                ChangingVisibility = true;
                IsVisible = false;
                ChangingVisibility = false;
            }
        }
        private Visibility VisibilityField = Visibility.Visible;


        /// <summary>
        /// Sets the default <see cref="Visibility"/> state for when the <see cref="IsVisible"/> is set to false.
        /// <br/> This property is not meant to be bound to, more of a supporting property for <see cref="IsVisible"/>
        /// </summary>
        /// <remarks>
        /// Default mode is <see cref="Visibility.Collapsed"/>
        /// </remarks>
        public Visibility HiddenMode
        {
            get { return HiddenModeField; }
            set { 
                SetProperty(ref HiddenModeField, value, nameof(HiddenMode));
                if (!IsVisible) Visibility = value;
            }
        }
        private Visibility HiddenModeField = Visibility.Collapsed;

    }
}
