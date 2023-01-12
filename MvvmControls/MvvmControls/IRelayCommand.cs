using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Extends <see cref="Microsoft.Toolkit.Mvvm.Input.IRelayCommand"/> to add a public event handler that, which can be used to notify CanExecuteChanged subscribers
    /// </summary>
    /// <remarks>
    /// Inherits: 
    /// <br/> - <see cref="Microsoft.Toolkit.Mvvm.Input.IRelayCommand"/>
    /// <br/> - <see cref="ICommand"/>
    /// </remarks>
    public interface IRelayCommand : Microsoft.Toolkit.Mvvm.Input.IRelayCommand, ICommand
    {
        /// <summary>
        /// Event Handler that allows an external event to raise CanExecuteChanged
        /// </summary>
        void NotifyCanExecuteChanged(object sender, EventArgs e);
    }

    /// <summary>
    /// Extends <see cref="Microsoft.Toolkit.Mvvm.Input.IRelayCommand{T}"/> to add a public event handler that, which can be used to notify CanExecuteChanged subscribers
    /// </summary>
    /// <remarks>
    /// Inherits: 
    /// <br/> - <see cref="IRelayCommand"/>
    /// <br/> - <see cref="Microsoft.Toolkit.Mvvm.Input.IRelayCommand{T}"/>
    /// <br/> - <see cref="ICommand"/>
    /// </remarks>
    /// <inheritdoc cref="Microsoft.Toolkit.Mvvm.Input.IRelayCommand{T}"/>
    public interface IRelayCommand<in T> : IRelayCommand, Microsoft.Toolkit.Mvvm.Input.IRelayCommand<T>, ICommand
    {

    }

    /// <summary>
    /// Extends <see cref="Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand"/> to add a public event handler that, which can be used to notify CanExecuteChanged subscribers
    /// </summary>
    /// <remarks>
    /// Inherits: 
    /// <br/> - <see cref="IRelayCommand"/>
    /// <br/> - <see cref="Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand"/>
    /// <br/> - <see cref="ICommand"/>
    /// </remarks>
    public interface IAsyncRelayCommand : Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand, ICommand, IRelayCommand
    {
        /// <summary>
        /// Gets all of the tasks that are currently executing
        /// </summary>
        IEnumerable<Task> RunningTasks { get; }
    }

    /// <summary>
    /// Extends <see cref="Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand{T}"/> to add a public event handler that, which can be used to notify CanExecuteChanged subscribers
    /// </summary>
    /// <remarks>
    /// Inherits: 
    /// <br/> - <see cref="IAsyncRelayCommand"/>
    /// <br/> - <see cref="IRelayCommand{T}"/>
    /// <br/> - <see cref="Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand{T}"/>
    /// <br/> - <see cref="ICommand"/>
    /// </remarks>
    public interface IAsyncRelayCommand<in T> : IAsyncRelayCommand, Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand<T>, ICommand, IRelayCommand<T>
    {
        
    }


}
