using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RFBCodeWorks.MvvmControls.Behaviors
{
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Assigns an <see cref="IListBox"/> to a <see cref="ListBox"/> or <see cref="ListView"/>
        /// </summary>
        public static readonly DependencyProperty ListBoxDefinitionProperty =
            DependencyProperty.RegisterAttached("ListBoxDefinition",
                typeof(IListBox),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, ListBoxDefinitionPropertyChanged)
                );

        private static void ListBoxDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ListBox cntrl)
            {
                SetSelectorDefinition(cntrl, args.NewValue as ISelector);
                
                if (args.OldValue is IListBox oldDef)
                {
                    UnbindIfBound(cntrl, ListBox.SelectionModeProperty, oldDef);
                }
                if (args.NewValue is IListBox def)
                {
                    //Selection Mode
                    BindingOperations.SetBinding(cntrl, ListBox.SelectionModeProperty, new Binding(nameof(IListBox.SelectionMode))
                    {
                        Source = def,
                        Mode = BindingMode.OneWay
                    });
                }
            }
        }

        /// <summary>
        /// Gets the assigned <see cref="IListBox"/> from a <see cref="ListBox"/>
        /// </summary>
        public static IListBox GetListBoxDefinition(DependencyObject obj)
        {
            return (IListBox)obj.GetValue(ListBoxDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IListBox"/> to a <see cref="ListBox"/>
        /// </summary>
        public static void SetListBoxDefinition(DependencyObject obj, IListBox value)
        {
            obj.SetValue(ListBoxDefinitionProperty, value);
        }
    }
}
