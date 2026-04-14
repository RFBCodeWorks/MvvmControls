using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonTask
    {
        public bool RunSuccess { get; set; }
        public int DelayPeriod { get; set; } = 5;

        /// <summary>
        /// Perform an action Asynchronously
        /// </summary>
        /// <returns></returns>
        [Button]
        private async Task Run()
        {
            await Task.Delay(DelayPeriod);
            RunSuccess = true;
        }
    }
}
