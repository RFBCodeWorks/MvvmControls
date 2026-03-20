using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonActionCanExecute
    {
        public bool RunSuccess { get; set; }
        public bool CanExecute { get; set; }

        [Button(CanExecute = nameof(CanExecuteFunc))]
        private void Run() => RunSuccess = true;
        private bool CanExecuteFunc() => CanExecute;
    }
}
