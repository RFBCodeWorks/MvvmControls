using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonTaskAllowConcurrentExecutionsFalse
    {
        public bool RunSuccess { get; set; }
        public int DelayPeriod { get; set; } = 250;

        [Button(AllowConcurrentExecutions = false)]
        private async Task Run()
        {
            await Task.Delay(DelayPeriod);
            RunSuccess = true;
        }
    }
}
