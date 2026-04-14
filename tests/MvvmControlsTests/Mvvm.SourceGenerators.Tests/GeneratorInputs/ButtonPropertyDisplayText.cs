using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonPropertyDisplayText
    {
        public const string ExpectedDisplayText = "Click Me";

        public bool RunSuccess { get; set; }
        public int DelayPeriod { get; set;  }

        [Button(DisplayText = ExpectedDisplayText)]
        private async Task Run()
        {
            await Task.Delay(DelayPeriod);
            RunSuccess = true;
        }
    }
}
