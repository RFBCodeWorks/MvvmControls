using RFBCodeWorks.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
//using CommunityToolkit.Mvvm;
//using CommunityToolkit.Mvvm.ComponentModel;

namespace RFBCodeWorks.Mvvm.ComponentModel
{
    /// <summary>
    /// Base class for a ViewModel object that provides a view for a single object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectViewModel<T> : ViewModelBase where T : class
    {
        private readonly IEqualityComparer<T> _equalityComparer;
        private T _model;
        private ObjectCommandFactory _cmdfactory;

        /// <summary>
        /// Create a new ObjectViewModel
        /// </summary>
        public ObjectViewModel() :this(default, default) { }

        /// <summary>
        /// Create a new ObjectViewModel
        /// </summary>
        public ObjectViewModel(T model) : this(model, default) {}

        /// <summary>
        /// Create a new ObjectViewModel
        /// </summary>
        public ObjectViewModel(T model, IEqualityComparer<T> equalityComparer)
        {
            _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
            Model = model;
        }

        
        /// <summary>
        /// Raised when the ObjectModel is updated
        /// </summary>
        public event PropertyChangingEventHandler ModelChanging;


        /// <summary>
        /// Raise the <see cref="ModelChanged"/> event 
        /// </summary>
        protected virtual void OnModelChanging()
        {
            ModelChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Model))); // Raise the ObjectModelChanging event
            if (Model is not null) UnsubscribeFromObjectModel(Model);
        }

        /// <summary>
        /// Override this method to unsubscribe from the object models events. <br/>
        /// This method will occur during <see cref="OnModelChanging()"/> after the event is raised while the old model is still in place.
        /// </summary>
        protected virtual void UnsubscribeFromObjectModel(T model) { }

        /// <inheritdoc/>
        protected override void Dispose_UnsubscribeFromEvents()
        {
            if (Model is not null) UnsubscribeFromObjectModel(Model);
            base.Dispose_UnsubscribeFromEvents();
        }

        #region < ObjectModelChanged Event >

        /// <summary>
        /// Raised when the ObjectModel is updated
        /// </summary>
        public event PropertyChangedEventHandler ModelChanged;


        /// <summary>
        /// Raise the <see cref="ModelChanged"/> event 
        /// </summary>
        protected virtual void OnModelChanged()
        {
            ModelChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Model))); // Raise the ObjectModelChanged event
            //this.OnPropertyChanged(string.Empty); // Use String.Empty here to indicate that all properties have changed
            if (!(Model is null)) SubscribeToObjectModel(Model);
        }

        /// <summary>
        /// Override this method to subscribe to the object models events. <br/>
        /// This method will occur during <see cref="OnModelChanged()"/> before the event is raised, but after the new model is in place.
        /// </summary>
        protected virtual void SubscribeToObjectModel(T model) { }

        #endregion

        /// <summary>
        /// The object this ViewModel represents 
        /// </summary>
        public virtual T Model
        {
            get => _model;
            set {
                if (_equalityComparer.Equals(_model, value) is false)
                {
                    OnModelChanging();
                    base.SetProperty(ref _model, value, string.Empty); // Use String.Empty here to indicate that all properties have changed
                    OnModelChanged();
                }
            }
        }

        /// <summary>
        /// A Factory to create commands associated with this <see cref="ObjectViewModel{T}"/>
        /// </summary>
        protected ObjectCommandFactory CommandFactory {
            get {
                if (_cmdfactory is null) _cmdfactory = new ObjectCommandFactory(this);
                return _cmdfactory;
            }
            set => _cmdfactory = value;
        }

        #region < ObjectCommandFactory >

        /// <summary>
        /// Generate <see cref="System.Windows.Input.ICommand"/>s that rely on the <see cref="Model"/> of an <see cref="ObjectViewModel{T}"/>
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
            public virtual bool DefaultCanExecuteFunction() => !(ViewModel.Model is null);
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
                ViewModel.ModelChanged += cmd.NotifyCanExecuteChanged;
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
                ViewModel.ModelChanged += cmd.NotifyCanExecuteChanged;
                return cmd;
            }

            #endregion

            #region < FromTask >

            /// <summary>
            /// Create a new <see cref="ButtonDefinition"/> that executes an execute that does not require any parameters
            /// </summary>
            /// <inheritdoc cref="ButtonDefinition.ButtonDefinition(Action, Func{bool})" path="*"/>
            public virtual AsyncButtonDefinition FromTask(Func<Task> execute, Func<bool> canExecute = null, string toolTip = null, string displayText = null)
            {
                if (execute is null) throw new ArgumentNullException(nameof(execute));
                var cmd = new AsyncButtonDefinition(
                    execute,
                    canExecute ?? DefaultCanExecuteFunction
                    )
                {
                    DisplayText = displayText,
                    ToolTip = toolTip,
                };
                ViewModel.ModelChanged += cmd.NotifyCanExecuteChanged;
                return cmd;
            }

            /// <summary>
            /// Create a new <see cref="ButtonDefinition"/> that executes an execute that does not require any parameters
            /// </summary>
            /// <inheritdoc cref="ButtonDefinition.ButtonDefinition(Action, Func{bool})" path="*"/>
            public virtual AsyncButtonDefinition FromTask(Func<CancellationToken, Task> execute, Func<bool> canExecute = null, string toolTip = null, string displayText = null)
            {
                if (execute is null) throw new ArgumentNullException(nameof(execute));
                var cmd = new AsyncButtonDefinition(
                    execute,
                    canExecute ?? DefaultCanExecuteFunction
                    )
                {
                    DisplayText = displayText,
                    ToolTip = toolTip,
                };
                ViewModel.ModelChanged += cmd.NotifyCanExecuteChanged;
                return cmd;
            }

            /// <summary>
            /// Create a new <see cref="RelayCommand{O}"/>
            /// </summary>
            /// <inheritdoc cref="ButtonDefinition{T}.ButtonDefinition(Action{T}, Func{T, bool})" path="*"/>
            public virtual AsyncButtonDefinition<O> FromTask<O>(Func<O, CancellationToken, Task> execute, Func<O, bool> canExecute = null, string toolTip = null, string displayText = null)
            {
                if (execute is null) throw new ArgumentNullException(nameof(execute));
                var cmd = new AsyncButtonDefinition<O>(
                    execute,
                    canExecute ?? DefaultCanExecuteFunction
                    )
                {
                    DisplayText = displayText,
                    ToolTip = toolTip,
                };
                ViewModel.ModelChanged += cmd.NotifyCanExecuteChanged;
                return cmd;
            }

            /// <summary>
            /// Create a new <see cref="RelayCommand{O}"/>
            /// </summary>
            /// <inheritdoc cref="ButtonDefinition{T}.ButtonDefinition(Action{T}, Func{T, bool})" path="*"/>
            public virtual AsyncButtonDefinition<O> FromTask<O>(Func<O, Task> execute, Func<O, bool> canExecute = null, string toolTip = null, string displayText = null)
            {
                if (execute is null) throw new ArgumentNullException(nameof(execute));
                var cmd = new AsyncButtonDefinition<O>(
                    execute,
                    canExecute ?? DefaultCanExecuteFunction
                    )
                {
                    DisplayText = displayText,
                    ToolTip = toolTip,
                };
                ViewModel.ModelChanged += cmd.NotifyCanExecuteChanged;
                return cmd;
            }

            #endregion

            #region < FromMethodName >

            /// <summary>
            /// Use reflection to retrieve a parameterless method by its method name, then create an execute to invoke against the <see cref="ObjectViewModel{T}.Model"/>
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

                void Invoker() => m.Invoke(ViewModel.Model, null);
                return Invoker;
            }

            /// <summary>
            /// Use reflection to retrieve a method by its method name, then create an execute to invoke against the <see cref="ObjectViewModel{T}.Model"/>
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

                void Invoker(O obj) => m.Invoke(ViewModel.Model, new object[] { obj });
                return Invoker;
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
                ViewModel.ModelChanged += cmd.NotifyCanExecuteChanged;
                return cmd;
            }

            /// <returns>new <see cref="ButtonDefinition"/></returns>
            /// <inheritdoc cref="CreateActionFromMethodName(string)"/>
            public ButtonDefinition FromMethodName(string methodName, string toolTip = null, string displayText = null)
            {
                return FromMethodName(methodName, Primitives.AbstractCommand.ReturnTrue, toolTip, displayText);
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
                ViewModel.ModelChanged += cmd.NotifyCanExecuteChanged;
                return cmd;
            }

            /// <returns>new <see cref="ButtonDefinition{O}"/></returns>
            /// <inheritdoc cref="CreateActionFromMethodName{O}(string)"/>
            public ButtonDefinition<O> FromMethodName<O>(string methodName, string toolTip = null, string displayText = null)
            {
                return FromMethodName<O>(methodName, Primitives.CommandBase.ReturnTrue, toolTip, displayText);
            }

            #endregion

            #region < FromMethodNameAsync >

            /// <summary>
            /// Use reflection to retrieve a parameterless method by its method name, then create an execute to invoke against the <see cref="ObjectViewModel{T}.Model"/>
            /// </summary>
            /// <param name="methodName">
            /// use the 'nameof()' function to retrieve the method name. Ex: <Br/>
            /// nameof(ViewModel.ObjectModel.MethodToCall)
            /// </param>
            protected Func<Task> CreateTaskFromMethodName(string methodName)
            {
                if (methodName is null) throw new ArgumentNullException(nameof(methodName));
                if (string.IsNullOrWhiteSpace(methodName)) throw new ArgumentException("Method Name must not be empty", nameof(methodName));

                var t = typeof(T);
                var m = t.GetMethod(methodName.Trim(), Array.Empty<Type>());
                if (!(m.ReturnType == typeof(Task) || m.ReturnType.IsSubclassOf(typeof(Task)))) // Check IF NOT a type of task
                {
                    throw new ArgumentException($"The method retrieved by the name of '{m.Name}' does not return a Task.", nameof(methodName));
                }
                async Task Invoker()
                {
                    await (m.Invoke(ViewModel.Model, null) as Task);
                }
                return Invoker;
            }

            /// <summary>
            /// Use reflection to retrieve a method by its method name, then create an execute to invoke against the <see cref="ObjectViewModel{T}.Model"/>
            /// </summary>
            /// <param name="methodName">
            /// use the 'nameof()' function to retrieve the method name. Ex: <Br/>
            /// nameof(ViewModel.ObjectModel.MethodToCall(<typeparamref name="O"/>)
            /// </param>
            protected Func<O,Task> CreateTaskFromMethodName<O>(string methodName)
            {
                if (methodName is null) throw new ArgumentNullException(nameof(methodName));
                if (string.IsNullOrWhiteSpace(methodName)) throw new ArgumentException("Method Name must not be empty", nameof(methodName));

                var t = typeof(T);
                var m = t.GetMethod(methodName, new Type[1] { typeof(O) });
                if (!(m.ReturnType == typeof(Task) || m.ReturnType.IsSubclassOf(typeof(Task)))) // Check IF NOT a type of task
                {
                    throw new ArgumentException($"The method retrieved by the name of '{m.Name}' does not return a Task.", nameof(methodName));
                }
                async Task Invoker(O obj)
                {
                    await (m.Invoke(ViewModel.Model, new object[] { obj }) as Task);
                }
                return Invoker;
            }


            /// <returns>new <see cref="ButtonDefinition"/></returns>
            /// <inheritdoc cref="CreateTaskFromMethodName(string)"/>
            public virtual AsyncButtonDefinition FromMethodNameAsync(string methodName, Func<bool> canExecute, string toolTip = null, string displayText = null)
            {
                var cmd = new AsyncButtonDefinition(
                    this.CreateTaskFromMethodName(methodName),
                    CanBeInvoked(canExecute ?? throw new ArgumentNullException(nameof(canExecute)))
                    )
                {
                    ToolTip = toolTip,
                    DisplayText = displayText
                };
                ViewModel.ModelChanged += (_, _) => cmd?.Command?.NotifyCanExecuteChanged();
                return cmd;
            }

            /// <returns>new <see cref="ButtonDefinition"/></returns>
            /// <inheritdoc cref="CreateTaskFromMethodName(string)"/>
            public AsyncButtonDefinition FromMethodNameAsync(string methodName, string toolTip = null, string displayText = null)
            {
                return FromMethodNameAsync(methodName, Primitives.AbstractCommand.ReturnTrue, toolTip, displayText);
            }

            /// <returns>new <see cref="ButtonDefinition{O}"/></returns>
            /// <inheritdoc cref="CreateTaskFromMethodName{O}(string)"/>
            public virtual AsyncButtonDefinition<O> FromMethodNameAsync<O>(string methodName, Func<O, bool> canExecute, string toolTip = null, string displayText = null)
            {
                var cmd = new AsyncButtonDefinition<O>(
                    this.CreateTaskFromMethodName<O>(methodName),
                    CanBeInvoked(canExecute ?? throw new ArgumentNullException(nameof(canExecute)))
                    )
                {
                    ToolTip = toolTip,
                    DisplayText = displayText
                };
                ViewModel.ModelChanged += (_, _) => cmd?.Command?.NotifyCanExecuteChanged();
                return cmd;
            }

            /// <returns>new <see cref="ButtonDefinition{O}"/></returns>
            /// <inheritdoc cref="CreateTaskFromMethodName{O}(string)"/>
            public AsyncButtonDefinition<O> FromMethodNameAsync<O>(string methodName, string toolTip = null, string displayText = null)
            {
                return FromMethodNameAsync<O>(methodName, Primitives.AbstractCommand<O>.ReturnTrue, toolTip, displayText);
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
                    DefaultAction = execute ?? throw new ArgumentException("execute must not be null", nameof(execute)),
                    AlternateAction = altExecute,
                    DefaultActionCanExecute = canExecute ?? DefaultCanExecuteFunction,
                    AlternateActionCanExecute = altCanExecute ?? DefaultCanExecuteFunction,
                    DefaultDisplayText = defaultText,
                    AlternateDisplayText = altText,
                    DefaultTooltip = defaultToolTip,
                    AlternateToolTip = altToolTip
                };
                ViewModel.ModelChanged += cmd.NotifyCanExecuteChanged;
                return cmd;
            }
        }

        #endregion
    }
}
