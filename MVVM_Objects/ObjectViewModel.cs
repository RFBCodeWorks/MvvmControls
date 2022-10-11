using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
//using Microsoft.Toolkit.Mvvm;
//using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using RFBCodeWorks.MVVMObjects.BaseControlDefinitions;
using RFBCodeWorks.MVVMObjects.ControlInterfaces;

namespace RFBCodeWorks.MVVMObjects
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

        #region < ObjectCommand Class >

        /// <summary>
        /// A variant of the <see cref="ObjectCommand{O}"/> class whose execute does not require a parameter.
        /// </summary>
        /// <remarks> 
        /// This class automatically wraps the execute to allow <see cref="ICommand"/> functionality.
        /// </remarks>
        public class ObjectCommand : ObjectCommand<object>
        {
            /// <summary>
            /// Create a new <see cref="ObjectCommand"/> object, whose Action does not require any parameters.
            /// </summary>
            /// <inheritdoc cref="ObjectCommand{O}.ObjectCommand(ObjectViewModel{T}, Action{O})"/>
            public ObjectCommand(ObjectViewModel<T> viewModel, Action execute) : 
                base(
                    viewModel, 
                    new Action<object>((o) => execute())
                    ) 
            { }

            /// <summary>
            /// Create a new <see cref="ObjectCommand"/> object, whose Action does not require any parameters.
            /// </summary>
            /// <inheritdoc cref="ObjectCommand{O}.ObjectCommand(ObjectViewModel{T}, Action{O}, Func{O, bool})"/>
            public ObjectCommand(ObjectViewModel<T> viewModel, Action execute, Func<bool> canExecute)
                : base(
                      viewModel,
                      new Action<object>(o => execute()),
                      new Func<object, bool>(o => canExecute())
                      )
            { }
        }

        #endregion

        #region < ObjectCommand - Generic Class >

        /// <summary>
        /// An <see cref="ICommand"/> object that relies on <see cref="ObjectModel"/> to determine the result of <see cref="CanExecute(object)"/>
        /// </summary>
        /// <typeparam name="O">The parameter type the execute will accept. If no parameter is required, use <see cref="object"/></typeparam>
        public class ObjectCommand<O> : AbstractButtonDefinition
        {
            #region < Constructors >

            /// <summary>
            /// Create a new ObjectCommand
            /// </summary>
            /// <inheritdoc cref="ObjectCommand{O}.ObjectCommand(ObjectViewModel{T}, Action{O}, Func{O, bool})"/>
            public ObjectCommand(ObjectViewModel<T> viewModel, Action<O> execute)
            {
                ViewModel = viewModel;
                ExecuteAction = execute;
                CanExecuteFunction = null;
                ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }

            /// <summary>
            /// Create a new ObjectCommand
            /// </summary>
            /// <param name="viewModel">The associated ViewModel that houses this ICommand</param>
            /// <param name="execute">The execute to perform against the <see cref="ObjectViewModel{T}.ObjectModel"/></param>
            /// <param name="canExecute">Some Functions that returns TRUE/FALSE if the <see cref="Execute(object)"/> should be callable.</param>
            public ObjectCommand(ObjectViewModel<T> viewModel, Action<O> execute, Func<O, bool> canExecute)
            {
                ViewModel = viewModel;
                ExecuteAction = execute;
                CanExecuteFunction = canExecute;
                ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }

            #endregion

            #region < Properties >

            /// <summary>
            /// The ViewModel this command is associated with
            /// </summary>
            protected ObjectViewModel<T> ViewModel { get; }

            /// <summary>
            /// Action to execute via the <see cref="Execute(object)"/> method. <br/> This can be left null if the <see cref="Execute(object)"/> method is overridden.
            /// </summary>
            protected Action<O> ExecuteAction { get; init; }

            /// <summary>
            /// The function that is called by the base <see cref="CanExecute(object)"/> method.
            /// </summary>
            /// <remarks>
            /// If not specified, default functionality is checking if the ViewModel.ObjectModel reference is null / not null.
            /// </remarks>
            protected Func<O, bool> CanExecuteFunction { get; init; }

            #endregion

            #region < Methods >

            /// <remarks>
            /// Gets queried after <see cref="AbstractButtonDefinition.CanExecuteChanged"/> is raised. <br/>
            /// Default functionality of <see cref="ObjectCommand{O}"/> returns TRUE if the <see cref="ObjectViewModel{T}.ObjectModel"/> is not null 
            /// </remarks>
            /// <inheritdoc cref="ICommand.CanExecute(object)"/>
            public override bool CanExecute(object parameter)
            {
                if (CanExecuteFunction is null) return !(ViewModel.ObjectModel is null); // If a custom execute is not specified, simply evaluate the reference for null
                if (parameter is O | parameter is null)
                    return CanExecuteFunction((O)parameter);
                else
                    throw new ArgumentException($"Invalid object type passed to ICommand object.\n Expected: {typeof(O)}\nReceived: {parameter.GetType()}");
                 
            }


            /// <inheritdoc cref="ICommand.Execute(object)"/>
            /// <remarks>
            /// If the <paramref name="parameter"/> is not of type <typeparamref name="O"/>, then an <see cref="ArgumentException"/> will be thrown. <br/>
            /// Does not throw if parameter is null.
            /// </remarks>
            public override void Execute(object parameter)
            {
                if (parameter is O | parameter is null)
                    ExecuteAction((O)parameter);
                else
                    throw new ArgumentException($"Invalid object type passed to ICommand object.\n Expected: {typeof(O)}\nReceived: {parameter.GetType()}");
            }

            /// <summary>
            /// This method runs when the ViewModel's ObjectModel property is updated. <br/>
            /// Base method only raises <see cref="AbstractButtonDefinition.CanExecuteChanged"/> when the <see cref="ObjectModel"/> is changed -- determine by evaluating <see cref="PropertyChangedEventArgs.PropertyName"/>
            /// </summary>
            protected virtual void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                switch (e.PropertyName)
                {
                    case null:
                    case "":
                    case nameof(ObjectModel):
                        NotifyCanExecuteChanged();
                        break;
                    default:
                        break;
                }
            }

            #endregion

        }

        #endregion

        #region TwoStateObjectCommand 

        /// <inheritdoc cref="TwoStateRelayCommand"/>
        public class TwoStateObjectCommand : ObjectCommand
        {
            /// <inheritdoc cref="TwoStateRelayCommand.TwoStateRelayCommand(Action, Func{bool}, Action, Func{bool})"/>
            public TwoStateObjectCommand(ObjectViewModel<T> viewModel, Action execute) : base(viewModel, execute)
            {
                ButtonDefinition = new();
                //ButtonDefinition.PropertyChanged += (o, e) => OnPropertyChanged(e.PropertyName);
            }

            /// <inheritdoc cref="TwoStateRelayCommand.TwoStateRelayCommand(Action, Func{bool}, Action, Func{bool})"/>
            public TwoStateObjectCommand(ObjectViewModel<T> viewModel, Action execute, Func<bool> canExecute) : base(viewModel, execute, canExecute)
            {
                ButtonDefinition = new();
                //ButtonDefinition.PropertyChanged += (o, e) => OnPropertyChanged(e.PropertyName);
            }

            /// <summary>
            /// <inheritdoc cref="TwoStateButtonTextProvider"/>
            /// </summary>
            public TwoStateButtonTextProvider ButtonDefinition { get; }

            /// <inheritdoc/>
            public override void NotifyCanExecuteChanged()
            {
                base.NotifyCanExecuteChanged();
            }
        }

        #endregion

        #region < ObjectCommandFactory >

        /// <summary>
        /// Generate <see cref="ICommand"/> objects from
        /// </summary>
        protected class ObjectCommandFactory
        {
            

            #region < Constructors & Properties >
            
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

            #endregion

            #region < Methods >

            /// <summary>
            /// If not specified, this method will be set as the <see cref="ObjectCommand{O}.CanExecuteFunction"/>.
            /// </summary>
            /// <returns>TRUE if the ViewModel.ObjectModel is not null. ( FALSE is the reference is null ) </returns>
            public virtual bool DefaultCanExecuteFunction() => !(ViewModel.ObjectModel is null);
            private bool DefaultCanExecuteFunction<O>(O parameter) => DefaultCanExecuteFunction();

            #region < FromAction >

            /// <summary>
            /// Create a new <see cref="ObjectCommand{O}"/> that executes an execute that does not require any parameters
            /// </summary>
            /// <inheritdoc cref="ObjectCommand{O}.ObjectCommand(ObjectViewModel{T}, Action{O}, Func{O, bool})" path="*"/>
            public virtual ObjectCommand FromAction(Action execute, string toolTip = "", Func<bool> canExecute = null)
            {
                if (execute is null) throw new ArgumentNullException(nameof(execute));
                return new ObjectCommand(
                    ViewModel, 
                    execute, 
                    canExecute ?? DefaultCanExecuteFunction
                    ) { ToolTip = toolTip };
            }

            /// <summary>
            /// Create a new <see cref="ObjectCommand{O}"/>
            /// </summary>
            /// <inheritdoc cref="ObjectCommand{O}.ObjectCommand(ObjectViewModel{T}, Action{O}, Func{O, bool})" path="*"/>
            public virtual ObjectCommand<O> FromAction<O>(Action<O> execute)
            {
                if (execute is null) throw new ArgumentNullException(nameof(execute));
                return new ObjectCommand<O>(
                    ViewModel,
                    execute,
                    DefaultCanExecuteFunction<O>
                    )
                { ToolTip = string.Empty};
            }

            /// <summary>
            /// Create a new <see cref="ObjectCommand{O}"/>
            /// </summary>
            /// <inheritdoc cref="ObjectCommand{O}.ObjectCommand(ObjectViewModel{T}, Action{O}, Func{O, bool})" path="*"/>
            public virtual ObjectCommand<O> FromAction<O>(Action<O> execute, string toolTip)
            {
                if (execute is null) throw new ArgumentNullException(nameof(execute));
                return new ObjectCommand<O>(
                    ViewModel,
                    execute,
                    DefaultCanExecuteFunction<O>
                    )
                { ToolTip = toolTip };
            }

            /// <summary>
            /// Create a new <see cref="ObjectCommand{O}"/>
            /// </summary>
            /// <inheritdoc cref="ObjectCommand{O}.ObjectCommand(ObjectViewModel{T}, Action{O}, Func{O, bool})" path="*"/>
            public virtual ObjectCommand<O> FromAction<O>(Action<O> execute, string toolTip, Func<O, bool> canExecute)
            {
                if (execute is null) throw new ArgumentNullException(nameof(execute));
                return new ObjectCommand<O>(
                    ViewModel, 
                    execute, 
                    canExecute ?? DefaultCanExecuteFunction<O>
                    ) { ToolTip = toolTip };
            }

            /// <inheritdoc cref="ObjectCommand{O}.ObjectCommand(ObjectViewModel{T}, Action{O}, Func{O, bool})" path="*"/>
            public virtual ObjectCommand<O> FromAction<O>(Action<O> execute, string toolTip, Func<bool> canExecute)
            {
                if (execute is null) throw new ArgumentNullException(nameof(execute));
                if (canExecute is null)
                    return FromAction<O>(execute, toolTip, DefaultCanExecuteFunction<O>);
                else
                    return new ObjectCommand<O>(
                        ViewModel, 
                        execute, 
                        new Func<O,bool>( O => canExecute())
                        ) { ToolTip = toolTip };
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
            /// nameof(ViewModel.ObjectModel.MethodToCall(O)
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

            
            /// <returns>new <see cref="ObjectCommand"/></returns>
            /// <inheritdoc cref="CreateActionFromMethodName(string)"/>
            /// <inheritdoc cref="FromMethodName{O}(string, string, Func{bool})"/>
            public virtual ObjectCommand FromMethodName(string methodName, string toolTip = "", Func<bool> canExecute = null)
            {
                return new ObjectCommand(
                    this.ViewModel, 
                    this.CreateActionFromMethodName(methodName), 
                    canExecute ?? DefaultCanExecuteFunction
                    ) { ToolTip = toolTip };
            }

            /// <returns>new <see cref="ObjectCommand{O}"/></returns>
            /// <inheritdoc cref="CreateActionFromMethodName{O}(string)"/>
            public virtual ObjectCommand<O> FromMethodName<O>(string methodName)
            {
                return new ObjectCommand<O>(
                    this.ViewModel,
                    this.CreateActionFromMethodName<O>(methodName),
                    DefaultCanExecuteFunction
                    )
                { ToolTip = string.Empty};
            }

            /// <returns>new <see cref="ObjectCommand{O}"/></returns>
            /// <inheritdoc cref="CreateActionFromMethodName{O}(string)"/>
            public virtual ObjectCommand<O> FromMethodName<O>(string methodName, string toolTip)
            {
                return new ObjectCommand<O>(
                    this.ViewModel, 
                    this.CreateActionFromMethodName<O>(methodName), 
                    DefaultCanExecuteFunction
                    ) { ToolTip = toolTip };
            }

            /// <returns>new <see cref="ObjectCommand{O}"/></returns>
            /// <inheritdoc cref="CreateActionFromMethodName{O}(string)"/>
            /// <inheritdoc cref="ObjectCommand{O}.ObjectCommand(ObjectViewModel{T}, Action{O}, Func{O, bool})"/>
            public virtual ObjectCommand<O> FromMethodName<O>(string methodName, string toolTip, Func<bool> canExecute)
            {
                if (canExecute is null) return FromMethodName<O>(methodName, toolTip);
                return new ObjectCommand<O>(
                    this.ViewModel, 
                    this.CreateActionFromMethodName<O>(methodName), 
                    ( canExecute is null ) ? DefaultCanExecuteFunction<O> : new Func<O, bool> ( o => canExecute())
                    ) { ToolTip = toolTip };
            }

            /// <inheritdoc cref="FromMethodName{O}(string, string, Func{bool})"/>
            public virtual ObjectCommand<O> FromMethodName<O>(string methodName, string toolTip, Func<O, bool> canExecute)
            {
                return new ObjectCommand<O>(
                    this.ViewModel, 
                    this.CreateActionFromMethodName<O>(methodName), 
                    canExecute ?? DefaultCanExecuteFunction
                    ) { ToolTip = toolTip };
            }

            #endregion

            #endregion
        }

        #endregion
    }

}
