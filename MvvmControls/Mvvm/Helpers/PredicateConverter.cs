using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvvm.Helpers
{
    /// <summary>
    /// Implicitly convert between a <see cref="Predicate{T}"/> and a <see cref="Func{T, R}"/> where TResult is <see langword="bool"/>
    /// </summary>
    /// <typeparam name="T">The type of object to evaluate</typeparam>
    public class PredicateConverter<T>
    {
        /// <summary>
        /// Implicitly create a new PredicateConverter
        /// </summary>
        /// <param name="pred">The Predicate to convert to a Func{T,bool}</param>
        public PredicateConverter(Predicate<T> pred)
        {
            Pred = pred;
        }

        /// <summary>
        /// Implicitly create a new PredicateConverter
        /// </summary>
        /// <param name="func">The func to convert to a Predicate{T}</param>
        public PredicateConverter(Func<T,bool> func)
        {
            Func = func;
        }

        private readonly Predicate<T> Pred;
        private readonly Func<T, bool> Func;

        /// <summary>
        /// Evaluate the <paramref name="parameter"/>
        /// </summary>
        /// <returns>The result of the evalution method passed into the constructor</returns>
        public bool Invoke(T parameter)
        {
            if (Pred is null)
                return Func(parameter);
            else
                return Pred(parameter);
        }

        /// <inheritdoc cref="PredicateConverter{T}.PredicateConverter(Predicate{T})"/>
        public static implicit operator PredicateConverter<T>(Predicate<T> pred) => new(pred);
        
        /// <inheritdoc cref="PredicateConverter{T}.PredicateConverter(Func{T, bool})"/>
        public static implicit operator PredicateConverter<T>(Func<T, bool> func) => new(func);

        /// <summary>
        /// Implicitly return the Func{T,bool}
        /// </summary>
        public static implicit operator Func<T, bool>(PredicateConverter<T> converter) => converter.Func is not null ? converter.Func : converter.Invoke;

        /// <summary>
        /// Implicitly return the Predicate{T}
        /// </summary>
        public static implicit operator Predicate<T>(PredicateConverter<T> converter) => converter.Pred is not null ? converter.Pred: converter.Invoke;
    }
}
