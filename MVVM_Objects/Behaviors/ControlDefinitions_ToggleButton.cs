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
        /// Assigns an <see cref="IToggleButtonDefinition"/> to a <see cref="ToggleButton"/>
        /// </summary>
        public static readonly DependencyProperty ToggleButtonDefinitionProperty =
            DependencyProperty.RegisterAttached("ToggleButtonDefinition",
                typeof(IToggleButtonDefinition),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, ToggleButtonDefinitionPropertyChanged)
                );

        private static void ToggleButtonDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ToggleButton btn)
            {
                if (args.OldValue is IToggleButtonDefinition oldDef)
                {
                    UnbindToggleButtonDefinition(btn, oldDef);
                }
                if (args.NewValue is IToggleButtonDefinition def)
                {
                    BindToggleButtonDefinition(btn, def);
                }
            }
        }

        private static void BindToggleButtonDefinition(ToggleButton btn, IToggleButtonDefinition def)
        {
            BindIControlDefinition(btn, def);
            SetBinding(btn, ToggleButton.IsCheckedProperty, nameof(def.IsChecked), def);
            SetBinding(btn, ToggleButton.IsThreeStateProperty, nameof(def.IsThreeState), def);
            SetContent(btn, def);
        }

        private static void UnbindToggleButtonDefinition(ToggleButton btn, IToggleButtonDefinition oldDef)
        {
            UnbindIControlDefinition(btn, oldDef);
            UnbindIfBound(btn, ToggleButton.IsCheckedProperty, oldDef);
            UnbindIfBound(btn, ToggleButton.IsThreeStateProperty, oldDef);
        }

        /// <summary>
        /// Gets the assigned <see cref="IToggleButtonDefinition"/> from a <see cref="ToggleButton"/>
        /// </summary>
        public static IToggleButtonDefinition GetToggleButtonDefinition(DependencyObject obj)
        {
            return (IToggleButtonDefinition)obj.GetValue(ToggleButtonDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IToggleButtonDefinition"/> to a <see cref="ToggleButton"/>
        /// </summary>
        public static void SetToggleButtonDefinition(DependencyObject obj, IToggleButtonDefinition value)
        {
            obj.SetValue(ToggleButtonDefinitionProperty, value);
        }
    }
}
