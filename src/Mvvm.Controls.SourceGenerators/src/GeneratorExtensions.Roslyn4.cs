#if ROSLYN_4_0_OR_GREATER

using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm.SourceGenerators
{
    internal static partial class GeneratorExtensions
    {
        /// <summary>
        /// Registers a diagnostic reporter then filters the values to only return ones that report they have valid data
        /// </summary>
        public static IncrementalValuesProvider<T> ReportDiagnostics<T>(this IncrementalValuesProvider<DataOrDiagnostics<T>> values, IncrementalGeneratorInitializationContext context)
            where T : struct
        {
            context.ReportDiagnostics(values.Where(d => d.IsErrored).Select((d, _) => d.Diagnostics));
            return values.Where(d => d.IsValid).Select((d, _) => d.Data);
        }

        /// <summary>
        /// Registers an output node into an <see cref="IncrementalGeneratorInitializationContext"/> to output diagnostics.
        /// </summary>
        /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> instance.</param>
        /// <param name="diagnostics">The input <see cref="IncrementalValuesProvider{TValues}"/> sequence of diagnostics.</param>
        public static void ReportDiagnostics(this IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<Diagnostic> diagnostics)
        {
            context.RegisterSourceOutput(diagnostics, static (context, diagnostic) =>
            {
                context.ReportDiagnostic(diagnostic);
            });
        }

        /// <summary>
        /// Registers an output node into an <see cref="IncrementalGeneratorInitializationContext"/> to output diagnostics.
        /// </summary>
        /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> instance.</param>
        /// <param name="diagnostics">The input <see cref="IncrementalValuesProvider{TValues}"/> sequence of diagnostics.</param>
        public static void ReportDiagnostics(this IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ImmutableArray<Diagnostic>> diagnostics)
        {
            context.RegisterSourceOutput(diagnostics, static (context, diagnostics) =>
            {
                foreach (Diagnostic diagnostic in diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            });
        }

        /// <summary>
        /// Groups items in a given <see cref="IncrementalValuesProvider{TValue}"/> sequence by a specified key.
        /// </summary>
        /// <typeparam name="TLeft">The type of left items in each tuple.</typeparam>
        /// <typeparam name="TRight">The type of right items in each tuple.</typeparam>
        /// <typeparam name="TKey">The type of resulting key elements.</typeparam>
        /// <typeparam name="TElement">The type of resulting projected elements.</typeparam>
        /// <param name="source">The input <see cref="IncrementalValuesProvider{TValues}"/> instance.</param>
        /// <param name="keySelector">The key selection <see cref="Func{T, TResult}"/>.</param>
        /// <param name="elementSelector">The element selection <see cref="Func{T, TResult}"/>.</param>
        /// <returns>An <see cref="IncrementalValuesProvider{TValues}"/> with the grouped results.</returns>
        public static IncrementalValuesProvider<(TKey Key, EquatableArray<TElement> Right)> GroupBy<TLeft, TRight, TKey, TElement>
            (
                this IncrementalValuesProvider<(TLeft Left, TRight Right)> source,
                Func<(TLeft Left, TRight Right), TKey> keySelector,
                Func<(TLeft Left, TRight Right), TElement> elementSelector
            )

            where TLeft : IEquatable<TLeft>
            where TRight : IEquatable<TRight>
            where TKey : IEquatable<TKey>
            where TElement : IEquatable<TElement>
        {
            return source.Collect().SelectMany((item, token) =>
            {
                Dictionary<TKey, ImmutableArray<TElement>.Builder> map = new();

                foreach ((TLeft, TRight) pair in item)
                {
                    TKey key = keySelector(pair);
                    TElement element = elementSelector(pair);

                    if (!map.TryGetValue(key, out ImmutableArray<TElement>.Builder builder))
                    {
                        builder = ImmutableArray.CreateBuilder<TElement>();

                        map.Add(key, builder);
                    }

                    builder.Add(element);
                }

                token.ThrowIfCancellationRequested();

                ImmutableArray<(TKey Key, EquatableArray<TElement> Elements)>.Builder result =
                    ImmutableArray.CreateBuilder<(TKey, EquatableArray<TElement>)>();

                foreach (KeyValuePair<TKey, ImmutableArray<TElement>.Builder> entry in map)
                {
                    result.Add((entry.Key, entry.Value.ToImmutable()));
                }

                return result;
            });
        }

        public static IncrementalValuesProvider<(TKey Key, EquatableArray<TElement> Values)> GroupBy<TKey, TElement>
        (
            this IncrementalValuesProvider<TElement> source,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey> keyComparer
        )
            where TElement : IEquatable<TElement>
        {
            return source.Collect().SelectMany((collection, token) =>
            {
                Dictionary<TKey, ImmutableArray<TElement>.Builder> map = new(keyComparer);

                foreach (TElement item in collection)
                {
                    TKey key = keySelector(item);

                    if (!map.TryGetValue(key, out ImmutableArray<TElement>.Builder builder))
                    {
                        builder = ImmutableArray.CreateBuilder<TElement>();
                        map.Add(key, builder);
                    }

                    builder.Add(item);
                }

                token.ThrowIfCancellationRequested();

                ImmutableArray<(TKey Key, EquatableArray<TElement> Elements)>.Builder result =
                    ImmutableArray.CreateBuilder<(TKey, EquatableArray<TElement>)>();

                foreach (KeyValuePair<TKey, ImmutableArray<TElement>.Builder> entry in map)
                {
                    result.Add((entry.Key, entry.Value.ToImmutable()));
                }

                return result;
            });
        }
    }

    /// <summary>
    /// Extensions for <see cref="EquatableArray{T}"/>.
    /// </summary>
    internal static class EquatableArray
    {
        /// <summary>
        /// Creates an <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the input array.</typeparam>
        /// <param name="array">The input <see cref="ImmutableArray{T}"/> instance.</param>
        /// <returns>An <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</returns>
        public static EquatableArray<T> AsEquatableArray<T>(this ImmutableArray<T> array)
            where T : IEquatable<T>
        {
            return new(array);
        }
    }

    /// <summary>
    /// An immutable, equatable array. This is equivalent to <see cref="ImmutableArray{T}"/> but with value equality support.
    /// </summary>
    /// <typeparam name="T">The type of values in the array.</typeparam>
    internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// The underlying <typeparamref name="T"/> array.
        /// </summary>
        private readonly T[]? array;

        /// <summary>
        /// Creates a new <see cref="EquatableArray{T}"/> instance.
        /// </summary>
        /// <param name="array">The input <see cref="ImmutableArray{T}"/> to wrap.</param>
        public EquatableArray(ImmutableArray<T> array)
        {
            this.array = Unsafe.As<ImmutableArray<T>, T[]?>(ref array);
        }

        /// <summary>
        /// Gets a reference to an item at a specified position within the array.
        /// </summary>
        /// <param name="index">The index of the item to retrieve a reference to.</param>
        /// <returns>A reference to an item at a specified position within the array.</returns>
        public ref readonly T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref AsImmutableArray().ItemRef(index);
        }

        /// <summary>
        /// Gets a value indicating whether the current array is empty.
        /// </summary>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AsImmutableArray().IsEmpty;
        }

        /// <sinheritdoc/>
        public bool Equals(EquatableArray<T> array)
        {
            return AsSpan().SequenceEqual(array.AsSpan());
        }

        /// <sinheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is EquatableArray<T> array && Equals(this, array);
        }

        /// <sinheritdoc/>
        public override int GetHashCode()
        {
            if (this.array is not T[] array)
            {
                return 0;
            }

            HashCode hashCode = default;

            foreach (T item in array)
            {
                hashCode.Add(item);
            }

            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Gets an <see cref="ImmutableArray{T}"/> instance from the current <see cref="EquatableArray{T}"/>.
        /// </summary>
        /// <returns>The <see cref="ImmutableArray{T}"/> from the current <see cref="EquatableArray{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArray<T> AsImmutableArray()
        {
            return Unsafe.As<T[]?, ImmutableArray<T>>(ref Unsafe.AsRef(in this.array));
        }

        /// <summary>
        /// Creates an <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.
        /// </summary>
        /// <param name="array">The input <see cref="ImmutableArray{T}"/> instance.</param>
        /// <returns>An <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</returns>
        public static EquatableArray<T> FromImmutableArray(ImmutableArray<T> array)
        {
            return new(array);
        }

        /// <summary>
        /// Returns a <see cref="ReadOnlySpan{T}"/> wrapping the current items.
        /// </summary>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> wrapping the current items.</returns>
        public ReadOnlySpan<T> AsSpan()
        {
            return AsImmutableArray().AsSpan();
        }

        /// <summary>
        /// Copies the contents of this <see cref="EquatableArray{T}"/> instance to a mutable array.
        /// </summary>
        /// <returns>The newly instantiated array.</returns>
        public T[] ToArray()
        {
            return AsImmutableArray().ToArray();
        }

        /// <summary>
        /// Gets an <see cref="ImmutableArray{T}.Enumerator"/> value to traverse items in the current array.
        /// </summary>
        /// <returns>An <see cref="ImmutableArray{T}.Enumerator"/> value to traverse items in the current array.</returns>
        public ImmutableArray<T>.Enumerator GetEnumerator()
        {
            return AsImmutableArray().GetEnumerator();
        }

        /// <sinheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)AsImmutableArray()).GetEnumerator();
        }

        /// <sinheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)AsImmutableArray()).GetEnumerator();
        }

        /// <summary>
        /// Implicitly converts an <see cref="ImmutableArray{T}"/> to <see cref="EquatableArray{T}"/>.
        /// </summary>
        /// <returns>An <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</returns>
        public static implicit operator EquatableArray<T>(ImmutableArray<T> array)
        {
            return FromImmutableArray(array);
        }

        /// <summary>
        /// Implicitly converts an <see cref="EquatableArray{T}"/> to <see cref="ImmutableArray{T}"/>.
        /// </summary>
        /// <returns>An <see cref="ImmutableArray{T}"/> instance from a given <see cref="EquatableArray{T}"/>.</returns>
        public static implicit operator ImmutableArray<T>(EquatableArray<T> array)
        {
            return array.AsImmutableArray();
        }

        /// <summary>
        /// Checks whether two <see cref="EquatableArray{T}"/> values are the same.
        /// </summary>
        /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
        /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
        /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are equal.</returns>
        public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="EquatableArray{T}"/> values are not the same.
        /// </summary>
        /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
        /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
        /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are not equal.</returns>
        public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right)
        {
            return !left.Equals(right);
        }
    }
}
#endif