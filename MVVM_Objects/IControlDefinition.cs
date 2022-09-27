using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects
{
    /// <summary>
    /// base interface for control binding definitions
    /// </summary>
    public interface IControlDefinition : IToolTipProvider, INotifyPropertyChanged
    {
        /// <summary>
        /// Flag to set the visibility of the button
        /// </summary>
        System.Windows.Visibility Visibility { get; set; }
    }
}
