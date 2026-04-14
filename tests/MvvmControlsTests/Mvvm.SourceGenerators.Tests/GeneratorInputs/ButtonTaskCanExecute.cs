using System.Threading;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonTaskCanExecute
    {
        public bool RunSuccess { get; set; }
        public bool CanExecute { get; set; }
        public int DelayPeriod { get; set; } = 5;

        [Button(CanExecute = nameof(CanExecuteFunc))]
        private async Task Run(CancellationToken token)
        {
            await Task.Delay(DelayPeriod, token);
            token.ThrowIfCancellationRequested();
            RunSuccess = true;
        }
        private bool CanExecuteFunc() => CanExecute;
    }
}
