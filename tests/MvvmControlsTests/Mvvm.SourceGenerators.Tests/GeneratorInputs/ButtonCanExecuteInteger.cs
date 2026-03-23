namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonCanExecuteInteger
    {
        public bool RunSuccess { get; set; }
        public int Min { get; set; } = 0;
        public int Max { get; set;  } = 100;

        /// <summary>
        /// Perform an Action using the specified <paramref name="value"/>
        /// </summary>
        /// <param name="value">The value to use</param>
        [Button(CanExecute = nameof(CanExecute))]
        private void Run(int value) => RunSuccess = true;

        private bool CanExecute(int value) => (value >= Min && value <= Max);
    }
}
