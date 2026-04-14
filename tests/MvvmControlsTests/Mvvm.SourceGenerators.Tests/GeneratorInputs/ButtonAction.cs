namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonAction
    {
        public bool RunSuccess { get; set; }

        /// <summary>
        /// Perform an Action
        /// </summary>
        [Button]
        private void Run() => RunSuccess = true;
    }
}
