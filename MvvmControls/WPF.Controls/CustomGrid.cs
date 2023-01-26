using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RFBCodeWorks.WPF.Controls
{
    /// <summary>
    /// Abstract Base class for a custom Grid control
    /// </summary>
    public abstract class CustomGrid : Grid
    {

        /// <summary>
        /// Initialize
        /// </summary>
        public CustomGrid() : base() { }

        /// <summary>
        /// Flag set by the OnRender method to allow it to re-render while in design-mode
        /// </summary>
        protected bool firstRenderComplete { get; private set; }

        /// <inheritdoc/>
        protected override void OnRender(DrawingContext dc)
        {
            //This allows the render to update in design mode, but prevents constant recalculating while not in design mode
            if (!firstRenderComplete)
            {
                OnVisualChildrenChanged(null, null);
                firstRenderComplete = !this.IsDesignMode();
            }

            base.OnRender(dc);
        }

        /// <inheritdoc/>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (Children != null)
                ForEachChild(Children);

            EvaluateFillLastRow();
            EvaluateFillLastColumn();
        }

        /// <summary>
        /// If <see cref="OnVisualChildrenChanged(DependencyObject, DependencyObject)"/> is not overridden, the <see cref="CustomGrid"/> base class will call this method. <br/>
        /// Use this method to iterate against all the children and populate the datagrid rows/columns.
        /// </summary>
        /// <param name="children">The grid children</param>
        protected abstract void ForEachChild(UIElementCollection children);


        #region < Dependency Properties >

        /// <summary>
        /// Automatically created Columns will use this width
        /// </summary>
        public GridLength AutoColumnWidth
        {
            get { return (GridLength)GetValue(AutoColumnHeightProperty); }
            set { SetValue(AutoColumnHeightProperty, value); }
        }

        /// <inheritdoc cref="AutoColumnWidth"/>
        public static readonly DependencyProperty AutoColumnHeightProperty =
            DependencyProperty.Register(nameof(AutoColumnWidth), typeof(GridLength), typeof(CustomGrid), new PropertyMetadata(default));


        /// <summary>
        /// Automatically created Rows will use this height
        /// </summary>
        public GridLength AutoRowHeight
        {
            get { return (GridLength)GetValue(AutoRowLengthProperty); }
            set { SetValue(AutoRowLengthProperty, value); }
        }

        /// <inheritdoc cref="AutoRowHeight"/>
        public static readonly DependencyProperty AutoRowLengthProperty =
            DependencyProperty.Register(nameof(AutoRowHeight), typeof(GridLength), typeof(CustomGrid), new PropertyMetadata(default));

        /// <summary>
        /// If true, set the last column to '*' instead of 'Auto'
        /// </summary>
        public bool FillLastColumn
        {
            get { return (bool)GetValue(FillLastColumnProperty); }
            set { SetValue(FillLastColumnProperty, value); }
        }

        /// <inheritdoc cref="FillLastColumn"/>
        public static readonly DependencyProperty FillLastColumnProperty =
            DependencyProperty.Register(nameof(FillLastColumn), typeof(bool), typeof(CustomGrid), new PropertyMetadata(false));

        /// <summary>
        /// If true, set the last row to '*' instead of 'Auto'
        /// </summary>
        public bool FillLastRow
        {
            get { return (bool)GetValue(FillLastRowProperty); }
            set { SetValue(FillLastRowProperty, value); }
        }

        /// <inheritdoc cref="FillLastRow"/>
        public static readonly DependencyProperty FillLastRowProperty =
            DependencyProperty.Register(nameof(FillLastRow), typeof(bool), typeof(CustomGrid), new PropertyMetadata(false));

        #endregion

        #region < Get New Col/Rows >

        /// <summary>
        /// Create a new Row Definition for the grid
        /// </summary>
        protected virtual RowDefinition GetNewRowDef(GridLength? height = null) => new RowDefinition() { Height = height ?? AutoRowHeight };

        /// <summary>
        /// Create a new Column Definition for the grid
        /// </summary>
        protected virtual ColumnDefinition GetNewColDef(GridLength? width = null) => new ColumnDefinition() { Width = width ?? AutoColumnWidth };

        /// <summary>
        /// Gets a new row definition via <see cref="GetNewRowDef"/>, adds it to the RowDefinitions collection, then returns the row that was created.
        /// </summary>
        /// <returns>a new RowDefinition</returns>
        protected virtual RowDefinition AddRowDefinition(GridLength? height = null)
        {
            var Def = GetNewRowDef(height);
            RowDefinitions.Add(Def);
            return Def;
        }

        /// <summary>
        /// Gets a new Column definition via <see cref="GetNewColDef"/>, adds it to the ColumnDefinitions collection, then returns the column that was created.
        /// </summary>
        /// <returns>a new ColumnDefinition</returns>
        protected virtual ColumnDefinition AddColumnDefinition(GridLength? width = null)
        {
            var Def = GetNewColDef(width);
            ColumnDefinitions.Add(Def);
            return Def;
        }

        #endregion

        /// <summary>
        /// Clears both the Row and Column definitions
        /// </summary>
        protected void ClearRowColumnDefinitions()
        {
            RowDefinitions.Clear();
            ColumnDefinitions.Clear();
        }

        /// <summary>
        /// Evaluate <see cref="FillLastRow"/> and set the last row to <see cref="GridUnitType.Star"/> if true
        /// </summary>
        protected virtual void EvaluateFillLastRow()
        {
            //Add row to fill remainder of space
            if (FillLastRow && RowDefinitions.Count >= 1)
                RowDefinitions.Last().Height = new System.Windows.GridLength(AutoRowHeight.Value, System.Windows.GridUnitType.Star);
        }

        /// <summary>
        /// Evaluate <see cref="FillLastColumn"/> and set the last row to <see cref="GridUnitType.Star"/> if true
        /// </summary>
        protected virtual void EvaluateFillLastColumn()
        {
            if (FillLastColumn && ColumnDefinitions.Count >= 1)
                ColumnDefinitions.Last().Width = new GridLength(AutoColumnWidth.Value, GridUnitType.Star);
        }

    }
}
