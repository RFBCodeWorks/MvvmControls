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

namespace RFBCodeWorks.MVVMObjects.Behaviors
{
    /// <summary>
    /// Contains the AttachedProperties to bind controls to their definitions
    /// </summary>
    public static class ControlDefinitions
    {

        private static void UnbindIfBound(DependencyObject obj, DependencyProperty prop, object source)
        {
            var binding = BindingOperations.GetBindingExpression(obj, prop)?.ParentBinding;
            if ((binding?.Source is null) || binding.Source == "") return;
            if (binding.Source == source) 
                BindingOperations.ClearBinding(obj, Control.VisibilityProperty);
        }

        #region < IControlDefinition >

        /// <summary>
        /// Bind to Visibility and ToolTip
        /// </summary>
        private static void BindIControlDefinition(Control obj, IControlDefinition def)
        {
            //Visibility
            BindingOperations.SetBinding(obj, Control.VisibilityProperty, new Binding("Visibility")
            {
                Source = def,
                Mode = BindingMode.OneWay
            });

            //ToolTip
            BindingOperations.SetBinding(obj, ItemsControl.ToolTipProperty, new Binding("ToolTip")
            {
                Source = def,
                Mode = BindingMode.OneWay
            });
        }

        /// <summary>
        /// Unbind Visibiltiy and ToolTip
        /// </summary>
        private static void UnbindIControlDefinition(Control obj, IControlDefinition def)
        {
            UnbindIfBound(obj, Control.VisibilityProperty, def);
            UnbindIfBound(obj, Control.ToolTipProperty, def);
        }

        #endregion

        #region < ItemSourceDefinition >

        ///// <summary>
        ///// Gets the assigned <see cref="IComboBoxDefinition"/> from a <see cref="System.Windows.Controls.ComboBox"/>
        ///// </summary>
        //public static IComboBoxDefinition GetMyProperty(DependencyObject obj)
        //{
        //    return (IComboBoxDefinition)obj.GetValue(ItemSourceDefinitionProperty);
        //}


        ///// <summary>
        ///// Assigns an <see cref="IComboBoxDefinition"/> to a <see cref="System.Windows.Controls.ComboBox"/>
        ///// </summary>
        //public static void SetMyProperty(DependencyObject obj, IComboBoxDefinition value)
        //{
        //    if (obj != null && obj is ItemsControl cmb)
        //    {
        //        if (value is null)
        //        {
        //            UnSetComboBoxProperty(cmb);
        //        }
        //        else
        //        {
        //            SetComboBoxProperty(cmb, value);
        //        }
        //        obj.SetValue(ComboBoxDefinitionProperty, value);
        //    }
        //}

        ///// <summary>
        ///// Assigns an <see cref="IComboBoxDefinition"/> to a <see cref="ComboBox"/>
        ///// </summary>
        //public static readonly DependencyProperty ItemSourceDefinitionProperty =
        //    DependencyProperty.RegisterAttached("ItemSourceDefinition",
        //        typeof(IItemSource),
        //        typeof(DefinitionTargets),
        //        new PropertyMetadata(null)
        //        );

        private static void BindItemSource(ItemsControl obj, IItemSource def)
        {
            BindIControlDefinition(obj, def);

            //ItemSource
            BindingOperations.SetBinding(obj, ItemsControl.ItemsSourceProperty, new Binding("ItemSource")
            {
                Source = def,
                Mode = BindingMode.OneWay
            });
        }

        private static void UnbindItemSource(ItemsControl obj, IItemSource oldDef)
        {
            UnbindIfBound(obj, ItemsControl.ItemsSourceProperty, oldDef);
            UnbindIControlDefinition(obj, oldDef);
        }

        #endregion

        #region < SelectorDefinition Property >

        private static void BindSelectorDefinition(Selector obj, ISelector def)
        {
            BindItemSource(obj, def);

            //Selected Item
            BindingOperations.SetBinding(obj, Selector.SelectedItemProperty, new Binding("SelectedItem")
            {
                Source = def,
                 Mode = BindingMode.TwoWay, 
                IsAsync = false,
            });

            //Selected Value
            BindingOperations.SetBinding(obj, Selector.SelectedValueProperty, new Binding("SelectedValue")
            {
                Source = def
            });

            //Selected Value Path
            BindingOperations.SetBinding(obj, Selector.SelectedValuePathProperty, new Binding("SelectedValuePath")
            {
                Source = def,
                Mode = BindingMode.OneWay
            });
        }

        private static void UnbindSelectorDefinition(Selector obj, ISelector oldDef)
        {
            UnbindIfBound(obj, Selector.SelectedItemProperty, oldDef);
            UnbindIfBound(obj, Selector.SelectedValueProperty, oldDef);
            UnbindIfBound(obj, Selector.SelectedValuePathProperty, oldDef);
            UnbindItemSource(obj, oldDef);
        }

        #endregion

        #region < ComboBoxDefinitionProperty >

        /// <summary>
        /// Assigns an <see cref="IComboBoxDefinition"/> to a <see cref="ComboBox"/>
        /// </summary>
        public static readonly DependencyProperty ComboBoxDefinitionProperty =
            DependencyProperty.RegisterAttached("ComboBoxDefinition",
                typeof(IComboBoxDefinition),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, ComboBoxDefinitionPropertyChanged)
                );

        private static void ComboBoxDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ComboBox cmb)
            {
                if (args.OldValue is IComboBoxDefinition oldDef)
                {
                   UnbindSelectorDefinition(cmb, oldDef);
                }
                if (args.NewValue is IComboBoxDefinition def)
                {
                    BindSelectorDefinition(cmb, def);
                }
            }
        }

        /// <summary>
        /// Gets the assigned <see cref="IComboBoxDefinition"/> from a <see cref="ComboBox"/>
        /// </summary>
        public static IComboBoxDefinition GetComboBoxDefinition(DependencyObject obj)
        {
            return (IComboBoxDefinition)obj.GetValue(ComboBoxDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IComboBoxDefinition"/> to a <see cref="ComboBox"/>
        /// </summary>
        public static void SetComboBoxDefinition(DependencyObject obj, IComboBoxDefinition value)
        {
            obj.SetValue(ComboBoxDefinitionProperty, value);
        }


        #endregion

        #region < ListBoxDefinitionProperty >

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
                    BindingOperations.SetBinding(cntrl, ListBox.SelectionModeProperty, new Binding("SelectionMode")
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

        #endregion

        #region < IButtonDefinitionProperty >

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

                    //Selection Mode
                    BindingOperations.SetBinding(cntrl, Button.CommandProperty, new Binding()
                    {
                        Source = def,
                        Mode = BindingMode.OneWay
                    });
                    if (cntrl.GetBindingExpression(Button.ContentProperty) is null)
                    {
                        BindingOperations.SetBinding(cntrl, Button.ContentProperty, new Binding("ButtonText")
                        {
                            Source = def,
                            Mode = BindingMode.OneWay
                        });
                    }
                }
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

        #endregion

    }
}
