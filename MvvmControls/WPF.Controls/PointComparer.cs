using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RFBCodeWorks.WPF.Controls
{
    /// <summary>
    /// EqualityComparer for <see cref="Point"/> structs
    /// </summary>
    public class PointComparer : EqualityComparer<Point>
    {
        /// <summary>
        /// Singleton of the PointComparer class
        /// </summary>
        public static PointComparer Comparer { get; } = new();

        /// <summary>
        /// Check if the two points are equal
        /// </summary>
        /// <returns>TRUE of both are equal, otherwise false</returns>
        public override bool Equals(Point x, Point y)
        {
            return x.X == y.X && x.Y == y.Y;
        }

        /// <inheritdoc/>
        public override int GetHashCode(Point obj)
        {
            return obj.GetHashCode();
        }
    }
}
