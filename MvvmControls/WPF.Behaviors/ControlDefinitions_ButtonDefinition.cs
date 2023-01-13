using RFBCodeWorks.Mvvm;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RFBCodeWorks.WPF.Behaviors
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

                    //Bind the COMMAND property
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
