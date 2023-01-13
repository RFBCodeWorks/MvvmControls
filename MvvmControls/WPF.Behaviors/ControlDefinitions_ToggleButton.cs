using RFBCodeWorks.Mvvm;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace RFBCodeWorks.WPF.Behaviors
{
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Assigns an <see cref="IToggleButton"/> to a <see cref="ToggleButton"/>
        /// </summary>
        public static readonly DependencyProperty ToggleButtonDefinitionProperty =
            DependencyProperty.RegisterAttached("ToggleButtonDefinition",
                typeof(IToggleButton),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, ToggleButtonDefinitionPropertyChanged)
                );

        private static void ToggleButtonDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ToggleButton btn)
            {
                if (args.OldValue is IToggleButton oldDef)
                {
                    UnbindIControlDefinition(btn, oldDef);
                    UnbindIfBound(btn, ToggleButton.IsCheckedProperty, oldDef);
                    UnbindIfBound(btn, ToggleButton.IsThreeStateProperty, oldDef);
                    UnbindIfBound(btn, ButtonBase.ContentProperty, oldDef);
                }
                if (args.NewValue is IToggleButton def)
                {
                    BindIControlDefinition(btn, def);
                    SetBinding(btn, ToggleButton.IsCheckedProperty, nameof(def.IsChecked), def);
                    SetBinding(btn, ToggleButton.IsThreeStateProperty, nameof(def.IsThreeState), def);
                    SetContent(btn, def);
                }
            }
        }

        /// <summary>
        /// Gets the assigned <see cref="IToggleButton"/> from a <see cref="ToggleButton"/>
        /// </summary>
        public static IToggleButton GetToggleButtonDefinition(DependencyObject obj)
        {
            return (IToggleButton)obj.GetValue(ToggleButtonDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IToggleButton"/> to a <see cref="ToggleButton"/>
        /// </summary>
        public static void SetToggleButtonDefinition(DependencyObject obj, IToggleButton value)
        {
            obj.SetValue(ToggleButtonDefinitionProperty, value);
        }
    }
}
