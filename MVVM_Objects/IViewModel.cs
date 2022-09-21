using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects
{
    /// <summary>
    /// Interface implemented by the <see cref="ViewModelBase"/> class
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Reference to the ViewModel that owns this viewmodel.
        /// </summary>
        /// <remarks>
        /// This would be used by a DialogService to locate the viewmodel registered to the window's DataContext.
        /// </remarks>
        IViewModel ParentViewModel { get; }
    }
}
