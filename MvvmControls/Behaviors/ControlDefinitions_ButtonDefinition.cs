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
        /// <summary> </summary>
        public static readonly DependencyProperty ButtonDefinitionProperty =
            DependencyProperty.RegisterAttached(
                "ButtonDefinition",
                typeof(IButtonDefinition),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, ButtonDefinitionPropertyChanged));

        private static void ButtonDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is Button cntrl)
            {
                if (args.OldValue is IButtonDefinition oldDef)
                {
                    UnbindIControlDefinition(cntrl, oldDef);
                    UnbindIfBound(cntrl, Button.CommandProperty, oldDef);
                }
                if (args.NewValue is IButtonDefinition def)
                {
                    BindIControlDefinition(cntrl, def);

                    //Binding
                    BindingOperations.SetBinding(cntrl, Button.CommandProperty, new Binding()
                    {
                        Source = def,
                        Mode = BindingMode.OneWay,

                    });

                }
                SetContent(cntrl, args.NewValue);
            }
        }

        /// <summary> Get <see cref="ButtonDefinitionProperty"/> </summary>
        public static IButtonDefinition GetButtonDefinition(DependencyObject obj)
        {
            return (IButtonDefinition)obj.GetValue(ButtonDefinitionProperty);
        }

        /// <summary> Set <see cref="ButtonDefinitionProperty"/> </summary>
        public static void SetButtonDefinition(DependencyObject obj, IButtonDefinition value)
        {
            obj.SetValue(ButtonDefinitionProperty, value);
        }
    }
}
