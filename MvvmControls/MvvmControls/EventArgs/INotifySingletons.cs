using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MvvmControls
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    internal static class INotifySingletons
    {
        public static readonly INotifyArgSet DisplayName = new(nameof(IDisplayTextProvider.DisplayText));
        public static readonly INotifyArgSet ToolTip = new(nameof(IToolTipProvider.ToolTip));
        public static readonly INotifyArgSet ItemSource = new(nameof(RFBCodeWorks.MvvmControls.ISelector.Items));
        public static readonly INotifyArgSet SelectedIndex = new(nameof(RFBCodeWorks.MvvmControls.ISelector.SelectedIndex));
        public static readonly INotifyArgSet SelectedItem = new(nameof(RFBCodeWorks.MvvmControls.ISelector.SelectedItem));
        public static readonly INotifyArgSet SelectedValue = new(nameof(RFBCodeWorks.MvvmControls.ISelector.SelectedValue));
        public static readonly INotifyArgSet IsEnabled = new(nameof(RFBCodeWorks.MvvmControls.ISelector.IsEnabled));
        public static readonly INotifyArgSet IsDefaultState = new(nameof(RFBCodeWorks.MvvmControls.Primitives.AbstractTwoStateButton.IsDefaultState));
        

        public class INotifyArgSet
        {
            public INotifyArgSet(string propertyName)
            {
                ChangedArgs = new(propertyName);
                ChangingArgs = new(propertyName);
            }
            
            readonly System.ComponentModel.PropertyChangedEventArgs ChangedArgs;
            readonly System.ComponentModel.PropertyChangingEventArgs ChangingArgs;
            public static implicit operator INotifyArgSet(string propertyName) => new INotifyArgSet(propertyName);
            public static implicit operator System.ComponentModel.PropertyChangedEventArgs(INotifyArgSet  args) => args.ChangedArgs;
            public static implicit operator System.ComponentModel.PropertyChangingEventArgs(INotifyArgSet args) => args.ChangingArgs;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
