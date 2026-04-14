using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonTaskAllowConcurrentExecutionsTrue
    {
        public bool RunSuccess { get; set; }
        public int DelayPeriod { get; set; } = 5;

        [Button(AllowConcurrentExecutions = true)]
        private async Task Run()
        {
            await Task.Delay(DelayPeriod);
            RunSuccess = true;
        }
    }
}
