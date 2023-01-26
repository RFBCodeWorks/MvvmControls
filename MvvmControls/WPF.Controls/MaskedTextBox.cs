using RFBCodeWorks.WPF.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace RFBCodeWorks.WPF.Controls
{
    /// <summary>
    /// Derived from Textbox, introduces a Mask parameter
    /// </summary>
    public class MaskedTextBox : TextBox
    {
        static MaskedTextBox()
        {

        }

        /// <inheritdoc/>
        public MaskedTextBox()
        {
            DataObject.AddPastingHandler(this, OnPaste);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.StringFormat, true);
            if (!isText) return;
            
            var text = e.SourceDataObject.GetData(DataFormats.StringFormat) as string;
            int i = 0;
            int lastPos = SelectionStart;
            int sl = SelectionLength;
            for (i = 0; i < text.Length; i++)
            {
                char nextchar = text[i];
                int nextPos = MaskProvider.FindEditPositionFrom(lastPos, true);
                int nextImmutable = MaskProvider.FindNonEditPositionFrom(lastPos, true);
                if (MaskProvider.VerifyChar(nextchar, nextPos, out _))
                {
                    bool success = false;
                    if (sl > 0)
                    {
                        success = MaskProvider.Replace(nextchar, nextPos);
                        sl--;
                    }
                    else
                    {
                        success = MaskProvider.InsertAt(nextchar, nextPos);
                    }
                    if (success)
                    {
                        lastPos = nextPos + 1;
                        SelectionStart++;
                        CaretIndex++;
                    }
                }
                //If next pasted character is equal to the next immutable mask character, move to the following character --> 990.990.990.990 --> pasting '1.2.3' results in 1__.2__.3__.___
                else if (nextImmutable >= 0 && nextchar == MaskProvider.Mask[nextImmutable])
                {
                    lastPos = MaskProvider.FindEditPositionFrom(nextImmutable, true);
                }
            }
            Text = MaskProvider.ToDisplayString();
            e.Handled = true;
            e.CancelCommand();
        }

        /// <summary>
        /// The raw text value (unmasked) text
        /// </summary>
        public string RawText
        {
            get { return (string)GetValue(RawTextProperty); }
            set { SetValue(RawTextProperty, value); }
        }

        /// <inheritdoc cref="MaskProvider"/>     
        public static readonly DependencyProperty RawTextProperty =
            DependencyProperty.Register(nameof(RawText), typeof(string), typeof(MaskedTextBox), new PropertyMetadata(null, RawTextChanged));

        private static void RawTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var t = d as MaskedTextBox;
            var provider = t.MaskProvider;
            if (provider is null)
                t.Text = e.NewValue as string;
            else
            {
                if (provider.Set(e.NewValue as string))
                    t.Text = provider.ToDisplayString();
            }
        }

        /// <summary>
        /// Gets/Sets the mask to use with the textbox
        /// </summary>
        /// <remarks>
        /// Mask Creation: <br/>
        /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.maskedtextbox.mask?view=windowsdesktop-7.0&amp;viewFallbackFrom=net-7.0#system-windows-forms-maskedtextbox-mask"/>
        /// </remarks>
        public string Mask
        {
            get { return MaskProvider?.Mask; }
            set { SetMaskProperty(this, value); }
        }

        /// <summary>
        /// Gets the Mask from the MaskedTextProvider assigned to the MaskedtextBox
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static object GetMaskProperty(DependencyObject d) => (d.GetValue(MaskProviderProperty) as MaskedTextProvider)?.Mask;

        /// <inheritdoc cref="MaskedTextProvider.MaskedTextProvider(string)"/>
        /// <inheritdoc cref="Mask"/>
        public static void SetMaskProperty(DependencyObject d, object mask)
        {
            if (string.IsNullOrWhiteSpace(mask as string))
                d.SetValue(MaskProviderProperty, null);
            else
                d.SetValue(MaskProviderProperty, new MaskedTextProvider((string)mask));
        }

        /// <summary>
        /// Gets/Sets the mask to use with the textbox
        /// </summary>
        public MaskedTextProvider MaskProvider
        {
            get { return (MaskedTextProvider)GetValue(MaskProviderProperty); }
            set { SetValue(MaskProviderProperty, value); }
        }

        /// <inheritdoc cref="MaskProvider"/>     
        public static readonly DependencyProperty MaskProviderProperty =
            DependencyProperty.Register(nameof(MaskProvider), typeof(MaskedTextProvider), typeof(MaskedTextBox), new PropertyMetadata(null, MaskedProviderChanged));

        private static void MaskedProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Apply the current text to the new provider
            if (e.NewValue is MaskedTextProvider provider)
            {
                if (e.OldValue != null)
                {
                    provider.Set((e.OldValue as MaskedTextProvider).ToString());
                }
                d.SetValue(TextProperty, provider.ToDisplayString());
            }
        }

        /// <inheritdoc/>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            MaskedTextProvider provider = MaskProvider;
            int position = provider.FindEditPositionFrom(SelectionStart, true);
            
            bool charactervalid = provider.VerifyChar(e.Text.Single(), position, out _);
            int nextImmutable = provider.FindNonEditPositionFrom(position, true);
            bool isNextImmutable = nextImmutable >=0 && provider.Mask[nextImmutable] == e.Text[0];
            
            if (!charactervalid && !isNextImmutable) goto Handled;
            if (isNextImmutable)
                position = provider.FindEditPositionFrom(nextImmutable, true);

            _ = RemoveSelected();
            string txt = provider.ToDisplayString();
            // inserting a character
            if (position < txt.Length)
            {

                if (Keyboard.IsKeyToggled(Key.Insert))
                {
                    if (provider.Replace(e.Text, position))
                        position++;
                }
                else
                {
                    if (provider.InsertAt(e.Text, position))
                        position++;
                }
            }
            //Starting with empty text or appending to last position
            else if (position >= txt.Length)
            {
                if (provider.Add(e.Text.Single()))
                    position++;
            }

            RefreshText(provider, provider.FindEditPositionFrom(position, true)); ;
        Handled:
            e.Handled = true;
            base.OnPreviewTextInput(e);
        }

        /// <inheritdoc/>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            int editPos = CaretIndex;
            switch (e.Key)
            {
                case Key.Back:
                    if (!RemoveSelected())
                    {
                        editPos = MaskProvider.FindEditPositionFrom(CaretIndex, false);
                        editPos = editPos == CaretIndex ? MaskProvider.FindEditPositionFrom(CaretIndex - 1, false) : editPos;
                        MaskProvider.RemoveAt(editPos);
                    }
                    //editPos = MaskProvider.FindEditPositionFrom(editPos, false);
                    e.Handled = true;
                    break;
                case Key.Delete:
                    if (!RemoveSelected())
                    {
                        editPos = MaskProvider.FindEditPositionFrom(CaretIndex, true);
                        MaskProvider.RemoveAt(editPos);
                    }
                    e.Handled = true;
                    break;
            }
            if (e.Handled)
                RefreshText(MaskProvider, editPos);
            base.OnPreviewKeyDown(e);
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        private void RefreshText(MaskedTextProvider provider, int newCursorPosition)
        {
            Text = provider.ToDisplayString();
            CaretIndex = newCursorPosition < 0 | newCursorPosition > Text.Length - 1 ? Text.Length : newCursorPosition;
        }

        /// <inheritdoc/>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
        }

        /// <summary>
        /// Evaluates the current selected text, and removes it using the current index. Does not update the Text property. 
        /// </summary>
        /// <returns>
        /// TRUE if any items characters were removed, otherwise false.
        /// </returns>
        protected bool RemoveSelected()
        {
            var provider = MaskProvider;
            if (provider is null) return false;

            var length = SelectionLength;
            if (length >= provider.Length)
            {
                provider.Clear();
                return true;
            }
            else if (length > 0)
            {
                int position = provider.FindEditPositionFrom(SelectionStart, true);
                if (position < 0) return false; //Failure to find a valid position to remove from
                int i = 0;
                while (i < length)
                {
                    provider.RemoveAt(position);
                    i++;
                }
                return true;
            }
            return false;
        }

    }
}
