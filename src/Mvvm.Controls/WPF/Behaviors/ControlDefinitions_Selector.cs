using RFBCodeWorks.Mvvm;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace RFBCodeWorks.WPF.Behaviors
{
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Assigns an <see cref="ISelector"/> to a <see cref="System.Windows.Controls.Primitives.Selector"/>
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
                    UnbindIfBound(cntrl, Selector.SelectedIndexProperty, oldDef);
                    UnbindItemSource(cntrl, oldDef);
                }
                
                if (args.NewValue is ISelector def)
                {
                    BindItemSource(cntrl, def);

                    //Selected Value Path
                    SetBinding(cntrl, Selector.SelectedValuePathProperty, nameof(ISelector.SelectedValuePath), def, BindingMode.OneWay);

                    // Prefer Selected Item over all others, as it is most efficient for most cases
                    // if not preferring this, dev. should bind to other properties normally.
                    SetBinding(cntrl, Selector.SelectedItemProperty, nameof(ISelector.SelectedItem), def, BindingMode.TwoWay);
                    
                    // unbind from other properties to ensure feedback loop does not occur
                    UnbindIfBound(cntrl, Selector.SelectedIndexProperty, def);
                    UnbindIfBound(cntrl, Selector.SelectedValueProperty, def);
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
