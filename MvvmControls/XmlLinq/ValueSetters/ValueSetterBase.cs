using System;

namespace RFBCodeWorks.MvvmControls.XmlLinq.ValueSetters
{
    /// <summary>
    /// Abstract Base class for any XmlValueSetters 
    /// </summary>
    /// <typeparam name="T">The type of value to store into the IXValueObject node</typeparam>
    public abstract class ValueSetterBase<T> : ObservableObject
    {
        /// <summary>
        /// Set the ValueProvider
        /// </summary>
        /// <param name="xValueProvider"></param>
        protected ValueSetterBase(IXValueObject xValueProvider)
        {
            XValueProvider = xValueProvider ?? throw new ArgumentNullException(nameof(xValueProvider));
            XValueProvider.ValueChanged += XValueProvider_ValueChanged;
            XValueProvider.Added += XValueProvider_Added;
            XValueProvider.Removed += XValueProvider_Removed;
        }

        /// <summary>
        /// Occurs when the Value is updated
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// The object that provides XNode whose value will be updated
        /// </summary>
        public IXValueObject XValueProvider { get; }

        /// <summary>
        /// The value to set the node to
        /// </summary>
        public abstract T Value { get; set; }

        /// <summary>
        /// Protected flag for derived classes to utilize to avoid 
        /// </summary>
        protected bool IsSettingValue { get; set; }

        /// <summary>
        /// Raises the ValueChanged event
        /// </summary>
        protected virtual void OnValueChanged(EventArgs e = null)
        {
            ValueChanged?.Invoke(this, e ?? new());
            OnPropertyChanged(nameof(Value));
        }

        private void XValueProvider_ValueChanged(object sender, EventArgs e)
        {
            if (!IsSettingValue) XValueProvider_ValueChanged();
        }

        private void XValueProvider_Removed(object sender, EventArgs e)
        {
            if (!IsSettingValue) XValueProvider_Removed();
        }

        private void XValueProvider_Added(object sender, EventArgs e)
        {
            if (!IsSettingValue) XValueProvider_Added();
        }

        /// <summary>
        /// Occurs when the <see cref="IXValueObject.ValueChanged"/> event is raised by the <see cref="XValueProvider"/>
        /// </summary>
        protected abstract void XValueProvider_ValueChanged();

        /// <summary>
        /// Occurs when the <see cref="IXObjectProvider.Removed"/> event is raised by the <see cref="XValueProvider"/>
        /// </summary>
        protected abstract void XValueProvider_Removed();

        /// <summary>
        /// Occurs when the <see cref="IXObjectProvider.Added"/> event is raised by the <see cref="XValueProvider"/>
        /// </summary>
        protected abstract void XValueProvider_Added();

    }
}
