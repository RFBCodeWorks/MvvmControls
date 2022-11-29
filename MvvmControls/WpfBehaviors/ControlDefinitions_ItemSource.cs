using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace RFBCodeWorks.WPFBehaviors
{
    public static partial class ControlDefinitions
    {
        private static void BindItemSource(ItemsControl obj, IItemSource def)
        {
            BindIControlDefinition(obj, def);

            //ItemSource
            SetBinding(obj, ItemsControl.ItemsSourceProperty, nameof(IItemSource.Items), def, BindingMode.OneWay);

            if (obj.GetValue(ItemsControl.ItemTemplateProperty) == null)
            {
                SetBinding(obj, ItemsControl.DisplayMemberPathProperty, nameof(IItemSource.DisplayMemberPath), def, BindingMode.OneWay);
            }
        }

        private static void UnbindItemSource(ItemsControl obj, IItemSource oldDef)
        {
            UnbindIfBound(obj, ItemsControl.ItemsSourceProperty, oldDef);
            UnbindIfBound(obj, ItemsControl.DisplayMemberPathProperty, oldDef);
            UnbindIControlDefinition(obj, oldDef);
        }
    }
}
