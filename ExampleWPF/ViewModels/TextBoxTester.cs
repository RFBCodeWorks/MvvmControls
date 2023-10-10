using RFBCodeWorks.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleWPF.ViewModels
{
    class TextBoxTester
    {
        public TextBoxTester()
        {
            ResetTextboxBtn = new ButtonDefinition(this.TextCBoxDef.Refresh) { DisplayText = "Reset Text" };
        }

        public TextValidationControl TextValidation { get; }
            = new TextValidationControl(new System.Text.RegularExpressions.Regex("^[0-9]{1,4}$"))
            {
                Error = "Text must be a numeric value between 1 and 4 digits long, ex: 0 -> 9999",
                ToolTip = "Text must be a numeric value between 1 and 4 digits long, ex: 0 -> 9999",
                Text = "0"
            };

        public RFBCodeWorks.Mvvm.TextControlDefinition TextCBoxDef { get; }
            = new TextControlDefinition()
            {
                IsReadOnly = false,
                GetText = () => "This textbox has been reset to its default value"
            };

        public ButtonDefinition ResetTextboxBtn { get; }
    }
}
