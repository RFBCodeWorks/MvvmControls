using System;

#nullable enable

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// An exception that occurs when the refresh of a <see cref="RefreshableSelector{T, TList, TSelectedValue}"/> failes to refresh
    /// </summary>
    public class RefreshFailedException : Exception
    {
        /// <inheritdoc cref="Exception.Exception()"/>
        public RefreshFailedException() { }
        
        /// <inheritdoc cref="Exception.Exception(string)"/>
        public RefreshFailedException(string message) : base(message) { }

        /// <inheritdoc cref="Exception.Exception(string, Exception)"/>
        public RefreshFailedException(string message, Exception? innerException) : base(message, innerException) { }
    }
}
