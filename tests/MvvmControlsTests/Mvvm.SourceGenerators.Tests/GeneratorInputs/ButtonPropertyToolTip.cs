using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonPropertyToolTip
    {
        public const string ExpectedToolTip = "Custom ToolTip";

        public bool RunSuccess { get; set; }
        public int DelayPeriod { get; set;  }

        [Button(Tooltip = ExpectedToolTip)]
        private async Task Run()
        {
            await Task.Delay(DelayPeriod);
            RunSuccess = true;
        }
    }
}
