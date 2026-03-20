using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonTaskAllowConcurrentExecutionsFalse
    {
        public bool RunSuccess { get; set; }
        public int DelayPeriod { get; set; } = 10;

        [Button(AllowConcurrentExecutions = false)]
        private async Task Run()
        {
            await Task.Delay(DelayPeriod);
            RunSuccess = true;
        }
    }
}
