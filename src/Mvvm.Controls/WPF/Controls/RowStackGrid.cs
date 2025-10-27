using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.WPF.Controls
{
    /// <summary>
    /// Grid that acts like a StackPanel control for rows, but grid for columns.
    /// </summary>
    public partial class RowStackGrid : SimpleGrid
    {

        /// <summary>
        /// Create a new RowStackGrid
        /// </summary>
        public RowStackGrid() : base() { }

        /// <inheritdoc/>
        protected override void ForEachChild(UIElementCollection children)
        {
            // Loop through all child controls, assigning them to rows. Each time a column already called out is specified, it gets dropped into a new row.
            // EX:

            RowDefinitions.Clear();
            int curRow = -1;
            List<Point> GridPopulation = new();
            foreach (System.Windows.UIElement curChild in Children)
            {
                int expectedCol = GetColumn(curChild);
                Point assignedCell = new(expectedCol, curRow);

                if (curRow < 0 || GridPopulation.Contains(assignedCell, PointComparer.Comparer))
                {
                    //create a new row
                    curRow++;
                    RowDefinitions.Add(GetNewRowDef());
                    assignedCell = new(expectedCol, curRow);
                }

                //Assign to the grid
                SetColumn(curChild, (int)assignedCell.X);
                SetRow(curChild, (int)assignedCell.Y);
                GridPopulation.Add(assignedCell);
            }
            if (base.IsPaddingSpecified)RowCount++;
        }
    }
}
