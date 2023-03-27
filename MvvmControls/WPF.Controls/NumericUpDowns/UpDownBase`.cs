using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RFBCodeWorks.WPF.Controls.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UpDownBase<T> : UpDownBase, IUpDown
        where T : struct, IComparable<T>, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Initialize the control
        /// </summary>
        protected UpDownBase() : base()
        {
            IncreaseValueCommand = new RelayCommand(IncreaseValueAction, CanIncreaseValue);
            DecreaseValueCommand = new RelayCommand(DecreaseValueAction, CanDecreaseValue);
            DataObject.AddPastingHandler(this, OnPaste);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsGreaterThan(T reference, T value) => value.CompareTo(reference) > 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsLessThan(T reference, T value) => value.CompareTo(reference) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsGreaterThanMax(T value) => value.CompareTo(Maximum) > 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsGreaterOrEqualToMax(T value) => value.CompareTo(Maximum) >= 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsLessThanMin(T value) => value.CompareTo(Minimum) < 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsLessOrEqualToMin(T value) => value.CompareTo(Minimum) <= 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool NotEqual(T oldValue, T newValue) => !oldValue.Equals(newValue);

        /// <summary>
        /// Overrides the MetaData for the derived class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="smallChange"></param>
        /// <param name="largeChange"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        protected static void OverrideMetaData(Type type, T smallChange, T largeChange, T minValue, T maxValue)
        {
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            MinimumProperty.OverrideMetadata(type, new PropertyMetadata(minValue, RangeUpdated, CoerceMinimum));
            MaximumProperty.OverrideMetadata(type, new PropertyMetadata(maxValue, RangeUpdated, CoerceMaximum));
            ValueProperty.OverrideMetadata(type, new PropertyMetadata(default(T), EvaluateIsDirty, CoerceValue));
            DefaultValueProperty.OverrideMetadata(type, new PropertyMetadata(default(T), EvaluateIsDirty));
            SmallChangeProperty.OverrideMetadata(type, new PropertyMetadata(smallChange, DoNothingCallBack, CoerceIncrements));
            LargeChangeProperty.OverrideMetadata(type, new PropertyMetadata(largeChange, DoNothingCallBack, CoerceIncrements));

            HorizontalContentAlignmentProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(HorizontalAlignment.Center));
            VerticalContentAlignmentProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(VerticalAlignment.Center));
        }

        /// <summary>
        /// Attempt to parse a pasted value
        /// </summary>
        /// <param name="pastedValue">the string to parse</param>
        /// <param name="value">the output value</param>
        /// <returns>TRUE if parsed successfully, otherwise false</returns>
        protected abstract bool TryParse(string pastedValue, out T value);

        /// <summary>
        /// Evaluates <see cref="UpDownBase.IsReadOnly"/> and checks if the value is less than the maximum
        /// </summary>
        /// <returns>TRUE if the value can be increased</returns>
        protected virtual bool CanIncreaseValue()
        {
            return !IsReadOnly && (AllowOutsideRange | IsLessThan(Maximum, Value));
        }

        /// <summary>
        /// Evaluates <see cref="UpDownBase.IsReadOnly"/> and checks if the value is greater than the minimum
        /// </summary>
        /// <returns>TRUE if the value can be decreased</returns>
        protected virtual bool CanDecreaseValue()
        {
            return !IsReadOnly && (AllowOutsideRange | IsGreaterThan(Minimum, Value));
        }

        /// <summary>
        /// Handle the paste event
        /// </summary>
        protected virtual void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (IsReadOnly) goto Handled;

            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText) goto Handled;

            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
            if (TryParse(text, out T val))
            {
                Value = val;
            }

        Handled:
            e.CancelCommand();
            e.Handled = true;
        }

        #region < Minimum >

        /// <summary>
        /// The Minimum value the control allows
        /// </summary>
        public virtual T Minimum
        {
            get { return (T)GetValue(MinimumProperty); }
            set
            {
                var oldVal = Minimum;
                if (NotEqual(oldVal, value))
                {
                    if (IsGreaterThanMax(value)) throw new ArgumentException("Minimum cannot be greater than Maximum!");
                    SetValue(MinimumProperty, value);
                }
            }
        }

        /// <summary>
        /// Set the Minimum value for this control
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(T), typeof(UpDownBase<T>), new PropertyMetadata(default));

        private static object CoerceMinimum(DependencyObject d, object baseValue)
        {
            var obj = (UpDownBase<T>)d;
            var value = (T)baseValue;
            if (obj.IsLessThan(obj.Maximum, value))
                return value;
            else
            {
                return obj.Maximum;
            }
        }
        #endregion

        #region < Maximum >
        /// <summary>
        /// The maximum value the control allows
        /// </summary>
        public virtual T Maximum
        {
            get { return (T)GetValue(MaximumProperty); }
            set
            {
                var oldVal = Maximum;
                if (NotEqual(oldVal, value))
                {
                    if (IsLessThanMin(value)) throw new ArgumentException("Maximum cannot be lower than minimum!");
                    SetValue(MaximumProperty, value);
                }
            }
        }

        /// <summary>
        /// Set the Maximum value for this control
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(T), typeof(UpDownBase<T>), new PropertyMetadata(default));

        private static object CoerceMaximum(DependencyObject d, object baseValue)
        {
            var obj = (UpDownBase<T>)d;
            var value = (T)baseValue;
            if (obj.IsGreaterThan(obj.Minimum, value))
                return value;
            else
            {
                return obj.Minimum;
            }
        }
        #endregion

        #region < Value >
        /// <summary>
        /// The T value of the control
        /// </summary>
        public virtual T Value
        {
            get { return (T)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Set the Value of the control
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(T), typeof(UpDownBase<T>), new PropertyMetadata(default(T)));

        #endregion

        #region < Increment Values >

        /// <summary>
        /// The value to increment/decrement the control by when pressing a button
        /// </summary>
        public virtual T SmallChange
        {
            get { return (T)GetValue(SmallChangeProperty); }
            set
            {
                var oldVal = SmallChange;
                if (NotEqual(oldVal, value))
                {
                    if (IsGreaterThanMax(value)) throw new ArgumentException("Increment value cannot be greater than Maximum Value!");
                    SetValue(SmallChangeProperty, value);
                }
            }
        }

        /// <inheritdoc cref="SmallChange"/>
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(nameof(SmallChange), typeof(T), typeof(UpDownBase<T>), new PropertyMetadata(default));

        /// <summary>
        /// The value to increment/decrement the control by when pressing a button
        /// </summary>
        public virtual T LargeChange
        {
            get { return (T)GetValue(LargeChangeProperty); }
            set
            {
                var oldVal = SmallChange;
                if (NotEqual(oldVal, value))
                {
                    if (IsGreaterThanMax(value)) throw new ArgumentException("Increment value cannot be greater than Maximum Value!");
                    SetValue(LargeChangeProperty, value);
                }
            }
        }

        /// <inheritdoc cref="LargeChange"/>
        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register(nameof(LargeChange), typeof(T), typeof(UpDownBase<T>), new PropertyMetadata(default));

        private static object CoerceIncrements(DependencyObject d, object baseValue)
        {
            var obj = (UpDownBase<T>)d;
            var value = (T)baseValue;
            if (obj.IsGreaterOrEqualToMax(value))
                return obj.Maximum;
            else if (obj.IsLessOrEqualToMin(value))
                return obj.Minimum;
            else
                return value;
        }

        #endregion

        #region < Allow Outside Range >
        /// <summary>
        /// Set a flag determining if the value of the control can be set outside the range
        /// </summary>
        public bool AllowOutsideRange
        {
            get { return (bool)GetValue(AllowOutsideRangeProperty); }
            set { SetValue(AllowOutsideRangeProperty, value); }
        }

        /// <inheritdoc cref="AllowOutsideRange"/>
        public static readonly DependencyProperty AllowOutsideRangeProperty =
            DependencyProperty.Register(nameof(AllowOutsideRange), typeof(bool), typeof(UpDownBase<T>), new PropertyMetadata(false));
        #endregion

        #region < DefaultValue >

        /// <summary>
        /// Set the Default Value for the control
        /// </summary>
        public T DefaultValue
        {
            get { return (T)GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        object IUpDown.Value { get => this.Value; set { if (value is T val) this.Value = val; else throw new ArgumentException($"Cannot set Value property - expected value of type {typeof(T)}"); } }
        object IUpDown.Minimum { get => this.Minimum; set { if (value is T val) this.Minimum = val; else throw new ArgumentException($"Cannot set Minimum property - expected value of type {typeof(T)}"); } }
        object IUpDown.Maximum { get => this.Maximum; set { if (value is T val) this.Maximum = val; else throw new ArgumentException($"Cannot set Maximum property - expected value of type {typeof(T)}"); } }
        object IUpDown.SmallChange { get => this.SmallChange; set { if (value is T val) this.SmallChange = val; else throw new ArgumentException($"Cannot set SmallChange property - expected value of type {typeof(T)}"); } }
        object IUpDown.LargeChange { get => this.LargeChange; set { if (value is T val) this.LargeChange = val; else throw new ArgumentException($"Cannot set LargeChange property - expected value of type {typeof(T)}"); } }

        /// <inheritdoc cref="DefaultValue"/>
        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register(nameof(DefaultValue), typeof(T), typeof(UpDownBase<T>), new PropertyMetadata(default(T), EvaluateIsDirty));

        #endregion

        /// <summary>
        /// PropertyChangedCallback that does nothing
        /// </summary>
        private static void DoNothingCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var obj = (UpDownBase<T>)d;
            var value = (T)baseValue;
            if (obj.AllowOutsideRange)
            {
                EvaluateIsValid(obj, value);
                return value;
            }
            else
            {
                if (obj.IsGreaterOrEqualToMax(value))
                    return obj.Maximum;
                else if (obj.IsLessOrEqualToMin(value))
                    return obj.Minimum;
                else
                    return value;
            }
        }

        /// <summary>
        /// Compares the Default Value to the Current Value and sets IsDirty
        /// </summary>
        private static void EvaluateIsDirty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as UpDownBase<T>;
            obj.IsDirty = !EqualityComparer<T>.Default.Equals(obj.DefaultValue, obj.Value);
            obj.DecreaseValueCommand.NotifyCanExecuteChanged();
            obj.IncreaseValueCommand.NotifyCanExecuteChanged();
        }

        private static void RangeUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as UpDownBase<T>;
            EvaluateIsValid(obj, obj.Value);
        }

        /// <summary>
        /// Checks if the value is outside the min/max range
        /// </summary>
        private static void EvaluateIsValid(UpDownBase<T> obj, T value)
        {
            obj.IsValid = !(obj.IsGreaterThanMax(value) || obj.IsLessThanMin(value));
        }
        
    }
}
