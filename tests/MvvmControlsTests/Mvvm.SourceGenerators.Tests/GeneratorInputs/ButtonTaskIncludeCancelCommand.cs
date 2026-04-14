using System.Threading;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonTaskIncludeCancelCommand
    {
        public bool RunSuccess { get; set; }
        public int DelayPeriod { get; set; } = 5;

        [Button(IncludeCancelCommand = true)]
        private async Task Run(CancellationToken token)
        {
            await Task.Delay(DelayPeriod, token);
            token.ThrowIfCancellationRequested();
            RunSuccess = true;
        }
    }
}
