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
            MenuItemTestCommand = new RelayCommand(() => System.Windows.MessageBox.Show("Menu Item 1 Command has fired."));
            MenuItemTestCommand2 = new RelayCommand(() => System.Windows.MessageBox.Show("Menu Item 2 Command has fired."));
        }

        public bool TriggerClosingWindowInterface { get; set; }

        public DragDropHandler DragDropHandler { get; } = new DragDropHandler();

        public ViewModels.ComboBoxTester ComboxTester { get; } = new();
        public ViewModels.ListboxTester ListBoxTester { get; } = new();
        public ViewModels.TextBoxTester TextBoxTester { get; } = new();
        public ViewModels.XmlLinqViewModel XmlLinqTester { get; } = new();

        public ViewModels.TreeViewViewModel TreeViewTester { get; } = new();
 

        public ButtonDefinition ButtonContentTest { get; } = new(
            execute: () => MessageBox.Show("This button is assigned!")
            )
        {
            DisplayText = "This Button's Content is specified by the ControlDefinition",
            ToolTip = "This Tooltip was was specified by the ControlDefinition."
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
            set { 
                SetProperty(ref WasLoadedField, value, nameof(WasLoaded));
                //RFBCodeWorks.Mvvm.DialogServices.DialogServiceLocator.DefaultLocator.GetDialogService(this).ShowMessageBox(this, "Loaded!");
            }
        }
        private bool WasLoadedField;


        public RelayCommand MenuItemTestCommand { get; }
        public RelayCommand MenuItemTestCommand2 { get; }

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
