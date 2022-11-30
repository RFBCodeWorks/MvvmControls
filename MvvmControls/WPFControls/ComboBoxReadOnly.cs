using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.WPFControls
{
    /// <summary>
    /// Interaction logic for a Read Only ComboBox
    /// </summary>
    /// <remarks><see href="https://stackoverflow.com/questions/4882193/xaml-readonly-combobox"/></remarks>
    public partial class ComboBoxReadOnly : ComboBox
    {
        static ComboBoxReadOnly()
        {
            IsDropDownOpenProperty.OverrideMetadata(typeof(ComboBoxReadOnly), new FrameworkPropertyMetadata(
            propertyChangedCallback: delegate { },
            coerceValueCallback: (d, value) =>
            {
                if (((ComboBoxReadOnly)d).IsReadOnly)
                {
                    // Prohibit opening the drop down when read only.
                    return false;
                }

                return value;
            }));

            IsReadOnlyProperty.OverrideMetadata(typeof(ComboBoxReadOnly), new FrameworkPropertyMetadata(
                propertyChangedCallback: (d, e) =>
                {
                    // When setting "read only" to false, close the drop down.
                    if (e.NewValue is true)
                    {
                        ((ComboBoxReadOnly)d).IsDropDownOpen = false;
                    }
                }));
        }

        /// <summary>
        /// Initialize the ReadOnly ComboBox
        /// </summary>
        public ComboBoxReadOnly() : base()
        {
            
        }

        /// <inheritdoc/>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (IsReadOnly)
            {
                // Disallow changing the selection when read only.
                e.Handled = true;
                return;
            }
            base.OnSelectionChanged(e);
        }

    }
}
