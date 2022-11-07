using System.ComponentModel;
using System.Windows;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Contains the basic bindings all controls have
    /// </summary>
    public class BaseControlDefinition : ObservableObject,  IControlDefinition, IToolTipProvider
    {
        /// <summary>
        /// Subscribe to a component's PropertyChanged event to pass on the args. <see langword="sender"/> will be this control object.
        /// </summary>
        /// <param name="sender">The sender - this value is thrown out and is only present to conform as an EventHandler method</param>
        /// <param name="e">These ares will be passed directly to this object's PropertyChanged event</param>
        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => OnPropertyChanged(e);

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


        /// <summary>
        /// Enable/Disable a control
        /// </summary>
        public virtual bool IsEnabled
        {
            get { return IsEnabledField; }
            set { SetProperty(ref IsEnabledField, value, nameof(IsEnabled)); }
        }
        private bool IsEnabledField = true;


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
                if (value != IsVisible)
                {
                    OnPropertyChanging(nameof(IsVisible));
                    if (value)
                    {
                        Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Visibility = HiddenMode;
                    }
                }
            }
        }

        /// <summary>
        /// Get/Set the visibility of the control. This field is 
        /// </summary>
        public Visibility Visibility
        {
            get { return VisibilityField; }
            set
            {
                var oldValue = VisibilityField;
                bool changed = SetProperty(ref VisibilityField, value, nameof(Visibility));
                if (changed)
                {
                    if (oldValue == Visibility.Visible)
                        OnOnVisibilityChanged(true, false);
                    else if (value == Visibility.Visible)
                        OnOnVisibilityChanged(false, true);
                }
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
