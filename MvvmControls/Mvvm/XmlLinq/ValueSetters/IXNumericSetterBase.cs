using System;

namespace RFBCodeWorks.Mvvm.XmlLinq.ValueSetters
{
    /// <summary>
    /// Interface to interact with <see cref="XNumericSetterBase{T}"/> objects in a standardized way
    /// </summary>
    public interface IXNumericSetter
    {
        /// <inheritdoc cref="XNumericSetterBase{T}.Maximum"/>
        double Maximum { get; set; }
        /// <inheritdoc cref="XNumericSetterBase{T}.Minimum"/>
        double Minimum { get; set; }
        /// <inheritdoc cref="XNumericSetterBase{T}.Value"/>
        double Value { get; set; }

        /// <inheritdoc cref="XNumericSetterBase{T}.InvalidValueSubmitted"/>
        event EventHandler InvalidValueSubmitted;
    }
}