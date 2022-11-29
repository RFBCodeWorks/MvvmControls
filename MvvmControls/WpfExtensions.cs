using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace System.Windows
{
    /// <summary>
    /// Contains Extension Methods for Dependency Objects
    /// </summary>
    public static class WpfExtensions
    {
        private static readonly DependencyObject DesignModeTester = new();
        
        /// <summary>
        /// Detect if in in the VS designer or not
        /// </summary>
        /// <returns></returns>
        public static bool IsDesignMode(this DependencyObject obj) => System.ComponentModel.DesignerProperties.GetIsInDesignMode(obj);
    }
}
