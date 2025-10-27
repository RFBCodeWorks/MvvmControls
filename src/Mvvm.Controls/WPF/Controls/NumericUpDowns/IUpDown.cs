using CommunityToolkit.Mvvm.Input;

namespace RFBCodeWorks.WPF.Controls.Primitives
{
    /// <summary>
    /// Interface to interact with <see cref="UpDownBase{T}"/> objects in uniform manner
    /// </summary>
    public interface IUpDown
    {
        /// <inheritdoc cref="UpDownBase.DecreaseValueCommand"/>
        RelayCommand DecreaseValueCommand { get; init; }

        /// <inheritdoc cref="UpDownBase.IncreaseValueCommand"/>
        RelayCommand IncreaseValueCommand { get; init; }

        /// <inheritdoc cref="UpDownBase.IsReadOnly"/>
        bool IsReadOnly { get; set; }

        /// <inheritdoc cref="UpDownBase.IsValid"/>
        bool IsValid { get; set; }

        /// <inheritdoc cref="UpDownBase.StringFormat"/>
        string StringFormat { get; set; }
        
        /// <inheritdoc cref="UpDownBase{T}.Value"/>
        object Value { get; set; }

        /// <inheritdoc cref="UpDownBase{T}.Minimum"/>
        object Minimum { get; set; }

        /// <inheritdoc cref="UpDownBase{T}.Maximum"/>
        object Maximum { get; set; }

        /// <inheritdoc cref="UpDownBase{T}.SmallChange"/>
        object SmallChange { get; set; }

        /// <inheritdoc cref="UpDownBase{T}.LargeChange"/>
        object LargeChange { get; set; }
    }
}