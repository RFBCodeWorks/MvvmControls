using System;
using System.Collections.Generic;
using System.Linq;

namespace RFBCodeWorks.Mvvvm.XmlLinq.Controls
{

    /// <summary>
    /// ComboBox that stores a selected string value into an <see cref="IXValueObject"/>
    /// </summary>
    public class XComboBox : XComboBox<string>, IComboBox<string>
    {
        /// <inheritdoc/>
        public XComboBox(IXValueObject xValueProvider) : base(xValueProvider) { }
    }

    /// <summary>
    /// ComboBox that stores a selected value into an <see cref="IXValueObject"/>
    /// </summary>
    public class XComboBox<T> : XComboBox<T, object>, IComboBox<T>
    {
        /// <inheritdoc/>
        public XComboBox(IXValueObject xValueProvider) : base(xValueProvider) { }
    }

    /// <summary>
    /// ComboBox that stores a selected value into an <see cref="IXValueObject"/>
    /// </summary>
    public class XComboBox<T,V> : XComboBox<T, IList<T>, V>, IComboBox<T>
    {
        /// <inheritdoc/>
        public XComboBox(IXValueObject xValueProvider) : base(xValueProvider) { }
    }

    /// <summary>
    /// ComboBox that provides a list of some type, then stores the converted value of the selected item into an <see cref="IXValueObject"/>
    /// </summary>
    public class XComboBox<T, E, V> : ComboBoxDefinition<T,E, V>, IXValueControl where E: IList<T>
    {
        /// <summary>
        /// Create a new ComboBox that will store the SelectedItem into the <paramref name="xValueProvider"/>
        /// </summary>
        /// <param name="xValueProvider"></param>
        public XComboBox(IXValueObject xValueProvider) : base()
        {
            base.SelectedItemChanged += OnSelectedItemChanged;
            NodeValueSetter = xValueProvider;
            NodeValueSetter.Added += OnNodeChanged;
            NodeValueSetter.Removed += OnNodeChanged;
            NodeValueSetter.ValueChanged += NodeValueSetter_ValueChanged;
            
        }

        /// <inheritdoc/>
        public event EventHandler NodeChanged;

        /// <summary>
        /// The object that handles setting the value of the underlying XML Node
        /// </summary>
        public IXValueObject NodeValueSetter { get; }

        /// <summary>
        /// If TRUE, store the index of the item within the list, instead of the value of the item. Default is false.
        /// </summary>
        public bool StoreValueAsItemIndex { get; set; }

        /// <inheritdoc/>
        public override string Text
        {
            get => base.Text;
            set { }
        }

        /// <inheritdoc/>
        public sealed override bool IsEditable { get => false; set { } }

        /// <summary>
        /// Occurs when the Text property is updating on the node
        /// </summary>
        protected bool IsValueUpdating { get; private set; }

        /// <summary>
        /// A function that converts the selected item to a string value to store into the XML node.
        /// <br/> If not provided, will use object.ToString().
        /// </summary>
        public Func<T,string> ItemConverter { get; init; }

        /// <summary>
        /// React to the value being updated on the node
        /// </summary>
        protected virtual void NodeValueSetter_ValueChanged() 
        {
            if (Items is null) return;
            if (StoreValueAsItemIndex)
            {
                if (int.TryParse(NodeValueSetter.Value, out int value) && value >= 0 && value < (Items?.Count ?? 0))
                {
                    SelectedIndex = value;
                }
                else
                {
                    SelectedIndex = -1;
                }
            }
            else
            {
                string val = NodeValueSetter.Value;
                try
                {
                    SelectedItem = Items.Single(t => ItemConverter?.Invoke(t) == val);
                }
                catch
                {
                    SelectedIndex = -1;
                    Text = val;
                }
            }
        }

        /// <summary>
        /// React to the node being added/removed from the tree
        /// </summary>
        protected virtual void OnNodeChanged(object sender, EventArgs e)
        {
            IsEnabled = NodeValueSetter.CanBeCreated;
            OnNodeChanged();
        }

        /// <summary>
        /// Raises the <see cref="NodeChanged"/> event
        /// </summary>
        protected void OnNodeChanged()
        {
            NodeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void NodeValueSetter_ValueChanged(object sender, EventArgs e)
        {
            if (IsValueUpdating) return;
            NodeValueSetter_ValueChanged();
        }

        private void OnSelectedItemChanged(object sender, PropertyOfTypeChangedEventArgs<T> e)
        {
            IsValueUpdating = true;
            OnSelectedItemChanged();
            IsValueUpdating = false;
        }

        /// <summary>
        /// React to the Selected Item being changed
        /// </summary>
        protected virtual void OnSelectedItemChanged()
        {
            bool ValidItem = SelectedIndex >= 0;
            if (StoreValueAsItemIndex)
            {
                NodeValueSetter.Value = ValidItem ? SelectedIndex.ToString() : null;
            }
            else
            {
                if (SelectedItem is null)
                    NodeValueSetter.Value = null;
                else if (ItemConverter is null)
                    NodeValueSetter.Value = SelectedItem.ToString();
                else
                    NodeValueSetter.Value = ItemConverter.Invoke(SelectedItem);
            }
        }
    }
}
