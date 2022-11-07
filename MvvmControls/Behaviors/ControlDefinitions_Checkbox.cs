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
using RFBCodeWorks.MvvmControls.ControlInterfaces;

namespace RFBCodeWorks.MvvmControls.Behaviors
{
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Assigns an <see cref="IComboBoxDefinition"/> to a <see cref="CheckBox"/>
        /// </summary>
        public static readonly DependencyProperty CheckBoxDefinitionProperty =
            DependencyProperty.RegisterAttached("CheckBoxDefinition",
                typeof(ICheckBoxDefinition),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, CheckBoxDefinitionPropertyChanged)
                );

        private static void CheckBoxDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is CheckBox cmb)
            {
                if (args.OldValue is ICheckBoxDefinition oldDef)
                {
                    if (GetToggleButtonDefinition(cmb) == args.OldValue)
                        SetToggleButtonDefinition(cmb, null);
                    else
                        UnbindToggleButtonDefinition(cmb, oldDef);
                }
                if (args.NewValue is ICheckBoxDefinition def)
                {
                    SetToggleButtonDefinition(cmb, def);
                }
                SetContent(cmb, args.NewValue);
            }
        }

        /// <summary>
        /// Gets the assigned <see cref="ICheckBoxDefinition"/> from a <see cref="CheckBox"/>
        /// </summary>
        public static ICheckBoxDefinition GetCheckBoxDefinition(DependencyObject obj)
        {
            return (ICheckBoxDefinition)obj.GetValue(CheckBoxDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IComboBoxDefinition"/> to a <see cref="CheckBox"/>
        /// </summary>
        public static void SetCheckBoxDefinition(DependencyObject obj, ICheckBoxDefinition value)
        {
            obj.SetValue(CheckBoxDefinitionProperty, value);
        }
    }
}
