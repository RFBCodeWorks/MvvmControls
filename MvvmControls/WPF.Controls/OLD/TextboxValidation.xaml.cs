using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;

namespace RFBCodeWorks.WPF.Controls
{
    /// <summary>
    /// Interaction logic for NumericTextBox.xaml
    /// </summary>
    public partial class TextboxValidation : TextBox
    {
        /// <summary>
        /// 
        /// </summary>
        public TextboxValidation() : base() { }

        #region < Allowed Keys >

        /// <summary>
        /// Use this to allow certain characters, if this is set up, all other keys will be denied.
        /// </summary>
        public IEnumerable<Key> AllowedKeys
        {
            get { return (IEnumerable<Key>)GetValue(AllowedKeysProperty); }
            set { SetValue(AllowedKeysProperty, value); }
        }
        /// <summary></summary>
        public static readonly DependencyProperty AllowedKeysProperty =
            DependencyProperty.Register(nameof(AllowedKeys), typeof(IEnumerable<Key>), typeof(TextboxValidation), new PropertyMetadata(Array.Empty<Key>()));

        /// <summary>
        /// Use this to explicitly prohibit certain characters. All other characters will be accepted.
        /// </summary>
        public IEnumerable<Key> DisallowedKeys
        {
            get { return (Key[])GetValue(DisAllowedKeysProperty); }
            set { SetValue(DisAllowedKeysProperty, value); }
        }
        /// <summary></summary>
        public static readonly DependencyProperty DisAllowedKeysProperty =
            DependencyProperty.Register(nameof(DisallowedKeys), typeof(IEnumerable<Key>), typeof(TextboxValidation), new PropertyMetadata(Array.Empty<Key>()));

        #endregion

        #region < Data Validation >

        /// <summary>
        /// 
        /// </summary>
        public ValidationRuleDPO ValidationRule
        {
            get { return (ValidationRuleDPO)GetValue(ValidationRuleProperty); }
            set { SetValue(ValidationRuleProperty, value); }
        }
        /// <summary></summary>
        public static readonly DependencyProperty ValidationRuleProperty =
            DependencyProperty.Register(nameof(ValidationRule), typeof(ValidationRuleDPO), typeof(TextboxValidation));

        /// <summary>
        /// 
        /// </summary>
        public bool IsDataValid
        {
            get { return (bool)GetValue(IsDataValidProperty.DependencyProperty); }
        }
        /// <summary></summary>
        public static readonly DependencyPropertyKey IsDataValidProperty =
            DependencyProperty.RegisterReadOnly(nameof(IsDataValid), typeof(bool), typeof(TextboxValidation), new PropertyMetadata(true));

        #endregion

        #region < Background Brush Setup >

        /// <summary>
        /// Choose a default color to render the textbox. Default is <see cref="Brushes.White"/>
        /// </summary>
        /// <returns>
        /// Get: Get the current background color <br/>
        /// Set: Set the default background color to use if the input is valid.
        /// </returns>
        new public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set 
            {
                SetValue(DefaultBackgroundColorProperty, value);
                OnDataValidation();
            }
        }
        /// <summary></summary>
        public static readonly DependencyProperty DefaultBackgroundColorProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(TextboxValidation), new PropertyMetadata(Brushes.White));


        /// <summary>
        /// Choose a background brush to use when the value is changed from its original value. <br/>
        /// Default brush color is <see cref="Brushes.LightYellow"/>
        /// </summary>
        public Brush BackGroundInvalid
        {
            get { return (Brush)GetValue(BackGroundInvalidProperty); }
            set
            {
                SetValue(BackGroundInvalidProperty, value);
                OnDataValidation();
            }
        }
        /// <summary></summary>
        public static readonly DependencyProperty BackGroundInvalidProperty =
            DependencyProperty.Register(nameof(BackGroundInvalid), typeof(Brush), typeof(TextboxValidation), new PropertyMetadata(Brushes.Pink));

        #endregion

        #region < KeyDown Evaluation >

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Compares <paramref name="e"/>.Key and <paramref name="e"/>.System, then returns the key that was presed
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        static protected Key GetKeyFromKeyEventArgs(KeyEventArgs e) => e.Key == Key.System ? e.SystemKey : e.Key;

        protected static bool IsShiftKeyHeld() => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        protected static bool IsCtrlHeld() => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        protected static bool IsAltHeld() => Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // Fetch the real key.
            var key = GetKeyFromKeyEventArgs(e);
            bool AcceptKey = IsCtrlHeld() || key.IsFunctionKey() || key.IsSpecialSystemKey();
    
            // Check for Allowed Characters
            if (!AcceptKey && AllowedKeys.Any())
            {
                AcceptKey = AllowedKeys.Contains(key);
            }
            // Check for Disallowed Characters
            if (!AcceptKey && DisallowedKeys.Any())
            {
                AcceptKey = !DisallowedKeys.Contains(key);
            }
            //Accept/Allow check
            if (AcceptKey) 
                base.OnPreviewKeyDown(e);
            else 
                e.Handled = true;
        }

        #endregion

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            OnDataValidation();
            base.OnTextInput(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            OnDataValidation();
            base.OnLostFocus(e);
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        
        /// <inheritdoc cref="OnDataValidation(bool, bool)"/>
        protected virtual void OnDataValidation() => OnDataValidation(ValidationRule?.Validate(Text, CultureInfo.CurrentCulture).IsValid ?? true, true);

        /// <summary>
        /// Evaluate the <see cref="ValidationRule"/>, set <see cref="IsDataValid"/>, then set the background color accordingly
        /// </summary>
        /// <param name="UpdateBackground">
        /// If TRUE: Sets background color based on if data is valid or not. <br/>
        /// If FALSE: Do not set the background color. (only update the IsDataValid property) 
        /// </param>
        /// <param name="isDataValid"/>
        protected void OnDataValidation(bool isDataValid, bool UpdateBackground = true)
        {
            SetValue(IsDataValidProperty, isDataValid);
            if (UpdateBackground)
            {
                if (isDataValid)
                {
                    base.Background = this.Background; // Valid Data
                }
                else
                {
                    base.Background = this.BackGroundInvalid; //Invalid Data
                }
            }
        }

        /// <summary>
        /// Set Visibility to <see cref="Visibility.Hidden"/>
        /// </summary>
        public void Hide()
        {
            this.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Set Visibility to <see cref="Visibility.Visible"/>
        /// </summary>
        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }
    }
}
