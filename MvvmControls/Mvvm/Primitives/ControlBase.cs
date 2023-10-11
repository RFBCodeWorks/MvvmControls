using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace RFBCodeWorks.Mvvm.Primitives
{
    /// <summary>
    /// Contains the basic bindings all controls have
    /// </summary>
    public class ControlBase : ObservableObject, IControlDefinition, IToolTipProvider
    {
        /// <summary> Static method that can be used as the default Func{T, bool}</summary>
        /// <returns><see langword="true"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool ReturnTrue<T>(T ignoredParameter) => true;

        /// <summary> Static method that can be used as the default Func{bool}</summary>
        /// <returns><see langword="true"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool ReturnTrue() => true;

        #region < Event Arg Singletons >

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly INotifyArgs TooltipChangingArgs = EventArgSingletons.ToolTip;
        public static readonly INotifyArgs IsEnabledChangingArgs = EventArgSingletons.IsEnabled;
        private static readonly INotifyArgs HiddenModeChangingArgs = EventArgSingletons.HiddenMode;
        private static readonly INotifyArgs VisibilityChangingArgs = EventArgSingletons.Visibility;
        private static readonly INotifyArgs IsVisibleChangingArgs = EventArgSingletons.IsVisible;

        private static readonly PropertyOfTypeChangedEventArgs<bool> IsVisibleNowTRUE = new PropertyOfTypeChangedEventArgs<bool>(false, true, nameof(IsVisible));
        private static readonly PropertyOfTypeChangedEventArgs<bool> IsVisibleNowFALSE = new PropertyOfTypeChangedEventArgs<bool>(true, false, nameof(IsVisible));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion

        #region < OnVisibilityChanged >

        /// <summary>
        /// Occurs when <see cref="IsVisible"/> has been updated
        /// </summary>
        public event PropertyOfTypeChangedEventHandler<bool> VisibilityChanged;

        /// <summary> Raises the OnVisibilityChanged event, then raises the PropertyChanged event with the same args.</summary>
        /// <param name="isVisible">set TRUE if the visibility is changing to TRUE, set to false if the control is becoming hidden or collapsed.</param>
        protected virtual void OnOnVisibilityChanged(bool isVisible)
        {
            var e = isVisible ? IsVisibleNowTRUE : IsVisibleNowFALSE;
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
            set {
                if (IsEnabledField != value)
                {
                    OnPropertyChanging(IsEnabledChangingArgs);
                    IsEnabledField = value;
                    OnPropertyChanged(IsEnabledChangingArgs);
                }
            }
        }
        private bool IsEnabledField = true;


        /// <inheritdoc/>
        /// <remarks> May or may not be set! </remarks>
        public virtual string ToolTip
        {
            get => toolTip;
            set { 
                if (toolTip != value)
                {
                    OnPropertyChanging(TooltipChangingArgs);
                    toolTip = value;
                    OnPropertyChanged(TooltipChangingArgs);
                }
            }
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
                    OnPropertyChanging(IsVisibleChangingArgs);
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
                if (Visibility != value)
                {
                    OnPropertyChanging(VisibilityChangingArgs);
                    var oldValue = VisibilityField;
                    VisibilityField = value;
                    OnPropertyChanged(VisibilityChangingArgs);

                    if (oldValue == Visibility.Visible)
                        OnOnVisibilityChanged(false);
                    else if (value == Visibility.Visible)
                        OnOnVisibilityChanged(true);
                    
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
            set
            {
                if (HiddenMode != value)
                {
                    OnPropertyChanging(HiddenModeChangingArgs);
                    HiddenModeField = value;
                    if (!IsVisible) Visibility = value;
                    OnPropertyChanged(HiddenModeChangingArgs);
                }
            }
        }
        private Visibility HiddenModeField = Visibility.Collapsed;

        /// <summary>
        /// Subscribe to a component's PropertyChanged event to pass on the args. <see langword="sender"/> will be this control object.
        /// </summary>
        /// <param name="sender">The sender - this value is thrown out and is only present to conform as an EventHandler method</param>
        /// <param name="e">These ares will be passed directly to this object's PropertyChanged event</param>
        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => OnPropertyChanged(e);
    }
}
