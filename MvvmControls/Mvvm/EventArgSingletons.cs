using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MvvmControls.Primitives
{
    internal static class EventArgSingletons
    {
        #region < Event Arg Singletons >

        public static readonly PropertyChangingEventArgs ItemSourceItemsChanging = new(nameof(IItemSource.Items));
        public static readonly PropertyChangedEventArgs ItemSourceItemsChanged = new(nameof(IItemSource.Items));

        public static readonly PropertyChangingEventArgs ItemSourceDisplayMemberChanging = new(nameof(IItemSource.DisplayMemberPath));
        public static readonly PropertyChangedEventArgs ItemSourceDisplayMemberChanged = new(nameof(IItemSource.DisplayMemberPath));

        public static readonly PropertyChangingEventArgs SelectedIndexChanging = new(nameof(ISelector.SelectedIndex));
        public static readonly PropertyChangedEventArgs SelectedIndexChanged = new(nameof(ISelector.SelectedIndex));

        #endregion
    }
}
