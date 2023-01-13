namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Interface for CheckBox Definitions
    /// </summary>
    public interface ICheckBox : IToggleButton
    {

    }

    /// <summary>
    /// Provides a definition for Checkbox controls
    /// </summary>
    public class CheckBoxDefinition : Primitives.ToggleButtonDefinition, ICheckBox
    {
    }
}
