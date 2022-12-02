using System.Text.RegularExpressions;
using System.Windows;
using System.Linq;
using System.Windows.Controls;

namespace RFBCodeWorks.WPF.Controls
{
    /// <summary>
    /// Grid that can be defined using dependency properties for column/row counts instead of a full definition
    /// </summary>
    public class SimpleGrid : CustomGrid
    {
        private static readonly Regex BorderRegex1 = new Regex(@"^(?<ALL>[0-9]+)$", options: RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex BorderRegex2 = new Regex(@"^(?<Horizontal>[0-9]+)\s*,\s*(?<Vertical>[0-9]+)$", options: RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex BorderRegex4 = new Regex(@"^(?<Left>[0-9]+)\s*,\s*(?<Top>[0-9]+)\s*,\s*(?<Right>[0-9]+)\s*,\s*(?<Bottom>[0-9]+)$", options: RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        /// <summary> TRUE if the padding is specified, otherwise false </summary>
        protected bool IsPaddingSpecified => !string.IsNullOrEmpty(Padding);
        private GridLength[] parsedPadding = null;
        private GridLength[] ParseBorderSize()
        {
            if (!string.IsNullOrWhiteSpace(Padding))
            {
                int i = 0;
                GridLength l = default;
                GridLength[] sizes = new GridLength[4] { l, l, l, l };
                if (BorderRegex1.IsMatch(Padding))
                {
                    i = int.Parse(BorderRegex1.Match(Padding).Groups["ALL"].Value);
                    sizes[0] = new GridLength(i);
                    sizes[1] = new GridLength(i);
                    sizes[2] = new GridLength(i);
                    sizes[3] = new GridLength(i);
                }
                else if (BorderRegex2.IsMatch(Padding))
                {
                    var groups = BorderRegex2.Match(Padding).Groups;
                    i = int.Parse(groups["Horizontal"].Value);
                    sizes[0] = new GridLength(i);
                    sizes[2] = new GridLength(i);
                    i = int.Parse(groups["Vertical"].Value);
                    sizes[1] = new GridLength(i);
                    sizes[3] = new GridLength(i);
                }
                else if (BorderRegex4.IsMatch(Padding))
                {
                    var groups = BorderRegex4.Match(Padding).Groups;
                    sizes[0] = new GridLength(int.Parse(groups["Left"].Value));
                    sizes[1] = new GridLength(int.Parse(groups["Top"].Value));
                    sizes[2] = new GridLength(int.Parse(groups["Right"].Value));
                    sizes[3] = new GridLength(int.Parse(groups["Bottom"].Value));
                }
                return sizes;
            }
            return System.Array.Empty<GridLength>();
        }


        /// <summary>
        /// Define the dimensions of the outer-most columns and rows. Only applies if row/column count are greater than 2.
        /// </summary>
        public string Padding
        {
            get { return (string)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        /// <inheritdoc cref="Padding"/>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(nameof(Padding), typeof(string), typeof(SimpleGrid), new PropertyMetadata(string.Empty, BorderSizeChanged));


        /// <summary>
        /// Setting this value recreates the column definitions to the specified amount of columns.
        /// </summary>
        /// <returns>
        /// The number of columns in the column definition
        /// </returns>
        public int ColumnCount
        {
            get { return base.ColumnDefinitions.Count; }
            set { SetValue(ColumnCountProperty, value); }
        }

        /// <inheritdoc cref="ColumnCount"/>
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register(nameof(ColumnCount), typeof(int), typeof(SimpleGrid), new PropertyMetadata(0, ColumnCountChanged));

        /// <inheritdoc cref="ColumnCount"/>
        public static int GetColumnCount(DependencyObject obj)
        {
            if (obj is SimpleGrid cntrl)
                return cntrl.ColumnCount;
            return (int)obj.GetValue(ColumnCountProperty);
        }

        /// <inheritdoc cref="ColumnCount"/>
        public static void SetColumnCount(DependencyObject obj, int value)
        {
            if (obj is SimpleGrid cntrl)
                cntrl.ColumnCount= value;
        }

        /// <summary>
        /// Setting this value recreates the row definitions to the specified amount of rows.
        /// </summary>
        /// <returns>
        /// The number of rows in the row definition
        /// </returns>
        public int RowCount
        {
            get { return base.RowDefinitions.Count; }
            set { SetValue(RowCountProperty, value); }
        }

        /// <inheritdoc cref="RowCount"/>
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register(nameof(RowCount), typeof(int), typeof(SimpleGrid), new PropertyMetadata(0, RowCountChanged));

        /// <inheritdoc cref="RowCount"/>
        public static int GetRowCount(DependencyObject obj)
        {
            if (obj is SimpleGrid cntrl)
                return cntrl.RowCount;
            return (int)obj.GetValue(RowCountProperty);
        }

        /// <inheritdoc cref="RowCount"/>
        public static void SetRowCount(DependencyObject obj, int value)
        {
            if (obj is SimpleGrid cntrl)
                cntrl.RowCount = value;
        }

        private static void BorderSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            //Validate
            bool wasEmpty = string.IsNullOrEmpty((string)args.OldValue);
            string newVal = (string)args.NewValue;
            bool isEmpty = string.IsNullOrEmpty(newVal);
            bool isValid = isEmpty || (BorderRegex1.IsMatch(newVal) || BorderRegex2.IsMatch(newVal) || BorderRegex4.IsMatch(newVal));

            if (sender is SimpleGrid cntrl)
            {
                cntrl.SetPadding();
                if (!isValid)
                {
                    cntrl.Padding = (string)args.OldValue;
                    throw new System.ArgumentException("SimpleGrid.BorderSize value must follow the same pattern as a margin/padding property string - ex: '5' or '5,5' or '5,5,5,5'");
                }
            }
        }

        private void SetPadding()
        {
            parsedPadding = ParseBorderSize();
            if (parsedPadding.Length > 0)
            {
                if (RowCount >= 3)
                {
                    RowDefinitions[0].Height = parsedPadding[1];
                    RowDefinitions.Last().Height = parsedPadding[3];
                }
                if (ColumnCount >= 3)
                {
                    ColumnDefinitions[0].Width = parsedPadding[0];
                    ColumnDefinitions.Last().Width = parsedPadding[2];
                }
            }
            else
            {
                if (RowDefinitions.Any())
                {
                    RowDefinitions.First().Height = AutoRowHeight;
                    RowDefinitions.Last().Height = AutoRowHeight;
                    EvaluateFillLastRow();
                }
                if (ColumnDefinitions.Any())
                {
                    ColumnDefinitions.First().Width = AutoColumnWidth;
                    ColumnDefinitions.Last().Width = AutoColumnWidth;
                    EvaluateFillLastColumn();
                }
            }
        }

        private static void RowCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is SimpleGrid cntrl)
            {
                int old = (int)args.OldValue;
                int rows = (int)args.NewValue;
                if (rows == old) return;
                int startAdd = old;
                if (rows < old)
                {
                    cntrl.RowDefinitions.Clear();
                    startAdd = 0;
                }


                GridLength FirstRow = cntrl.AutoRowHeight;
                GridLength LastRow = cntrl.AutoRowHeight;
                if (cntrl.parsedPadding?.Length > 0)
                {
                    FirstRow = cntrl.parsedPadding[1];
                    LastRow = cntrl.parsedPadding[3];
                }
                for (int i = startAdd; i < rows; i++)
                {
                    if (i == 0 && rows >= 3)
                    {
                        cntrl.AddRowDefinition(FirstRow);
                    }
                    else if (i == rows - 1 && rows >= 3)
                    {
                        cntrl.AddRowDefinition(LastRow);
                    }
                    else
                    {
                        _ = cntrl.AddRowDefinition();
                    }
                }
            }
        }

        private static void ColumnCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is SimpleGrid cntrl)
            {
                int old = (int)args.OldValue;
                int cols = (int)args.NewValue;
                if (cols == old) return;
                int startAdd = old;
                if (cols < old)
                {
                    cntrl.ColumnDefinitions.Clear();
                    startAdd = 0;
                }

                GridLength First = cntrl.AutoColumnWidth;
                GridLength Last = cntrl.AutoColumnWidth;
                if (cntrl.parsedPadding?.Length > 0)
                {
                    First = cntrl.parsedPadding[0];
                    Last = cntrl.parsedPadding[2];
                }
                for (int i = startAdd; i < cols; i++)
                {
                    if (i == 0 && cols >= 3)
                    {
                        cntrl.AddColumnDefinition(First);
                    }
                    else if (i == cols - 1 && cols >= 3)
                    {
                        cntrl.AddColumnDefinition(Last);
                    }
                    else
                    {
                        _ = cntrl.AddColumnDefinition();
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            SetPadding();
        }

        /// <inheritdoc/>
        protected override void ForEachChild(UIElementCollection children)
        {
            /* This grid has no effect on its children! */
        }

        /// <inheritdoc/>
        protected override void EvaluateFillLastColumn()
        {
            if (base.FillLastColumn)
            {
                if (IsPaddingSpecified && ColumnCount > 3)
                {
                    ColumnDefinitions[ColumnDefinitions.Count - 2].Width = new GridLength(AutoColumnWidth.Value, GridUnitType.Star);
                }
                else
                    base.EvaluateFillLastColumn();
            }
        }

        /// <inheritdoc/>
        protected override void EvaluateFillLastRow()
        {
            if (base.FillLastRow)
            {
                if (IsPaddingSpecified && RowCount > 3)
                {
                    RowDefinitions[RowDefinitions.Count - 2].Height = new GridLength(AutoRowHeight.Value, GridUnitType.Star);
                }
                else
                    base.EvaluateFillLastRow();
            }
        }

    }
}
