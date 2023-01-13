using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvvm
{
    internal static class EventArgSingletons
    {
        public static readonly INotifyArgs DisplayName = new(nameof(IDisplayTextProvider.DisplayText));
        public static readonly INotifyArgs ToolTip = new(nameof(IToolTipProvider.ToolTip));

        public static readonly INotifyArgs IsVisible = new(nameof(IControlDefinition.IsVisible));
        public static readonly INotifyArgs Visibility = new(nameof(IControlDefinition.Visibility));
        internal static readonly INotifyArgs HiddenMode = new(nameof(Primitives.ControlBase.HiddenMode));

        public static readonly INotifyArgs SelectedIndex = new(nameof(RFBCodeWorks.Mvvvm.ISelector.SelectedIndex));
        public static readonly INotifyArgs SelectedItem = new(nameof(RFBCodeWorks.Mvvvm.ISelector.SelectedItem));
        public static readonly INotifyArgs SelectedValue = new(nameof(RFBCodeWorks.Mvvvm.ISelector.SelectedValue));
        public static readonly INotifyArgs IsEnabled = new(nameof(RFBCodeWorks.Mvvvm.ISelector.IsEnabled));
        public static readonly INotifyArgs IsDefaultState = new(nameof(RFBCodeWorks.Mvvvm.Primitives.AbstractTwoStateButton.IsDefaultState));

        public static readonly INotifyArgs ItemSourceItems = new(nameof(IItemSource.Items));
        public static readonly INotifyArgs DisplayMemberPath = new(nameof(IItemSource.DisplayMemberPath));

    }
}
