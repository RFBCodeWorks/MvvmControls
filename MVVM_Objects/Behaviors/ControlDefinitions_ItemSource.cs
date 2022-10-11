﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using RFBCodeWorks.MVVMObjects.ControlInterfaces;

namespace RFBCodeWorks.MVVMObjects.Behaviors
{
    public static partial class ControlDefinitions
    {
        private static void BindItemSource(ItemsControl obj, IItemSource def)
        {
            BindIControlDefinition(obj, def);

            //ItemSource
            SetBinding(obj, ItemsControl.ItemsSourceProperty, nameof(IItemSource.ItemSource), def, BindingMode.OneWay);
            SetBinding(obj, ItemsControl.DisplayMemberPathProperty, nameof(IItemSource.DisplayMemberPath), def, BindingMode.OneWay);
        }

        private static void UnbindItemSource(ItemsControl obj, IItemSource oldDef)
        {
            UnbindIfBound(obj, ItemsControl.ItemsSourceProperty, oldDef);
            UnbindIfBound(obj, ItemsControl.DisplayMemberPathProperty, oldDef);
            UnbindIControlDefinition(obj, oldDef);
        }

        private static void BindSelectorDefinition(Selector obj, ISelector def)
        {
            BindItemSource(obj, def);

            //Selected Value Path
            SetBinding(obj, Selector.SelectedValuePathProperty, nameof(ISelector.SelectedValuePath), def, BindingMode.OneWay);

            //Selected Value
            SetBinding(obj, Selector.SelectedValueProperty, nameof(ISelector.SelectedValue), def, BindingMode.TwoWay);

            //Selected Value
            SetBinding(obj, Selector.SelectedItemProperty, nameof(ISelector.SelectedItem), def, BindingMode.TwoWay);
        }

        private static void UnbindSelectorDefinition(Selector obj, ISelector oldDef)
        {
            UnbindIfBound(obj, Selector.SelectedItemProperty, oldDef);
            UnbindIfBound(obj, Selector.SelectedValueProperty, oldDef);
            UnbindIfBound(obj, Selector.SelectedValuePathProperty, oldDef);
            UnbindItemSource(obj, oldDef);
        }
    }
}
