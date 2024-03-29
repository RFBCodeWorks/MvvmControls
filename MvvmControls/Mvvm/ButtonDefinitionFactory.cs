﻿using System;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Factory for creating <see cref="ButtonDefinition"/> objects
    /// </summary>
    public static class ButtonDefinitionFactory
    {
        /// <returns>TRUE if the ViewModel.ObjectModel is not null. ( FALSE is the reference is null ) </returns>
        private static bool DefaultCanExecuteFunction() => true;
        private static bool DefaultCanExecuteFunction<T>(T parameter) => DefaultCanExecuteFunction();

        #region < FromAction >

        /// <summary>
        /// Create a new <see cref="ButtonDefinition"/> that executes an execute that does not require any parameters
        /// </summary>
        /// <inheritdoc cref="ButtonDefinition.ButtonDefinition(Action)" path="*"/>
        public static ButtonDefinition CreateCommand(Action execute, string toolTip = "")
        {
            if (execute is null) throw new ArgumentNullException(nameof(execute));
            return new ButtonDefinition(
                execute, 
                DefaultCanExecuteFunction
                ) { ToolTip = toolTip };
        }

        /// <inheritdoc cref="ButtonDefinition{T}.ButtonDefinition(Action{T}, Func{T, bool})" path="*"/>
        public static ButtonDefinition CreateCommand(Action execute, string toolTip, Func<bool> canExecute)
        {
            if (execute is null) throw new ArgumentNullException(nameof(execute));
            return new ButtonDefinition(
                execute, 
                canExecute ?? DefaultCanExecuteFunction
                ) { ToolTip = toolTip };
        }

        /// <summary>
        /// Create a new <see cref="ButtonDefinition{T}"/>
        /// </summary>
        /// <inheritdoc cref="ButtonDefinition{T}.ButtonDefinition(Action{T})" path="*"/>
        public static ButtonDefinition<T> CreateCommand<T>(Action<T> execute, string toolTip = "")
        {
            if (execute is null) throw new ArgumentNullException(nameof(execute));
            return new ButtonDefinition<T>(execute) { ToolTip = toolTip };
        }

        /// <inheritdoc cref="ButtonDefinition{T}.ButtonDefinition(Action{T}, Func{T, bool})" path="*"/>
        public static ButtonDefinition<T> CreateCommand<T>(Action<T> execute, string toolTip, Func<T, bool> canExecute)
        {
            if (execute is null) throw new ArgumentNullException(nameof(execute));
            return new ButtonDefinition<T>(
                execute, 
                canExecute ?? DefaultCanExecuteFunction
                ) { ToolTip = toolTip };
        }

        /// <inheritdoc cref="ButtonDefinition{T}.ButtonDefinition(Action{T}, Func{T, bool})" path="*"/>
        public static ButtonDefinition<T> CreateCommand<T>(Action<T> execute, string toolTip, Func<bool> canExecute)
        {
            if (execute is null) throw new ArgumentNullException(nameof(execute));
            if (canExecute is null)
                return CreateCommand<T>(execute, toolTip);
            else
                return new ButtonDefinition<T>(
                    execute, 
                    new Func<T, bool>(T => canExecute())
                    ) { ToolTip = toolTip };
        }

        #endregion

        #region < Create TwoState Command >

        /// <summary>
        /// Create a new TwoStateButtonDefinition that performs the same action in both states, but the displayed text changes
        /// </summary>
        /// <param name="action">action to perform in both states</param>
        /// <param name="canExecute">Function that determines if the action can execute in either state</param>
        /// <inheritdoc cref="CreateTwoStateButtonDefinition(Action, Action, string, string, Func{bool}, Func{bool})"/>
        /// <param name="defaultText"/> <param name="alternateText"/>
        public static Specialized.TwoStateButton CreateTwoStateButtonDefinition(Action action, string defaultText = "", string alternateText = "", Func<bool> canExecute = default)
        {
            return CreateTwoStateButtonDefinition(action, action, defaultText, alternateText, canExecute, canExecute);
        }

        /// <summary>
        /// Create a new TwoStateRelay Command that has two different actions it can perform
        /// </summary>
        /// <param name="defaultAction">default action to perform</param>
        /// <param name="alternateAction">alternate action to perform</param>
        /// <param name="defaultCanExecute">function to determine if the <paramref name="defaultAction"/> can execute</param>
        /// <param name="defaultText">Button Text for the default state</param>
        /// <param name="alternateText">Button Text for the alternate state</param>
        /// <param name="alternateCanExecute">function to determine if the <paramref name="alternateAction"/> can execute</param>
        /// <returns></returns>
        public static Specialized.TwoStateButton CreateTwoStateButtonDefinition(Action defaultAction, Action alternateAction, string defaultText = "", string alternateText = "", Func<bool> defaultCanExecute = default, Func<bool> alternateCanExecute = default)
        {
            if (defaultAction is null) throw new ArgumentNullException(nameof(defaultAction));
            if (alternateAction is null) throw new ArgumentNullException(nameof(alternateAction));

            static bool Enabled() => true;
            return new Specialized.TwoStateButton(
                defaultAction,
                defaultCanExecute ?? Enabled,
                alternateAction,
                alternateCanExecute ?? Enabled
                )
            {
                DefaultDisplayText = defaultText,
                AlternateDisplayText = alternateText,
            };
        }

        #endregion

        #region < ObjectInvokesMethodName >

        #region Create Action/Func

        /// <summary>
        /// Create a reference to some parameterless method that the <typeparamref name="T"/> object can execute.
        /// </summary>
        /// <param name="methodName">
        /// use the 'nameof()' function to retrieve the method name. Ex: <Br/>
        /// nameof(ViewModel.ObjectModel.MethodToCall(T)
        /// </param>
        /// <typeparam name="T">The object type to invoke the method against</typeparam>
        private static Action<T> CreateParameterlessActionFromObjectMethod<T>(string methodName)
        {
            if (methodName is null) throw new ArgumentNullException(nameof(methodName));
            if (string.IsNullOrWhiteSpace(methodName)) throw new ArgumentException("Method Name must not be empty", nameof(methodName));

            var t = typeof(T);
            var m = t.GetMethod(methodName, new Type[1] { typeof(T) });

            return new Action<T>((o) =>
            {
                m.Invoke(o,null);
            });
        }

        /// <summary>
        /// Anonymous method variant that calls <see cref="CreateParameterlessActionFromObjectMethod{T}(string)"/>, but gets <typeparamref name="T"/> from the supplied <paramref name="objectReference"/>
        /// </summary>
        private static Action<T> CreateParameterlessActionFromObjectMethod<T>(string methodName, T objectReference) => CreateParameterlessActionFromObjectMethod<T>(methodName);


        /// <summary>
        /// Default CanExecute method for the <see cref="ObjectInvokesMethodName{T}(string, string)"/> factory methods
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectReference"></param>
        /// <returns>FALSE if the object is null, TRUE if not null</returns>
        private static bool EvaluateNullObject<T>(T objectReference) => !(objectReference is null);

        #endregion

        /// <returns>new <see cref="ButtonDefinition{T}"/></returns>
        /// <inheritdoc cref="CreateParameterlessActionFromObjectMethod{T}(string)"/>
        public static ButtonDefinition<T> ObjectInvokesMethodName<T>(string methodName, string toolTip = "")
        {
            
            return new ButtonDefinition<T>(
                CreateParameterlessActionFromObjectMethod<T>(methodName), 
                EvaluateNullObject
                ) { ToolTip = toolTip };
        }

        /// <returns>new <see cref="ButtonDefinition{T}"/></returns>
        /// <inheritdoc cref="CreateParameterlessActionFromObjectMethod{T}(string)"/>
        /// <inheritdoc cref="ButtonDefinition{T}.ButtonDefinition(Action{T}, Func{T, bool})"/>
        public static ButtonDefinition<T> ObjectInvokesMethodName<T>(string methodName, string toolTip, Func<bool> canExecute)
        {
            if (canExecute is null) return ObjectInvokesMethodName<T>(methodName);
            return new ButtonDefinition<T>(
                CreateParameterlessActionFromObjectMethod<T>(methodName), 
                new Func<T, bool>(o => canExecute())
                ) { ToolTip = toolTip };
        }

        /// <inheritdoc cref="ObjectInvokesMethodName{T}(string, string, Func{bool})"/>
        public static ButtonDefinition<T> ObjectInvokesMethodName<T>(string methodName, string toolTip, Func<T, bool> canExecute)
        {
            return new ButtonDefinition<T>(
                CreateParameterlessActionFromObjectMethod<T>(methodName), 
                canExecute ?? EvaluateNullObject
                ) { ToolTip = toolTip };
        }

        #endregion

    }
}
