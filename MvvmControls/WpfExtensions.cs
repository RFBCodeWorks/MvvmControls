using System;
using System.Windows;

namespace RFBCodeWorks.WPF
{
    /// <summary>
    /// Contains Extension Methods for Dependency Objects
    /// </summary>
    public static class WpfExtensions
    {
        //private static readonly DependencyObject DesignModeTester = new();
        
        /// <summary>
        /// Detect if in in the VS designer or not
        /// </summary>
        /// <returns></returns>
        public static bool IsDesignMode(this DependencyObject obj) => System.ComponentModel.DesignerProperties.GetIsInDesignMode(obj);
    }
}
