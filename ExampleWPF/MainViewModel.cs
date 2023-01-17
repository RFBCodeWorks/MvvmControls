using RFBCodeWorks.Mvvm;
using RFBCodeWorks.WPF.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExampleWPF
{
    class MainViewModel : ViewModelBase, IWindowClosingHandler, IWindowFocusHandler, IWindowLoadingHandler
    {
        public MainViewModel()
        {

        }

        public DragDropHandler DragDropHandler { get; } = new DragDropHandler();

        public ListBoxDefinition<string> ListBoxDefinition { get; } = new ListBoxDefinition<string>()
        {
            Items = new string[] { "Index0", "Index1", "Index2", "Index3" },
            SelectionMode = SelectionMode.Multiple
        };

        public ListBoxDefinition<string> ListBoxDefinition2 { get; } = new ListBoxDefinition<string>()
        {
            Items = new string[] { "Howdy", "OhNo", "This is a string", "Hello" },
            SelectionMode = SelectionMode.Multiple
        };

        public ComboBoxDefinition<string> ComboBoxDefinition { get; } = new ComboBoxDefinition<string>()
        {
            Items = new string[] { "Index0", "Index1", "Index2", "Index3" }
        };

        public CheckBoxDefinition EnableDisableListBox { get; } = new CheckBoxDefinition()
        {
            IsThreeState = false,
            DisplayText = "Enable/Disable Listbox",
            IsChecked = true
        };

        public bool TriggerClosingWindowInterface { get; set; }

        public RadioButtonDefinition EnableComboBox { get; } = new RadioButtonDefinition() { DisplayText = "Enable ComboBox", GroupName = "EC" };
        public RadioButtonDefinition DisableComboBox { get; } = new RadioButtonDefinition() { DisplayText = "Disable ComboBox", GroupName = "EC" };

        public TextValidationControl TextValidation { get; }
            = new TextValidationControl(new System.Text.RegularExpressions.Regex("^[0-9]{1,4}$"))
            {
                Error = "Text must be a numeric value between 1 and 4 digits long, ex: 0 -> 9999",
                ToolTip = "Text must be a numeric value between 1 and 4 digits long, ex: 0 -> 9999",
                Text = "0"
            };


        /// <summary>
        /// 
        /// </summary>
        public bool WasContentRendered
        {
            get { return WasContentRenderedField; }
            set { SetProperty(ref WasContentRenderedField, value, nameof(WasContentRendered)); }
        }
        private bool WasContentRenderedField;


        /// <summary>
        /// 
        /// </summary>
        public bool WasLoaded
        {
            get { return WasLoadedField; }
            set { SetProperty(ref WasLoadedField, value, nameof(WasLoaded)); }
        }
        private bool WasLoadedField;


        public void OnUIElementGotFocus(object sender, EventArgs e)
        {
            
            if (sender is GroupBox box)
            {
                box.Background = new RadialGradientBrush(startColor: Colors.Transparent, endColor: Colors.Aquamarine)
                {
                    Opacity = 100,
                    Center = new Point(0.5, 0.5),
                    MappingMode = BrushMappingMode.RelativeToBoundingBox,
                    SpreadMethod = GradientSpreadMethod.Pad,
                };
                box.BorderBrush = new RadialGradientBrush(startColor: Colors.Black, endColor: Colors.Black)
                {
                    Opacity = 90,
                    //RadiusX = box.ActualWidth/2,
                    //RadiusY = box.ActualHeight/2,
                    //Center = new Point(0.5, 0.5),
                    //MappingMode = BrushMappingMode.RelativeToBoundingBox,
                    //SpreadMethod = GradientSpreadMethod.Reflect,
                };
                box.BorderThickness = new Thickness(2);
                //box.Padding = new Thickness(15);
                //box.FontSize = 15;
            }
        }

        public void OnUIElementLostFocus(object sender, EventArgs e)
        {
            if (sender is GroupBox box)
            {
                box.BorderBrush = default;
                box.Background = default;
                box.BorderThickness = default;
                box.Padding = default;
                box.ClearValue(GroupBox.FontSizeProperty);
            }
        }

        public void OnWindowClosed(object sender, EventArgs e)
        {
            if (TriggerClosingWindowInterface)
            System.Windows.MessageBox.Show("Window Closed", "Window Closed Event");
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (!TriggerClosingWindowInterface)
                return;

            e.Cancel = System.Windows.MessageBox.Show("This MessageBox was raised from the ViewModel.\n\nAllow window to close?", "Window Closing Event", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No;
        }

        public void OnWindowLoaded(object sender, EventArgs e)
        {
            WasLoaded = true;
        }

        public void OnWindowContentRendered(object sender, EventArgs e)
        {
            WasContentRendered = true;
        }
    }
}
