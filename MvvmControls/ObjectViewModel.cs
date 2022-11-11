using System;
using System.ComponentModel;
using System.Windows.Input;
//using Microsoft.Toolkit.Mvvm;
//using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Base class for a ViewModel object that provides a view for a single object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectViewModel<T> : ViewModelBase where T : class
    {
        #region < ObjectViewModel Class >
        #region < Constructors >

        /// <summary>
        /// Create a new ObjectViewModel
        /// </summary>
        public ObjectViewModel() { }

        /// <summary>
        /// Create a new ObjectViewModel
        /// </summary>
        public ObjectViewModel(T model)
        {
            ObjectModel = model;
        }

        #endregion

        #region < ObjectModelChanging Event >


        /// <summary>
        /// Raised when the ObjectModel is updated
        /// </summary>
        public event PropertyChangingEventHandler ObjectModelChanging;


        /// <summary>
        /// Raise the <see cref="ObjectModelChanged"/> event 
        /// </summary>
        protected virtual void OnObjectModelChanging()
        {
            ObjectModelChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(ObjectModel))); // Raise the ObjectModelChanging event
            if (!(ObjectModel is null)) UnsubscribeFromObjectModel(ObjectModel);
        }

        /// <summary>
        /// Override this method to unsubscribe from the object models events. <br/>
        /// This method will occur during <see cref="OnObjectModelChanging()"/> after the event is raised while the old model is still in place.
        /// </summary>
        protected virtual void UnsubscribeFromObjectModel(T model) { }

        /// <inheritdoc/>
        protected override void Dispose_UnsubscribeFromEvents()
        {
            if (!(ObjectModel is null)) UnsubscribeFromObjectModel(ObjectModel);
            base.Dispose_UnsubscribeFromEvents();
        }

        #endregion

        #region < ObjectModelChanged Event >

        /// <summary>
        /// Raised when the ObjectModel is updated
        /// </summary>
        public event PropertyChangedEventHandler ObjectModelChanged;


        /// <summary>
        /// Raise the <see cref="ObjectModelChanged"/> event 
        /// </summary>
        protected virtual void OnObjectModelChanged()
        {
            ObjectModelChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ObjectModel))); // Raise the ObjectModelChanged event
            //this.OnPropertyChanged(string.Empty); // Use String.Empty here to indicate that all properties have changed
            if (!(ObjectModel is null)) SubscribeToObjectModel(ObjectModel);
        }

        /// <summary>
        /// Override this method to subscribe to the object models events. <br/>
        /// This method will occur during <see cref="OnObjectModelChanged()"/> before the event is raised, but after the new model is in place.
        /// </summary>
        protected virtual void SubscribeToObjectModel(T model) { }

        #endregion

        #region < Properties >

        /// <summary>
        /// The object this ViewModel represents 
        /// </summary>
        public virtual T ObjectModel {
            get => model;
            set {
                OnObjectModelChanging();
                base.SetProperty(ref model, value, string.Empty); // Use String.Empty here to indicate that all properties have changed
                OnObjectModelChanged();
            }
        }
        private T model;

        /// <summary>
        /// A Factory to create commands associated with this <see cref="ObjectViewModel{T}"/>
        /// </summary>
        protected ObjectCommandFactory CommandFactory {
            get {
                if (cmdfactory is null) cmdfactory = new ObjectCommandFactory(this);
                return cmdfactory;
            }
            set => cmdfactory = value;
        }
        private ObjectCommandFactory cmdfactory;

        #endregion
        #endregion

        #region < ObjectCommandFactory >

        /// <summary>
        /// Generate <see cref="ICommand"/>s that rely on the <see cref="ObjectModel"/> of an <see cref="ObjectViewModel{T}"/>
        /// </summary>
        protected class ObjectCommandFactory
        {
            /// <summary>
            /// Create the Factory
            /// </summary>
            /// <param name="viewModel"></param>
            public ObjectCommandFactory(ObjectViewModel<T> viewModel)
            {
                ViewModel = viewModel;
            }

            /// <summary>
            /// The associated <see cref="ObjectViewModel{T}"/>
            /// </summary>
            public ObjectViewModel<T> ViewModel { get; }

            /// <summary>
            /// If not specified, the CanExecute functions will utilize this this function
            /// </summary>
            /// <returns>TRUE if the ViewModel.ObjectModel is not null. ( FALSE is the reference is null ) </returns>
            public virtual bool DefaultCanExecuteFunction() => !(ViewModel.ObjectModel is null);
            private bool DefaultCanExecuteFunction<O>(O parameter) => DefaultCanExecuteFunction();

            private Func<bool> CanBeInvoked(Func<bool> canExecute) => () => DefaultCanExecuteFunction() && canExecute();
            private Func<O,bool> CanBeInvoked<O>(Func<O,bool> canExecute) => (o) => DefaultCanExecuteFunction() && canExecute(o);

            #region < FromAction >

            /// <summary>
            /// Create a new <see cref="ButtonDefinition"/> that executes an execute that does not require any parameters
            /// </summary>
            /// <inheritdoc cref="ButtonDefinition.ButtonDefinition(Action, Func{bool})" path="*"/>
            public virtual ButtonDefinition FromAction(Action execute, Func<bool> canExecute = null, string toolTip = null, string displayText = null)
            {
                if (execute is null) throw new ArgumentNullException(nameof(execute));
                var cmd = new ButtonDefinition(
                    execute,
                    canExecute ?? DefaultCanExecuteFunction
                    )
                {
                    DisplayText = displayText,
                    ToolTip = toolTip,
                };
                ViewModel.ObjectModelChanged += cmd.Command.NotifyCanExecuteChanged;
                return cmd;
            }

            /// <summary>
            /// Create a new <see cref="RelayCommand{O}"/>
            /// </summary>
            /// <inheritdoc cref="ButtonDefinition{T}.ButtonDefinition(Action{T}, Func{T, bool})" path="*"/>
            public virtual ButtonDefinition<O> FromAction<O>(Action<O> execute, Func<O, bool> canExecute = null, string toolTip = null, string displayText = null)
            {
                if (execute is null) throw new ArgumentNullException(nameof(execute));
                var cmd = new ButtonDefinition<O>(
                    execute,
                    canExecute ?? DefaultCanExecuteFunction
                    )
                {
                    DisplayText = displayText,
                    ToolTip = toolTip,
                };
                ViewModel.ObjectModelChanged += cmd.Command.NotifyCanExecuteChanged;
                return cmd;
            }

            #endregion

            #region < FromMethodName >

            /// <summary>
            /// Use reflection to retrieve a parameterless method by its method name, then create an execute to invoke against the <see cref="ObjectViewModel{T}.ObjectModel"/>
            /// </summary>
            /// <param name="methodName">
            /// use the 'nameof()' function to retrieve the method name. Ex: <Br/>
            /// nameof(ViewModel.ObjectModel.MethodToCall)
            /// </param>
            protected Action CreateActionFromMethodName(string methodName)
            {
                if (methodName is null) throw new ArgumentNullException(nameof(methodName));
                if (string.IsNullOrWhiteSpace(methodName)) throw new ArgumentException("Method Name must not be empty", nameof(methodName));

                var t = typeof(T);
                var m = t.GetMethod(methodName.Trim(), Array.Empty<Type>());

                return new Action(() =>
                {
                    m.Invoke(ViewModel.ObjectModel, null);
                });
            }

            /// <summary>
            /// Use reflection to retrieve a method by its method name, then create an execute to invoke against the <see cref="ObjectViewModel{T}.ObjectModel"/>
            /// </summary>
            /// <param name="methodName">
            /// use the 'nameof()' function to retrieve the method name. Ex: <Br/>
            /// nameof(ViewModel.ObjectModel.MethodToCall(<typeparamref name="O"/>)
            /// </param>
            protected Action<O> CreateActionFromMethodName<O>(string methodName)
            {
                if (methodName is null) throw new ArgumentNullException(nameof(methodName));
                if (string.IsNullOrWhiteSpace(methodName)) throw new ArgumentException("Method Name must not be empty", nameof(methodName));

                var t = typeof(T);
                var m = t.GetMethod(methodName, new Type[1] { typeof(O) });

                return new Action<O>((o) =>
                {
                    m.Invoke(ViewModel.ObjectModel, new object[] { o });
                });
            }


            /// <returns>new <see cref="ButtonDefinition"/></returns>
            /// <inheritdoc cref="CreateActionFromMethodName(string)"/>
            public virtual ButtonDefinition FromMethodName(string methodName, Func<bool> canExecute, string toolTip = null, string displayText = null)
            {
                var cmd = new ButtonDefinition(
                    this.CreateActionFromMethodName(methodName),
                    CanBeInvoked(canExecute ?? throw new ArgumentNullException(nameof(canExecute)))
                    )
                {
                    ToolTip = toolTip,
                    DisplayText = displayText
                };
                ViewModel.ObjectModelChanged += cmd.Command.NotifyCanExecuteChanged;
                return cmd;
            }

            /// <returns>new <see cref="ButtonDefinition"/></returns>
            /// <inheritdoc cref="CreateActionFromMethodName(string)"/>
            public ButtonDefinition FromMethodName(string methodName, string toolTip = null, string displayText = null)
            {
                return FromMethodName(methodName, AbstractCommandBase.ReturnTrue, toolTip, displayText);
            }

            /// <returns>new <see cref="ButtonDefinition{O}"/></returns>
            /// <inheritdoc cref="CreateActionFromMethodName{O}(string)"/>
            public virtual ButtonDefinition<O> FromMethodName<O>(string methodName, Func<O, bool> canExecute, string toolTip = null, string displayText = null)
            {
                var cmd = new ButtonDefinition<O>(
                    this.CreateActionFromMethodName<O>(methodName),
                    CanBeInvoked(canExecute ?? throw new ArgumentNullException(nameof(canExecute)))
                    )
                {
                    ToolTip = toolTip,
                    DisplayText = displayText
                };
                ViewModel.ObjectModelChanged += cmd.Command.NotifyCanExecuteChanged;
                return cmd;
            }

            /// <returns>new <see cref="ButtonDefinition{O}"/></returns>
            /// <inheritdoc cref="CreateActionFromMethodName{O}(string)"/>
            public ButtonDefinition<O> FromMethodName<O>(string methodName, string toolTip = null, string displayText = null)
            {
                return FromMethodName<O>(methodName, AbstractCommandBase.ReturnTrue, toolTip, displayText);
            }

            #endregion
            
            /// <summary>
            /// Create a Two-State button
            /// </summary>
            /// <param name="execute"></param>
            /// <param name="altExecute"></param>
            /// <param name="canExecute"></param>
            /// <param name="altCanExecute"></param>
            /// <param name="defaultText"></param>
            /// <param name="altText"></param>
            /// <param name="defaultToolTip"></param>
            /// <param name="altToolTip"></param>
            /// <returns></returns>
            public Specialized.TwoStateButton TwoStateButton(Action execute, Action altExecute, Func<bool> canExecute, Func<bool> altCanExecute, string defaultText, string altText, string defaultToolTip =null, string altToolTip =null)
            {
                var cmd = new Specialized.TwoStateButton()
                {
                    DefaultAction = execute ?? throw new ArgumentException(nameof(execute)),
                    AlternateAction = altExecute,
                    DefaultActionCanExecute = canExecute ?? DefaultCanExecuteFunction,
                    AlternateActionCanExecute = altCanExecute ?? DefaultCanExecuteFunction,
                    DefaultDisplayText = defaultText,
                    AlternateDisplayText = altText,
                    DefaultTooltip = defaultToolTip,
                    AlternateToolTip = altToolTip
                };
                ViewModel.ObjectModelChanged += cmd.NotifyCanExecuteChanged;
                return cmd;
            }
        }

        #endregion
    }
}
