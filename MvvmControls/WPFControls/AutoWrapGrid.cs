using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.WPFControls
{
    /// <summary>
    /// Grid that acts like a vertical stack panel consisting of hortizontal stack panels, where each row is filled up before moving to the next row
    /// </summary>
    /// <remarks>
    /// Example with 5 items, and 3 columns specified: 
    /// 
    /// <br/>Item_1 - Item_2 - Item_3
    /// <br/>Item_4 - Item_5 - [Empty]
    /// <br/> [This row does not exist because there is no item 7]
    /// </remarks>
    public partial class AutoWrapGrid : SimpleGrid
    {

        /// <summary>
        /// Create a new AutoStackGrid
        /// </summary>
        public AutoWrapGrid() : base() { }

        /// <inheritdoc/>
        protected override void ForEachChild(UIElementCollection children)
        {
            // Loop through all child controls, assigning them to rows. Each time a column already called out is specified, it gets dropped into a new row.
            // EX:

            List<Point> GridPopulation = new();
            RowDefinitions.Clear();
            if (IsPaddingSpecified) AddRowDefinition(); //first row will be used for padding

            bool padCols = IsPaddingSpecified & ColumnCount > 3;
            int firstRow = IsPaddingSpecified ? 1 : 0;
            int firstCol = padCols ? 1 : 0;
            int lastCol = padCols ? ColumnCount - 2 : ColumnCount - 1; //Index of the last column to fill
            int curRow = firstRow;
            int curCol = firstCol - 1;
            var lastChild = Children[Children.Count - 1];

            foreach (System.Windows.UIElement curChild in children)
            {
                curCol++;
                if (curCol > lastCol)
                {
                    curCol = firstCol;
                    curRow++;
                    AddRowDefinition();
                }
                //Assign to the grid
                SetColumn(curChild, curCol);
                SetRow(curChild, curRow);
            }
            if (base.IsPaddingSpecified)RowCount++;
        }
    }
}
