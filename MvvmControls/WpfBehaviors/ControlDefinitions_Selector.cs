using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace RFBCodeWorks.WPFBehaviors
{
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Assigns an <see cref="IRadioButton"/> to a <see cref="RadioButton"/>
        /// </summary>
        public static readonly DependencyProperty SelectorDefinitionProperty =
            DependencyProperty.RegisterAttached("SelectorDefinition",
                typeof(ISelector),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, SelectorDefinitionPropertyChanged)
                );

        private static void SelectorDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is System.Windows.Controls.Primitives.Selector cntrl)
            {
                if (args.OldValue is ISelector oldDef)
                {
                    UnbindIfBound(cntrl, Selector.SelectedItemProperty, oldDef);
                    UnbindIfBound(cntrl, Selector.SelectedValueProperty, oldDef);
                    UnbindIfBound(cntrl, Selector.SelectedValuePathProperty, oldDef);
                    UnbindItemSource(cntrl, oldDef);
                }
                
                if (args.NewValue is ISelector def)
                {
                    BindItemSource(cntrl, def);

                    //Selected Value Path
                    SetBinding(cntrl, Selector.SelectedValuePathProperty, nameof(ISelector.SelectedValuePath), def, BindingMode.OneWay);

                    //Selected Value
                    SetBinding(cntrl, Selector.SelectedValueProperty, nameof(ISelector.SelectedValue), def, BindingMode.TwoWay);

                    //Selected Value
                    SetBinding(cntrl, Selector.SelectedItemProperty, nameof(ISelector.SelectedItem), def, BindingMode.TwoWay);
                }
            }
        }

        /// <summary>
        /// Gets the assigned <see cref="IRadioButton"/> from a <see cref="ComboBox"/>
        /// </summary>
        public static ISelector GetSelectorDefinition(DependencyObject obj)
        {
            return (ISelector)obj.GetValue(SelectorDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IRadioButton"/> to a <see cref="ComboBox"/>
        /// </summary>
        public static void SetSelectorDefinition(DependencyObject obj, ISelector value)
        {
            obj.SetValue(SelectorDefinitionProperty, value);
        }
    }
}
