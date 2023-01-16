#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmDialogs.FrameworkDialogs.MessageBox;
using DialogResult = System.Windows.MessageBoxResult;
using MessageBoxButtons = System.Windows.MessageBoxButton;
using MessageBoxIcon= System.Windows.MessageBoxImage;
using MessageBoxOptions = System.Windows.MessageBoxOptions;
using BaseMsg = MvvmDialogs.FrameworkDialogs.MessageBox.MessageBoxSettings;

namespace RFBCodeWorks.Mvvm.DialogServices
{

    /// <summary>
    /// Class that is used to display messages to the user
    /// </summary>
    public class MessageBoxSettings : BaseMsg
    {
        /// <summary>
        /// Instantiate a new message
        /// </summary>
        public MessageBoxSettings() : base() { }

        /// <summary>
        /// Instantiate a new message with the provided text and caption
        /// </summary>
        /// <param name="text">The message text to display to the user</param>
        /// <param name="caption">The title of the message box</param>
        /// <param name="buttons">The buttons to display on the message box</param>
        /// <param name="icon">The icon to display on the message box</param>
        /// <param name="defaultResult">The default result of the message box to be returned if the user clicks the 'X' button to close the window</param>
        /// <inheritdoc cref="BaseMsg.MessageBoxSettings"/>
        public MessageBoxSettings(
            string text,
            string caption,
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.None,
            DialogResult defaultResult = DialogResult.None
            //,MessageBoxOptions options = MessageBoxOptions.None
            ) : base()
        {
            base.MessageBoxText = text;
            base.Caption = caption;
            base.Button = buttons;
            base.Icon = icon;
            base.DefaultResult = defaultResult;
            //base.Options = options;
        }


        /// <summary>
        /// Show the dialog.
        /// </summary>
        /// <remarks>
        /// If not overridden by a derived class, default functionality uses <see cref="System.Windows.MessageBox.Show(string, string, MessageBoxButtons, MessageBoxIcon, DialogResult, MessageBoxOptions)"/>
        /// <br/>This will show the window non-modally, and disconnected from the <paramref name="parent"/>.
        /// <br/><br/>Best practice would be to create a new derived class within your project that overrides this method to use your project's <see cref="DialogService"/> to show the message.
        /// </remarks>
        /// <param name="parent">The Parent ViewModel that caused this message to show</param>
        /// <returns><inheritdoc cref="System.Windows.MessageBox.Show(string)"/></returns>
        public virtual DialogResult Show(IViewModel parent)
        {
            MvvmDialogs.IDialogService service = null;
            try
            {
                //Attempt to locate a dialog service
                service = DialogServiceLocator.DefaultLocator.GetDialogService(parent);
            }
            catch { }

            if (service != null)
                return service.ShowMessageBox(parent, this);
            else
                return System.Windows.MessageBox.Show(this.MessageBoxText, this.Caption, this.Button, this.Icon, this.DefaultResult, this.Options);
        }

        /// <inheritdoc cref="Show(IViewModel, string, string, MessageBoxButtons, MessageBoxIcon, DialogResult)"/>
        public static DialogResult ShowOKOnly(IViewModel owner, string text, string caption = "", MessageBoxIcon icon = MessageBoxIcon.None)
            => Show(owner, text, caption, buttons: MessageBoxButtons.OK, icon: icon, defaultResult: DialogResult.None);

        /// <inheritdoc cref="Show(IViewModel, string, string, MessageBoxButtons, MessageBoxIcon, DialogResult)"/>
        public static DialogResult ShowYesNo(IViewModel owner, string text, string caption = "", DialogResult defaultResult = DialogResult.No, MessageBoxIcon icon = MessageBoxIcon.Question)
            => Show(owner, text, caption, buttons: MessageBoxButtons.YesNo, icon: icon, defaultResult: defaultResult);

        /// <inheritdoc cref="Show(IViewModel, string, string, MessageBoxButtons, MessageBoxIcon, DialogResult)"/>
        public static DialogResult ShowYesNoCancel(IViewModel owner, string text, string caption = "", DialogResult defaultResult = DialogResult.No, MessageBoxIcon icon = MessageBoxIcon.Question)
            => Show(owner, text, caption, buttons: MessageBoxButtons.YesNoCancel, icon: icon, defaultResult: defaultResult);

        /// <summary>
        /// Create and show a new Message
        /// </summary>
        /// <returns>The Dialog Result</returns>
        /// <inheritdoc cref="MessageBoxSettings.MessageBoxSettings(string, string, MessageBoxButtons, MessageBoxIcon, DialogResult)"/>
        public static DialogResult Show(IViewModel owner, string text, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.YesNo, MessageBoxIcon icon = MessageBoxIcon.Question, DialogResult defaultResult = DialogResult.None)
        {
            return new MessageBoxSettings(text, caption, buttons, icon, defaultResult).Show(owner);
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member