using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvvm
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    internal static class INotifySingletons
    {
        public static readonly INotifyArgSet DisplayName = new(nameof(IDisplayTextProvider.DisplayText));
        public static readonly INotifyArgSet ToolTip = new(nameof(IToolTipProvider.ToolTip));
        public static readonly INotifyArgSet ItemSource = new(nameof(RFBCodeWorks.Mvvvm.ISelector.Items));
        public static readonly INotifyArgSet SelectedIndex = new(nameof(RFBCodeWorks.Mvvvm.ISelector.SelectedIndex));
        public static readonly INotifyArgSet SelectedItem = new(nameof(RFBCodeWorks.Mvvvm.ISelector.SelectedItem));
        public static readonly INotifyArgSet SelectedValue = new(nameof(RFBCodeWorks.Mvvvm.ISelector.SelectedValue));
        public static readonly INotifyArgSet IsEnabled = new(nameof(RFBCodeWorks.Mvvvm.ISelector.IsEnabled));
        public static readonly INotifyArgSet IsDefaultState = new(nameof(RFBCodeWorks.Mvvvm.Primitives.AbstractTwoStateButton.IsDefaultState));
        

        public class INotifyArgSet
        {
            public INotifyArgSet(string propertyName)
            {
                PropertyName = propertyName;
                PropertyChangedArgs = new(propertyName);
                PropertyChangingArgs = new(propertyName);
            }

            public readonly string PropertyName;
            public readonly System.ComponentModel.PropertyChangedEventArgs PropertyChangedArgs;
            public readonly System.ComponentModel.PropertyChangingEventArgs PropertyChangingArgs;
            public static implicit operator INotifyArgSet(string propertyName) => new INotifyArgSet(propertyName);
            public static implicit operator System.ComponentModel.PropertyChangedEventArgs(INotifyArgSet  args) => args.PropertyChangedArgs;
            public static implicit operator System.ComponentModel.PropertyChangingEventArgs(INotifyArgSet args) => args.PropertyChangingArgs;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
