using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RFBCodeWorks.WPF.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class DecimalUpDown : Primitives.UpDownBase<double>
    {
        static DecimalUpDown()
        {
            OverrideMetaData(typeof(DecimalUpDown), 1, 10, 0, 100);
            StringFormatProperty.OverrideMetadata(typeof(DecimalUpDown), new PropertyMetadata("d"));
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void LargeDecrement() => Value -= LargeChange;
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void LargeIncrement() => Value += LargeChange;
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void SmallDecrement() => Value -= SmallChange;
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void SmallIncrement() => Value += SmallChange;
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override bool TryParse(string pastedValue, out double value) => double.TryParse(pastedValue, out value);

        /// <inheritdoc/>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Handled) return;
            if (e.Key == Key.OemPeriod)
            {
                e.Handled = false;
                return;
            }
            base.OnPreviewKeyDown(e);
        }

        /// <inheritdoc/>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                if (e.OriginalSource is TextBox txt && !txt.Text.Contains("."))
                    return;
                else
                    e.Handled = true;
                return;
            }
                
            base.OnPreviewTextInput(e);
        }
    }
}
