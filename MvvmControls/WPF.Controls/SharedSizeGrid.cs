using System;
using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.WPF.Controls
{
    /// <summary>
    /// Grid control that can be set up to automatically share another grid's Column and Row definitions via the SharedSizeGroup property
    /// </summary>
    public class SharedSizeGrid : Grid
    {

        /// <summary>
        /// Provide a grid to share the size of the Row and Column Definitions
        /// </summary>
        public Grid RowAndColumnSource
        {
            get { return (Grid)GetValue(RowAndColumnSourceProperty); }
            set
            {
                SetValue(RowAndColumnSourceProperty, value);
            }
        }

        /// <inheritdoc cref="RowAndColumnSource"/>
        public static readonly DependencyProperty RowAndColumnSourceProperty =
            DependencyProperty.Register(nameof(RowAndColumnSource), typeof(Grid), typeof(SharedSizeGrid), new PropertyMetadata(null, RowAndColumnSourceChanged));


        /// <summary>
        /// Provide a grid whose RowDefinitions will be cloned
        /// </summary>
        public Grid RowSource
        {
            get { return (Grid)GetValue(RowSourceProperty); }
            set { SetValue(RowSourceProperty, value); }
        }

        /// <inheritdoc cref="RowSource"/>
        public static readonly DependencyProperty RowSourceProperty =
            DependencyProperty.Register(nameof(RowSource), typeof(Grid), typeof(SharedSizeGrid), new PropertyMetadata(null, propertyChangedCallback: RowSourceChanged));


        /// <summary>
        /// Provide a grid whose ColumnDefinitions will be cloned
        /// </summary>
        public Grid ColumnSource
        {
            get { return (Grid)GetValue(ColumnSourceProperty); }
            set { SetValue(ColumnSourceProperty, value); }
        }

        /// <inheritdoc cref="ColumnSource"/>
        public static readonly DependencyProperty ColumnSourceProperty =
            DependencyProperty.Register(nameof(ColumnSource), typeof(Grid), typeof(SharedSizeGrid), new PropertyMetadata(null, ColumnSourceChanged));

        private static void RowAndColumnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisGrid = d as SharedSizeGrid;
            if (thisGrid is null) return;
            thisGrid.ColumnSource = e.NewValue as Grid;
            thisGrid.RowSource = e.NewValue as Grid;
        }

        private static void RowSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisGrid = d as SharedSizeGrid;
            if (thisGrid != null && e.OldValue is Grid oldGrid)
            {
                //oldGrid.SizeChanged -= thisGrid.RowSourceUpdated;
            }
            if (thisGrid != null && e.OldValue is Grid newGrid)
            {
                //newGrid.SizeChanged += thisGrid.RowSourceUpdated;
            }
            thisGrid?.RowSourceUpdated(thisGrid.RowSource, null);
        }

        private static void ColumnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisGrid = d as SharedSizeGrid;
            if (thisGrid != null && e.OldValue is Grid oldGrid)
            {
                //oldGrid.SizeChanged -= thisGrid.ColumnSouceUpdated;
            }
            if (thisGrid != null && e.OldValue is Grid newGrid)
            {
                //newGrid.SizeChanged += thisGrid.ColumnSouceUpdated;
            }
            thisGrid?.ColumnSouceUpdated(thisGrid.ColumnSource, null);
        }

        private void RowSourceUpdated(object sender, EventArgs e)
        {
            var grid = sender as Grid;
            if (grid != RowSource) return;
            this.RowDefinitions.Clear();
            foreach (RowDefinition def in RowSource.RowDefinitions)
            {
                var newDef = new RowDefinition();
                if (string.IsNullOrEmpty(def.SharedSizeGroup))
                {
                    newDef.MinHeight = def.MinHeight;
                    newDef.MaxHeight = def.MaxHeight;
                    newDef.Height = def.Height;
                }
                else
                {
                    newDef.SharedSizeGroup = def.SharedSizeGroup;
                }
                this.RowDefinitions.Add(newDef);
            }
        }

        private void ColumnSouceUpdated(object sender, EventArgs e)
        {
            var grid = sender as Grid;
            if (grid != ColumnSource) return;
            this.ColumnDefinitions.Clear();
            foreach (ColumnDefinition def in ColumnSource.ColumnDefinitions)
            {
                var newDef = new ColumnDefinition();
                if (string.IsNullOrEmpty(def.SharedSizeGroup))
                {
                    newDef.MinWidth = def.MinWidth;
                    newDef.MaxWidth = def.MaxWidth;
                    newDef.Width = def.Width;
                }
                else
                {
                    newDef.SharedSizeGroup = def.SharedSizeGroup;
                }
                this.ColumnDefinitions.Add(newDef);
            }
        }
    }
}
