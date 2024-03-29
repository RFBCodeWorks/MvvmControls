﻿using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvm.Primitives
{
    /// <summary>
    /// Abstract base object that inherits from the following:
    /// <br/> - <see cref="ControlBase"/>
    /// <br/> - <see cref="IRelayCommand"/>
    /// <br/> - <see cref="ICommand"/> - Explicitly implemented. <see cref="ICommand.Execute(object)"/> will execute synchronously unless derived class overrides the ICommand.Execute(object) implementation.
    /// <br/> - <see cref="IToolTipProvider"/>
    /// <br/> - <see cref="INotifyPropertyChanged"/>
    /// <br/> - <see cref="IButtonDefinition"/>
    /// </summary>
    public abstract class AbstractAsyncButtonDefinition : ControlBase, IButtonDefinition, ICommand
    {
        /// <inheritdoc/>
        public virtual string DisplayText
        {
            get { return ButtonTextField; }
            set { SetProperty(ref ButtonTextField, value, nameof(DisplayText)); }
        }
        private string ButtonTextField;

        /// <summary>
        /// Returns the result of the last <see cref="ICommand.CanExecute(object)"/> call.
        /// <br/> The set method has no effect.
        /// </summary>
        public override bool IsEnabled { get => base.IsEnabled; set { } }

        /// <inheritdoc cref="CommunityToolkit.Mvvm.Input.RelayCommand{T}.CanExecute(T)"/>
        public abstract bool CanExecute();

        /// <summary> The method through which the abstract base object implements <see cref="ICommand.Execute(object)"/> </summary>
        public abstract Task ExecuteAsync();

        /// <summary> Cancel the task </summary>
        public abstract void Cancel();

        /// <inheritdoc/>
        public abstract void NotifyCanExecuteChanged();

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged(object sender, EventArgs e) => this.NotifyCanExecuteChanged();

        #region < ICommand Explicit Implementation >

        /// <inheritdoc/>
        public abstract event EventHandler CanExecuteChanged;

        void ICommand.Execute(object parameter) => this.ExecuteAsync().Wait();
        bool ICommand.CanExecute(object parameter)
        {
            base.IsEnabled = CanExecute();
            return base.IsEnabled;
        }
        
        #endregion
    }

}
