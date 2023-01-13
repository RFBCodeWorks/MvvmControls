using RFBCodeWorks.Mvvm;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RFBCodeWorks.WPF.Behaviors
{
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Assigns an <see cref="IListBox"/> to a <see cref="ListBox"/> or <see cref="ListView"/>
        /// </summary>
        public static readonly DependencyProperty TextControlDefinitionProperty =
            DependencyProperty.RegisterAttached("TextControlDefinition",
                typeof(ITextControl),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, TextControlDefinitionPropertyChanged)
                );

        private static void TextControlDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is TextBox cntrl)
            {   
                if (args.OldValue is ITextControl oldDef)
                {
                    UnbindIControlDefinition(cntrl, oldDef);
                    UnbindIfBound(cntrl, TextBox.TextProperty, oldDef);
                }
                if (args.NewValue is ITextControl def)
                {
                    BindIControlDefinition(cntrl, def);
                    var bind = GetBindingInfo(cntrl, TextControlDefinitionProperty);
                    if (def is System.ComponentModel.INotifyDataErrorInfo IDN)
                    {
                        BindingOperations.SetBinding(cntrl, TextBox.TextProperty, new Binding(nameof(def.Text))
                        {
                            Source = def,
                            UpdateSourceTrigger = bind.UpdateSourceTrigger,
                            ValidatesOnNotifyDataErrors = true
                        });
                    }
                    else if (def is System.ComponentModel.IDataErrorInfo IDE)
                    {
                        BindingOperations.SetBinding(cntrl, TextBox.TextProperty, new Binding(nameof(def.Text))
                        {
                            Source = def,
                            UpdateSourceTrigger = bind.UpdateSourceTrigger,
                            ValidatesOnDataErrors = true
                        });
                    }
                    else
                    {
                        SetBinding(cntrl, TextBox.TextProperty, nameof(def.Text), def, BindingMode.TwoWay, bind.UpdateSourceTrigger);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the assigned <see cref="IListBox"/> from a <see cref="ListBox"/>
        /// </summary>
        public static ITextControl GetTextControlDefinition(DependencyObject obj)
        {
            return (ITextControl)obj.GetValue(TextControlDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IListBox"/> to a <see cref="ListBox"/>
        /// </summary>
        public static void SetTextControlDefinition(DependencyObject obj, ITextControl value)
        {
            obj.SetValue(TextControlDefinitionProperty, value);
        }
    }
}
