using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.Input;

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
        protected override void LargeDecrement() => Value -= LargeChange;
        /// <inheritdoc/>
        protected override void LargeIncrement() => Value += LargeChange;
        /// <inheritdoc/>
        protected override void SmallDecrement() => Value -= SmallChange;
        /// <inheritdoc/>
        protected override void SmallIncrement() => Value += SmallChange;
        /// <inheritdoc/>
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
