namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonCanExecuteInteger
    {
        public bool RunSuccess { get; set; }
        public int Min { get; set; } = 0;
        public int Max { get; set;  } = 100;

        [Button(CanExecute = nameof(CanExecute))]
        private void Run(int value) => RunSuccess = true;

        private bool CanExecute(int value) => (value >= Min && value <= Max);
    }
}
