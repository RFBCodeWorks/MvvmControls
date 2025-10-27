using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RFBCodeWorks.Mvvm.Specialized
{
    /// <summary>
    /// ViewModel for a ComboBox that is filled with integers
    /// </summary>
    public class IntegerPicker : ViewModelBase
    {
        /// <summary>
        /// Generate a new int[] array between 0 and <paramref name="max"/>
        /// </summary>
        /// <returns> <paramref name="max"/> = 10 -> { 0, 1, ..., 8, 9 }</returns>
        /// <inheritdoc cref="GenerateArray(int, int, bool)"/>
        public static int[] GenerateArray(int max, bool includeMax = false) => GenerateArray(0, max, includeMax);

        /// <summary>
        /// Generate a new int[] array between <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <param name="includeMax">Include the maximum number in the output array. Default does not include the <paramref name="max"/></param>
        /// <returns> 
        /// <paramref name="min"/> = 0 <br/>
        /// <paramref name="max"/> = 10 <br/>
        /// returns: { 0, 1, ..., 8, 9 }</returns>
        public static int[] GenerateArray(int min, int max, bool includeMax = false)
        {
            List<int> list = new(max);
            int i = min;
            while (i < max)
            {
                list.Add(i);
                i++;
            }
            if (includeMax) list.Add(max);
            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns/>
        /// <inheritdoc cref="GenerateArray(int, int, bool)"/>
        public IntegerPicker(int max, bool includeMax, ViewModelBase parent = null) : base(parent) 
        {
            Range = GenerateArray(max, includeMax);
            AllowNegatives = false;
        }

        /// <inheritdoc cref="IntegerPicker.IntegerPicker(int, bool, ViewModelBase)"/>
        public IntegerPicker(int min, int max, bool includeMax = false, ViewModelBase parent = null) : base(parent)
        {
            Range = GenerateArray(min, max, includeMax);
            AllowNegatives = min < 0;
        }

        /// <inheritdoc cref="IntegerPicker.IntegerPicker(int, bool, ViewModelBase)"/>
        public IntegerPicker(params int[] values) : base()
        {
            Range = values;
            AllowNegatives = values.Any((i) => i < 0);
        }

        /// <inheritdoc cref="IntegerPicker.IntegerPicker(int, bool, ViewModelBase)"/>
        public IntegerPicker(IViewModel parent, params int[] values) : base(parent)
        {
            Range = values;
            AllowNegatives = values.Any((i) => i < 0);
        }

        /// <summary>
        /// The range of numbers
        /// </summary>
        public int[] Range { get; }

        private bool AllowNegatives;

        /// <summary>
        /// Binding for integer-Only comboxes
        /// </summary>
        public int Value
        {
            get { return ValueField; }
            set
            {
                SetProperty(ref ValueField, value, nameof(Value)); OnValueUpdated();
                if (!updatingValue)
                {
                    updatingValue = true;
                    try
                    {
                        UnsafeValue = value.ToString(NumberFormat);
                    }
                    catch
                    {
                        UnsafeValue = value.ToString();
                    }
                    updatingValue = false;
                }
            }
        }
        private int ValueField;
        private bool updatingValue;

        /// <summary>
        /// Unsafe binding that accepts wildcards in the form of an asterisk. <br/>
        /// if the value is a valid int, it also updates the <see cref="Value"/> field.
        /// </summary>
        public string UnsafeValue
        {
            get { return string.IsNullOrWhiteSpace(StringValueField) ? Value.ToString() : StringValueField.Trim(); }
            set {
                
                if (updatingValue)
                {
                    SetProperty(ref StringValueField, value, nameof(UnsafeValue));
                    OnUnsafeValueUpdated();
                    return;
                }

                updatingValue = true;
                string val = value?.Trim() ?? string.Empty;

                Regex intMatch = AllowNegatives ? new Regex("^[-][0-9]+$", RegexOptions.Compiled) : new Regex("^[0-9]+$", RegexOptions.Compiled);
                Regex wildcardMatch = AllowNegatives ? new Regex("^[-][[0-9*?]+$", RegexOptions.Compiled) : new Regex("^[[0-9*?]+$", RegexOptions.Compiled);

                //Value = zero
                if (val == string.Empty)
                {
                    SetProperty(ref StringValueField, val, nameof(UnsafeValue));
                    OnUnsafeValueUpdated();
                    Value = 0;
                }

                //Check for integer-only string
                else if (intMatch.IsMatch(val))
                {
                    SetProperty(ref StringValueField, val, nameof(UnsafeValue));
                    OnUnsafeValueUpdated();
                    
                    //Update the VALUE property
                    Value = int.Parse(value);
                }

                // Check for wildcards
                else if (wildcardMatch.IsMatch(val))
                {
                    SetProperty(ref StringValueField, val, nameof(UnsafeValue));
                    OnUnsafeValueUpdated();
                }

                // Notify the UI to reject the value and restore previous value
                else
                {
                    OnPropertyChanged(nameof(UnsafeValue));
                }
                updatingValue = false;
            }
        }
        private string StringValueField = "*";


        /// <summary>
        /// <inheritdoc cref="int.ToString(string)" path="/param[@name='format']"/> <br/>
        /// </summary>
        public string NumberFormat
        {
            get { return NumberFormatField; }
            set { SetProperty(ref NumberFormatField, value, nameof(NumberFormat)); }
        }
        private string NumberFormatField = "#";


        #region < ValueUpdated >

        /// <summary>
        /// Delegate for the ValueUpdated event
        /// </summary>
        public delegate void ValueUpdatedHandler(object sender, EventArgs e);

        /// <summary>
        /// 
        /// </summary>
        public event ValueUpdatedHandler ValueUpdated;

        /// <summary> Raises the ValueUpdated event </summary>
        protected virtual void OnValueUpdated()
        {
            ValueUpdated?.Invoke(this, new EventArgs());
        }

        /// <summary> Raises the ValueUpdated event </summary>
        protected virtual void OnValueUpdated(EventArgs e)
        {
            ValueUpdated?.Invoke(this, e);
        }

        #endregion


        #region < UnsafeValueUpdated >

        /// <summary>
        /// Delegate for the UnsafeValueUpdated event
        /// </summary>
        public delegate void UnsafeValueUpdatedHandler(object sender, EventArgs e);

        /// <summary>
        /// 
        /// </summary>
        public event UnsafeValueUpdatedHandler UnsafeValueUpdated;

        /// <summary> Raises the UnsafeValueUpdated event </summary>
        protected virtual void OnUnsafeValueUpdated()
        {
            UnsafeValueUpdated?.Invoke(this, new EventArgs());
        }

        /// <summary> Raises the UnsafeValueUpdated event </summary>
        protected virtual void OnUnsafeValueUpdated(EventArgs e)
        {
            UnsafeValueUpdated?.Invoke(this, e);
        }

        #endregion



    }
}
