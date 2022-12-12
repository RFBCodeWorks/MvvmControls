using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

namespace RFBCodeWorks.WPF.Controls
{
    /// <summary>
    /// Two styles of IPV4 Textboxes
    /// </summary>
    public enum IPV4TextboxStyle
    {
        /// <summary>
        /// Simple Masked Textbox
        /// </summary>
        Masked,
        /// <summary>
        /// Control consisting of 4 IntegerUpDowns
        /// </summary>
        Numeric
    }

    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:RFBCodeWorks.WPF.Controls.IPV4TextBox"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:RFBCodeWorks.WPF.Controls.IPV4TextBox;assembly=RFBCodeWorks.WPF.Controls.IPV4TextBox"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class IPV4TextBox : Control
    {
        static IPV4TextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IPV4TextBox), new FrameworkPropertyMetadata(typeof(IPV4TextBox)));
        }


        private bool SettingIPAddress;
        private bool SettingText;



        /// <summary>
        /// Decide on a Textbox Style
        /// </summary>
        public IPV4TextboxStyle InputFormat
        {
            get { return (IPV4TextboxStyle)GetValue(InputFormatProperty); }
            set { SetValue(InputFormatProperty, value); }
        }

        /// <inheritdoc cref="InputFormat"/>
        public static readonly DependencyProperty InputFormatProperty =
            DependencyProperty.Register("InputFormat", typeof(IPV4TextboxStyle), typeof(IPV4TextBox), new PropertyMetadata(IPV4TextboxStyle.Masked));


        /// <summary>
        /// 
        /// </summary>
        public IPAddress IPAddress
        {
            get { return (IPAddress )GetValue(IPAddressProperty); }
            set { SetValue(IPAddressProperty, value); }
        }

        /// <inheritdoc cref="IPV4TextBox.IPAddress"/>
        public static readonly DependencyProperty IPAddressProperty =
            DependencyProperty.Register("IPAddress", typeof(IPAddress ), typeof(IPV4TextBox), new PropertyMetadata(null, IPAddressChanged));

        private static void IPAddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var t = d as IPV4TextBox;
            if (t.SettingIPAddress) return;
            t.SettingIPAddress = true;
            t.Text = e.NewValue?.ToString() ?? "";
            t.SettingIPAddress = false;
        }

        /// <summary>
        /// The text representation of the IP Address
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <inheritdoc cref="Text"/>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(IPV4TextBox), new PropertyMetadata("0.0.0.0", TextValueChanged, CoerceTextValue));

        private static object CoerceTextValue(DependencyObject d, object baseValue)
        {
            return ((string)baseValue).Trim();
        }

        #region < Groups >

        /// <summary>
        /// The first group within the IPV4 value
        /// </summary>
        public int Group1
        {
            get { return (int)GetValue(Group1Property); }
            set { SetValue(Group1Property, value); }
        }

        /// <inheritdoc cref="Group1"/>
        public static readonly DependencyProperty Group1Property =
            DependencyProperty.Register("Group1", typeof(int), typeof(IPV4TextBox), new PropertyMetadata(0, GroupValueChangedCallBack, CoerceGroupValue));

        /// <summary>
        /// The second group within the IPV4 value
        /// </summary>
        public int Group2
        {
            get { return (int)GetValue(Group2Property); }
            set { SetValue(Group2Property, value); }
        }

        /// <inheritdoc cref="Group2"/>
        public static readonly DependencyProperty Group2Property =
            DependencyProperty.Register("Group2", typeof(int), typeof(IPV4TextBox), new PropertyMetadata(0, GroupValueChangedCallBack, CoerceGroupValue));

        /// <summary>
        /// The third group within the IPV4 value
        /// </summary>
        public int Group3
        {
            get { return (int)GetValue(Group3Property); }
            set { SetValue(Group3Property, value); }
        }

        /// <inheritdoc cref="Group3"/>
        public static readonly DependencyProperty Group3Property =
            DependencyProperty.Register("Group3", typeof(int), typeof(IPV4TextBox), new PropertyMetadata(0, GroupValueChangedCallBack, CoerceGroupValue));

        /// <summary>
        /// The last group within the IPV4 value
        /// </summary>
        public int Group4
        {
            get { return (int)GetValue(Group4Property); }
            set { SetValue(Group4Property, value); }
        }

        /// <inheritdoc cref="Group4"/>
        public static readonly DependencyProperty Group4Property =
            DependencyProperty.Register("Group4", typeof(int), typeof(IPV4TextBox), new PropertyMetadata(0, GroupValueChangedCallBack, CoerceGroupValue));

        private static object CoerceGroupValue(DependencyObject d, object baseValue)
        {
            int v = (int)baseValue;
            if (v <= 0)
                return 0;
            if (v >= 255)
                return 255;
            return v;
        }

        #endregion

        //lang=regex
        private const string RegexGroup = @"\s*|\s*[0-1]?[0-9]{1,2}\s*|\s*[2][0-4][0-9]\s*|\s*[2][5][0-5]\s*";

        /// <summary>
        /// Regex that is used to identify and parse valid IPV4 strings
        /// </summary>
        /// <remarks>
        /// Each group is identified into its own Group within the match. IDs are: 
        /// <br/> G1, G2, G3, G4
        /// </remarks>
        public static readonly Regex TextRegex = new Regex(@$"^(?<G1>{RegexGroup})\.(?<G2>{RegexGroup})\.(?<G3>{RegexGroup})\.(?<G4>{RegexGroup})$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        private static void TextValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var t = d as IPV4TextBox;
            if (t.SettingText) return;
            
            var match = TextRegex.Match((string)e.NewValue);
             if (match.Success)
            {
                t.SettingText = true;
                t.Group1 = int.Parse(match.Groups["G1"].Value);
                t.Group2 = int.Parse(match.Groups["G2"].Value);
                t.Group3 = int.Parse(match.Groups["G3"].Value);
                t.Group4 = int.Parse(match.Groups["G4"].Value);
                t.SettingIPAddress = true;
                t.IPAddress = IPAddress.Parse((string)e.NewValue);
                t.SettingText = false;
                t.SettingIPAddress = false;
            }
        }

        private static void GroupValueChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var t = d as IPV4TextBox;
            if (t.SettingText | t.SettingIPAddress) return;
            t.SettingText = true;
            t.Text = string.Format("{0}.{1}.{2}.{3}", t.Group1, t.Group2, t.Group3, t.Group4);
            t.SettingText = false;
        }
    }
}
