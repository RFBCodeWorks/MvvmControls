using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using RFBCodeWorks.MVVMObjects.ControlInterfaces;

namespace RFBCodeWorks.MVVMObjects.Behaviors
{
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Assigns an <see cref="IListBoxDefinition"/> to a <see cref="ListBox"/> or <see cref="ListView"/>
        /// </summary>
        public static readonly DependencyProperty ListBoxDefinitionProperty =
            DependencyProperty.RegisterAttached("ListBoxDefinition",
                typeof(IListBoxDefinition),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, ListBoxDefinitionPropertyChanged)
                );

        private static void ListBoxDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ListBox cntrl)
            {
                if (args.OldValue is IListBoxDefinition oldDef)
                {
                    UnbindSelectorDefinition(cntrl, oldDef);
                    UnbindIfBound(cntrl, ListBox.SelectionModeProperty, oldDef);

                }
                if (args.NewValue is IListBoxDefinition def)
                {
                    BindSelectorDefinition(cntrl, def);

                    //Selection Mode
                    BindingOperations.SetBinding(cntrl, ListBox.SelectionModeProperty, new Binding(nameof(IListBoxDefinition.SelectionMode))
                    {
                        Source = def,
                        Mode = BindingMode.OneWay
                    });
                }
            }
        }

        /// <summary>
        /// Gets the assigned <see cref="IListBoxDefinition"/> from a <see cref="ListBox"/>
        /// </summary>
        public static IListBoxDefinition GetListBoxDefinition(DependencyObject obj)
        {
            return (IListBoxDefinition)obj.GetValue(ListBoxDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IListBoxDefinition"/> to a <see cref="ListBox"/>
        /// </summary>
        public static void SetListBoxDefinition(DependencyObject obj, IListBoxDefinition value)
        {
            obj.SetValue(ListBoxDefinitionProperty, value);
        }
    }
}
