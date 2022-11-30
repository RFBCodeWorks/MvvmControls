using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        /// <summary>
        /// Initialize the NumericUpDown Control
        /// </summary>
        public NumericUpDown()
        {
            InitializeComponent();
            DataObject.AddPastingHandler(this.NumBox, OnPaste);
        }

        #region < Value Properties >

        /// <summary>
        /// Represents the original value of the control - to be used if highlighting changed values is enabled
        /// </summary>
        public double DefaultValue
        {
            get { return (double)GetValue(OriginalValueProperty); }
            set
            {
                var oldVal = DefaultValue;
                if (value != oldVal)
                {
                    SetValue(OriginalValueProperty, value);
                    //OriginalValueChanged?.Invoke(this, new(oldVal, value));
                    //OnDataValidation();
                }
            }
        }

        // Using a DependencyProperty as the backing store for DefaultValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginalValueProperty =
            DependencyProperty.Register(nameof(DefaultValue), typeof(double), typeof(NumericUpDown), new PropertyMetadata(0, BGChanged));

        #endregion

        #region < Other Properties >





        

        /// <summary>
        /// 
        /// </summary>
        public bool HighlightIfChanged
        {
            get { return (bool)GetValue(HighlightIfChangedProperty); }
            set { SetValue(HighlightIfChangedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightIfChangedProperty =
            DependencyProperty.Register(nameof(HighlightIfChanged), typeof(bool), typeof(NumericUpDown), new PropertyMetadata(true, BGChanged));

        public Brush OutsideRangeBrushColor
        {
            get { return (Brush)GetValue(OutsideRangeBrushColorProperty); }
            set { SetValue(OutsideRangeBrushColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OutsideRangeBrushColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OutsideRangeBrushColorProperty =
            DependencyProperty.Register(nameof(OutsideRangeBrushColor), typeof(Brush), typeof(NumericUpDown), new PropertyMetadata(Brushes.Pink, BGChanged));

        public Brush ValueChangedBrushColor
        {
            get { return (Brush)GetValue(ValueChangedBrushColorProperty); }
            set { SetValue(ValueChangedBrushColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueChangedBrushColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueChangedBrushColorProperty =
            DependencyProperty.Register(nameof(ValueChangedBrushColor), typeof(Brush), typeof(NumericUpDown), new PropertyMetadata(Brushes.LightYellow, BGChanged));


        #endregion





        internal Brush BGHelper
        {
            get { return (Brush)GetValue(BGHelperProperty); }
            set { SetValue(BGHelperProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty BGHelperProperty =
            DependencyProperty.Register(nameof(BGHelper), typeof(Brush), typeof(NumericUpDown), new PropertyMetadata(Brushes.White, BGChanged));

        private static void BGChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var cntrl = (NumericUpDown)sender;
            if (cntrl is null) return;
            if (cntrl.Value < cntrl.Minimum | cntrl.Value > cntrl.Maximum)
            {
                cntrl.NumBox.Background = cntrl.OutsideRangeBrushColor;
            }
            else if (cntrl.HighlightIfChanged && cntrl.Value != cntrl.DefaultValue)
            {
                cntrl.NumBox.Background = cntrl.ValueChangedBrushColor;
            }
            else
                cntrl.NumBox.Background = cntrl.Background;
        }


        
    }
}
