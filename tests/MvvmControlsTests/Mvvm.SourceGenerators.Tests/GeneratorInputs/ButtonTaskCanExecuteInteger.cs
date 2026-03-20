using System.Threading;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonTaskCanExecuteInteger
    {
        public bool RunSuccess { get; set; }
        public int DelayPeriod { get; set; } = 5;
        public int Min { get; set; } = 0;
        public int Max { get; set; } = 100;

        [Button(CanExecute = nameof(CanExecute))]
        private async Task Run(int value, CancellationToken token)
        {
            await Task.Delay(DelayPeriod, token);
            token.ThrowIfCancellationRequested();
            RunSuccess = true;
        }

        private bool CanExecute(int value) => (value >= Min && value <= Max);
    }
}
