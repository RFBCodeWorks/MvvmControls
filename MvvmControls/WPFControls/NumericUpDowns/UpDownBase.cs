using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RFBCodeWorks.WPFControls.Primitives
{
    /// <summary>
    /// Abstract base class to allow a ControlTemplate to be made. <br/>
    /// Derived classes should Derive from <see cref="UpDownBase{T}"/>
    /// </summary>
    public abstract class UpDownBase : Control
    {
        /// <summary>
        /// Initialize the control
        /// </summary>
        protected UpDownBase() { }

        /// <summary>
        /// Command to assign the button that increases the value
        /// </summary>
        public RelayCommand IncreaseValueCommand { get; init; }

        /// <summary>
        /// Command to assign the button that decreases the value
        /// </summary>
        public RelayCommand DecreaseValueCommand { get; init; }

        /// <summary>
        /// The time that the button was initially pressed 
        /// </summary>
        DateTime InitialPress;
        /// <summary>
        /// The time the last button press was accepted
        /// </summary>
        DateTime LastTick;

        #region < IsReadOnly >
        /// <summary>Determine if the value of the control can be changed by the user interacting with the buttons</summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <inheritdoc cref="IsReadOnly"/>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(UpDownBase), new PropertyMetadata(false));
        #endregion

        #region < Button Orientation >

        /// <inheritdoc cref="UpDownOrientation"/>
        public UpDownOrientation ButtonOrientation
        {
            get { return (UpDownOrientation)GetValue(ButtonOrientationProperty); }
            set { SetValue(ButtonOrientationProperty, value); }
        }

        /// <inheritdoc cref="UpDownOrientation"/>
        public static readonly DependencyProperty ButtonOrientationProperty =
            DependencyProperty.Register("ButtonOrientation", typeof(UpDownOrientation), typeof(UpDownBase), new PropertyMetadata(UpDownOrientation.OnRight));

        #endregion

        #region < DisplayFormat >

        /// <summary>
        /// The format to display the string value in the textbox
        /// </summary>
        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        /// <inheritdoc cref="StringFormat"/>
        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register("StringFormat", typeof(string), typeof(UpDownBase), new PropertyMetadata(default));

        #endregion

        #region < Button Delay>
        /// <inheritdoc cref="System.Windows.Controls.Primitives.RepeatButton.Delay"/>
        public int ButtonDelay
        {
            get { return (int)GetValue(ButtonDelayProperty); }
            set { SetValue(ButtonDelayProperty, value); }
        }

        /// <inheritdoc cref="System.Windows.Controls.Primitives.RepeatButton.Delay"/>
        public static readonly DependencyProperty ButtonDelayProperty =
            DependencyProperty.Register("ButtonDelay", typeof(int), typeof(UpDownBase), new PropertyMetadata(System.Windows.Controls.Primitives.RepeatButton.DelayProperty.DefaultMetadata.DefaultValue));

        #endregion

        #region < Button Interval >
        /// <inheritdoc cref="System.Windows.Controls.Primitives.RepeatButton.Delay"/>
        public int ButtonInterval
        {
            get { return (int)GetValue(ButtonIntervalProperty); }
            set { SetValue(ButtonIntervalProperty, value); }
        }

        /// <inheritdoc cref="System.Windows.Controls.Primitives.RepeatButton.Delay"/>
        public static readonly DependencyProperty ButtonIntervalProperty =
            DependencyProperty.Register("ButtonInterval", typeof(int), typeof(UpDownBase), new PropertyMetadata(System.Windows.Controls.Primitives.RepeatButton.IntervalProperty.DefaultMetadata.DefaultValue));

        #endregion

        #region < Repeat Button Large Increments >

        /// <summary>
        /// The millisecond delay time while a button is being held before large changes start occurring
        /// </summary>
        public int DelayBeforeLargeChange
        {
            get { return (int)GetValue(DelayBeforeLargeChangeProperty); }
            set { SetValue(DelayBeforeLargeChangeProperty, value); }
        }

        /// <inheritdoc cref="DelayBeforeLargeChange"/>
        public static readonly DependencyProperty DelayBeforeLargeChangeProperty =
            DependencyProperty.Register("DelayBeforeLargeChange", typeof(int), typeof(UpDownBase), new PropertyMetadata(1500));

        #endregion

        private bool ShouldDoLargeChange()
        {
            bool isHeld = LastTick == default || LastTick.AddMilliseconds(5 * ButtonInterval) >= DateTime.Now;
            LastTick = DateTime.Now;
            if (!isHeld)
            {
                InitialPress = LastTick;
                return false;
            }
            return LastTick.Subtract(InitialPress).TotalMilliseconds >= DelayBeforeLargeChange;
        }

        /// <summary>
        /// Action to perform when the INCREASE command is executed
        /// </summary>
        protected void IncreaseValueAction()
        {
            if (ShouldDoLargeChange())
                LargeIncrement();
            else
                SmallIncrement();
        }

        
        /// <summary>
        /// Action to perform when the DECREASE command is executed
        /// </summary>
        protected void DecreaseValueAction()
        {
            if (ShouldDoLargeChange())
                LargeDecrement();
            else
                SmallDecrement();
        }

        /// <summary>
        /// Increase the value when the button is pressed
        /// </summary>
        protected abstract void SmallIncrement();
        
        /// <summary>
        /// Increase the value when the button is pressed
        /// </summary>
        protected abstract void LargeIncrement();
        
        /// <summary>
        /// Decrease the value when the button is pressed
        /// </summary>
        protected abstract void SmallDecrement();
        
        /// <summary>
        /// Decrease the value when the button is pressed
        /// </summary>
        protected abstract void LargeDecrement();

        /// <summary>React to the Up/Down arrows being pressed to increase/decrease the value</summary>
        /// <inheritdoc/>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Handled || IsReadOnly)
            {
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Up || e.Key == Key.OemPlus)
            {
                IncreaseValueAction();
                e.Handled = true;
            }
            else if (e.Key == Key.Down || e.Key == Key.OemMinus)
            {
                DecreaseValueAction();
                e.Handled = true;
            }
        }

        /// <inheritdoc/>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            bool isNum = double.TryParse(e.Text, out _);
            e.Handled = !isNum;
        }
    }

    
}
