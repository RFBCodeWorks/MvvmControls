using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomControls.WPF
{
    public static class TextBoxHelper
    {
        #region Enum Declarations

        public enum NumericFormat
        {
            /// <summary>
            /// Accepts input as DOUBLE
            /// </summary>
            Double,
            /// <summary>
            /// Accepts only Integer Values
            /// </summary>
            Int,
        }

        #endregion

        #region Dependency Properties & CLR Wrappers

        public static readonly DependencyProperty OnlyNumericProperty =
            DependencyProperty.RegisterAttached("OnlyNumeric", typeof(NumericFormat?), typeof(TextBoxHelper),
                new PropertyMetadata(null, DependencyPropertiesChanged));

        public static void SetOnlyNumeric(TextBox element, NumericFormat value) =>
            element.SetValue(OnlyNumericProperty, value);

        public static NumericFormat GetOnlyNumeric(TextBox element) =>
            (NumericFormat)element.GetValue(OnlyNumericProperty);


        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.RegisterAttached("DefaultValue", typeof(string), typeof(TextBoxHelper),
                new PropertyMetadata(null, DependencyPropertiesChanged));
        public static void SetDefaultValue(TextBox element, string value) =>
            element.SetValue(DefaultValueProperty, value);
        public static string GetDefaultValue(TextBox element) => (string)element.GetValue(DefaultValueProperty);


        public static readonly DependencyProperty AllowNegativesProperty =
            DependencyProperty.RegisterAttached("AllowNegative", typeof(bool), typeof(TextBoxHelper),
                new PropertyMetadata(null, DependencyPropertiesChanged));
        public static void SetAllowNegative(TextBox element, bool value) =>
            element.SetValue(AllowNegativesProperty, value);
        public static bool GetAllowNegative(TextBox element) => (bool)element.GetValue(AllowNegativesProperty);

        #endregion

        #region Dependency Properties Methods

        private static void DependencyPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TextBox textBox))
                throw new Exception("Attached property must be used with TextBox.");

            switch (e.Property.Name)
            {
                case "OnlyNumeric":
                    {
                        var castedValue = (NumericFormat?)e.NewValue;

                        if (castedValue.HasValue)
                        {
                            textBox.PreviewTextInput += TextBox_PreviewTextInput;
                            DataObject.AddPastingHandler(textBox, TextBox_PasteEventHandler);
                        }
                        else
                        {
                            textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                            DataObject.RemovePastingHandler(textBox, TextBox_PasteEventHandler);
                        }

                        break;
                    }

                case "DefaultValue":
                    {
                        var castedValue = (string)e.NewValue;

                        if (castedValue != null)
                        {
                            //textBox.TextChanged += TextBox_TextChanged;
                        }
                        else
                        {
                            //textBox.TextChanged -= TextBox_TextChanged;
                        }

                        break;
                    }
            }
        }

        #endregion

        /// <returns>TRUE if handled, FALSE is not ( False = accept the action )</returns>
        private static bool EvaluateString(TextBox textBox, string value)
        {
            bool AllowNegative = GetAllowNegative(textBox);

            switch (GetOnlyNumeric(textBox))
            {
                case NumericFormat.Double:
                    {
                        if (double.TryParse(value, out double number))
                        {
                            if (AllowNegative)
                                return false;  // Accept the input
                            else
                                return !(number > 0);
                        }
                        else
                            return true;
                    }

                case NumericFormat.Int:
                    {
                        if (int.TryParse(value, out int number))
                        {
                            if (AllowNegative)
                                return false;  // Accept the input
                            else
                                return !(number > 0);
                        }
                        else
                            return true;
                    }
            }
            throw new NotImplementedException(GetOnlyNumeric(textBox).GetName() + " Evaluation not implemented.");
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;

            string newText;

            if (textBox.SelectionLength == 0)
            {
                newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            }
            else
            {
                var textAfterDelete = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);

                newText = textAfterDelete.Insert(textBox.SelectionStart, e.Text);
            }

            e.Handled = EvaluateString(textBox, newText);

        }

        private static void TextBox_PasteEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            var textBox = (TextBox)sender;

            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var clipboardText = (string)e.DataObject.GetData(typeof(string));

                var newText = textBox.Text.Insert(textBox.SelectionStart, clipboardText);

                if (EvaluateString(textBox, newText))
                    e.CancelCommand();

            }
            else
            {
                e.CancelCommand();
            }
        }


    }
}
