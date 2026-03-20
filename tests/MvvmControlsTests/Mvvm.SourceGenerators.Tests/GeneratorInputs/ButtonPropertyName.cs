using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class ButtonPropertyName
    {
        public const string ExpectedPropertyName = "RunMe";
        
        public bool RunSuccess { get; set; }

        [Button(PropertyName = ExpectedPropertyName)]
        private void Run() => RunSuccess = true;
    }
}
