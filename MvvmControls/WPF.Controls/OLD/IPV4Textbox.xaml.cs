using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomControls.WPF
{
    /// <summary>
    /// Interaction logic for IPV4Textbox.xaml
    /// </summary>
    /// <remarks>
    /// <see href="https://stackoverflow.com/questions/35324285/how-to-create-masked-textbox-like-windows-ip-address-fields"/>
    /// </remarks>
    public partial class IPV4Textbox : UserControl, CustomControls.Interfaces.IPV4Textbox
    {
        public IPV4Textbox()
        {
            InitializeComponent();
            this.Group1.Value = 0;
            this.Group2.Value = 0;
            this.Group3.Value = 0;
            this.Group4.Value = 0;
            this.Group1.Minimum = 0;
            this.Group1.Maximum = 255;
            this.Group2.Minimum = 0;
            this.Group2.Maximum = 255;
            this.Group3.Minimum = 0;
            this.Group3.Maximum = 255;
            this.Group4.Minimum = 0;
            this.Group4.Maximum = 255;
        }

        public string Address
        {
            get { return (string)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        
        private static readonly List<Key> MoveForwardKeys = new List<Key> { Key.Right };
        private static readonly List<Key> MoveBackwardKeys = new List<Key> { Key.Left };
        private static readonly List<Key> OtherAllowedKeys = new List<Key> { Key.Tab, Key.Delete };
        private readonly List<TextBox> _segments = new List<TextBox>();
        private bool _suppressAddressUpdate = false;


        public static readonly DependencyProperty AddressProperty = DependencyProperty.Register(
            "Address", typeof (string), typeof (IPV4Textbox), new FrameworkPropertyMetadata(default(string), AddressChanged)
            {
                BindsTwoWayByDefault = true
            });

        private static void AddressChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var IPV4Textbox = dependencyObject as IPV4Textbox;
            var text = e.NewValue as string;

            if (text != null && IPV4Textbox != null)
            {
                IPV4Textbox._suppressAddressUpdate = true;
                var i = 0;
                foreach (var segment in text.Split('.'))
                {
                    IPV4Textbox._segments[i].Text = segment;
                    i++;
                }
                IPV4Textbox._suppressAddressUpdate = false;
            }
        }

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DigitKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelDigitKeyPress();
                HandleDigitPress();
            }
            else if(MoveBackwardKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelBackwardKeyPress();
                HandleBackwardKeyPress();
            }
            else if (MoveForwardKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelForwardKeyPress();
                HandleForwardKeyPress();
            } else if (e.Key == Key.Back)
            {
                HandleBackspaceKeyPress();
            }
            else if (e.Key == Key.OemPeriod)
            {
                e.Handled = true;
                HandlePeriodKeyPress();
            }
            else
            {
                e.Handled = !AreOtherAllowedKeysPressed(e);
            }
        }

        private bool AreOtherAllowedKeysPressed(KeyEventArgs e)
        {
            return e.Key == Key.C && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   e.Key == Key.V && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   e.Key == Key.A && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   e.Key == Key.X && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   OtherAllowedKeys.Contains(e.Key);
        }

        private void HandleDigitPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.Text.Length == 3 &&
                currentTextBox.CaretIndex == 3 && currentTextBox.SelectedText.Length == 0)
            {
                MoveFocusToNextSegment(currentTextBox);
            }
        }

        private bool ShouldCancelDigitKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;
            return currentTextBox != null && 
                   currentTextBox.Text.Length == 3 && 
                   currentTextBox.CaretIndex == 3 && 
                   currentTextBox.SelectedText.Length == 0;
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_suppressAddressUpdate)
            {
                Address = string.Format("{0}.{1}.{2}.{3}", Group1.Value, Group2.Value, Group3.Value, Group4.Value);
            }

            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.Text.Length == 3 && currentTextBox.CaretIndex == 3)
            {
                MoveFocusToNextSegment(currentTextBox);
            }
        }

        private bool ShouldCancelBackwardKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;
            return currentTextBox != null && currentTextBox.CaretIndex == 0;
        }

        private void HandleBackspaceKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.CaretIndex == 0 && currentTextBox.SelectedText.Length == 0)
            {
                MoveFocusToPreviousSegment(currentTextBox);
            }
        }

        private void HandleBackwardKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.CaretIndex == 0)
            {
                MoveFocusToPreviousSegment(currentTextBox);
            }
        }

        private bool ShouldCancelForwardKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;
            return currentTextBox != null && currentTextBox.CaretIndex == 3;
        }

        private void HandleForwardKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.CaretIndex == currentTextBox.Text.Length)
            {
                MoveFocusToNextSegment(currentTextBox);
            }
        }

        private void HandlePeriodKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.Text.Length > 0 && currentTextBox.CaretIndex == currentTextBox.Text.Length)
            {
                MoveFocusToNextSegment(currentTextBox);
            }
        }

        private void MoveFocusToPreviousSegment(TextBox currentTextBox)
        {
            if (!ReferenceEquals(currentTextBox, Group1))
            {
                var previousSegmentIndex = _segments.FindIndex(box => ReferenceEquals(box, currentTextBox)) - 1;
                currentTextBox.SelectionLength = 0;
                currentTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                _segments[previousSegmentIndex].CaretIndex = _segments[previousSegmentIndex].Text.Length;
            }
        }

        private void MoveFocusToNextSegment(TextBox currentTextBox)
        {
            if (!ReferenceEquals(currentTextBox, Group4))
            {
                currentTextBox.SelectionLength = 0;
                currentTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void DataObject_OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText)
            {
                e.CancelCommand();
                return;
            }

            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;

            int num;

            if (!int.TryParse(text, out num))
            {
                e.CancelCommand();
            }
        }

        public void Hide()
        {
            ((IPV4Textbox)this).Hide();
        }

        public void Show()
        {
            ((IPV4Textbox)this).Show();
        }
    }
}
