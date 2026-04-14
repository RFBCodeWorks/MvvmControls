using System.Windows.Input;

namespace RFBCodeWorks.WPF
{
    /// <summary>
    /// Contains extensions for evaluating the Keyboard keys and <see cref="Key"/> objects
    /// </summary>
    public static class KeyboardExtensions  
    {
        /// <summary>
        /// Check if the <paramref name="key"/> is an Alphabet Key (A-Z)
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if the key has a value between A-Z, otherwise false</returns>
        public static bool IsAlphabetKey(this Key key)
        {
            return key >= Key.A && key <= Key.Z;
        }

        /// <summary>
        /// Check if the <paramref name="key"/> is a numeric Key (0-9)
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if the key has a value between 0-9, otherwise false</returns>
        public static bool IsNumericKey(this Key key)
        {
            return (key >= Key.D0 && key <= Key.D9) || (Keyboard.IsKeyToggled(Key.NumLock) && (key >= Key.NumPad0 && key <= Key.NumPad9));
        }

        /// <summary>
        /// Checks if the <paramref name="key"/> is a standard punctuation key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsPunctuation(this Key key)
        {
            switch (key)
            {
                case Key.OemPeriod:
                case Key.OemSemicolon:
                case Key.OemComma:
                case Key.OemQuestion:
                case Key.Decimal:
                    return true;
                default:
                    return key == Key.D1 && IsShiftHeld(); // exclamation key
            }
        }

        /// <summary>
        /// Check if the <paramref name="key"/> is an Alphabet Key (A-Z) or a numeric key (0-9)
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if the key has a value between A-Z or 0-9, otherwise false</returns>
        public static bool IsAlphaNumeric(this Key key) => IsAlphabetKey(key) || IsNumericKey(key);

        /// <summary>
        /// Determine if either SHIFT key is held down
        /// </summary>
        /// <returns> Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) </returns>
        public static bool IsShiftHeld() => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

        /// <summary>
        /// Determine if the CTRL key is held down
        /// </summary>
        /// <returns> Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl); </returns>
        public static bool IsCtrlHeld() => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

        /// <summary>
        /// Determine if the ALT key is held down
        /// </summary>
        /// <returns> Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt); </returns>
        public static bool IsAltHeld() => Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);

        /// <summary>
        /// Get the key that was pressed from the KeyEventArgs
        /// </summary>
        /// <param name="e">The key event args</param>
        /// <returns>
        /// IF <see cref="KeyEventArgs.Key"/> == <see cref="Key.System"/>: return <see cref="KeyEventArgs.SystemKey"/>
        /// <br/> otherwise return <see cref="KeyEventArgs.Key"/>
        /// </returns>
        public static Key GetKey(this KeyEventArgs e) => e.Key == Key.System ? e.SystemKey : e.Key;

        /// <summary>
        /// Determine if the key is one of the following special keys: <br/>
        /// - <see cref="Key.Back"/>
        /// - <see cref="Key.Capital"/>
        /// - <see cref="Key.Clear"/>
        /// - <see cref="Key.CrSel"/>
        /// - <see cref="Key.Delete"/>
        /// - <see cref="Key.End"/>
        /// - <see cref="Key.Enter"/>
        /// - <see cref="Key.Escape"/>
        /// - <see cref="Key.Help"/>
        /// - <see cref="Key.Home"/>
        /// - <see cref="Key.Insert"/>
        /// - <see cref="Key.RWin"/>
        /// - <see cref="Key.LWin"/>
        /// - <see cref="Key.OemBackslash"/>
        /// - <see cref="Key.PageDown"/>
        /// - <see cref="Key.PageUp"/>
        /// - <see cref="Key.Print"/>
        /// - <see cref="Key.PrintScreen"/>
        /// - <see cref="Key.Return"/>
        /// - <see cref="Key.System"/>
        /// </summary>
        /// <returns> True if the key is typically a special function, otherwise false </returns>
        public static bool IsSpecialSystemKey(this Key key)
        {
            switch (key)
            {
                case Key.Back:
                case Key.Capital:
                case Key.Clear:
                case Key.OemClear:
                case Key.CrSel:
                case Key.Delete:
                case Key.End:
                case Key.Enter:
                case Key.Escape:
                case Key.Help:
                case Key.Home:
                case Key.Insert:
                case Key.RWin:
                case Key.LWin:
                case Key.OemBackslash:
                case Key.PageDown:
                case Key.PageUp:
                case Key.Print:
                case Key.PrintScreen:
                case Key.NumLock:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Check if the pressed key is any of the F# keys
        /// </summary>
        /// <param name="key"></param>
        /// <returns>TRUE if it is a F# key (such as <see cref="Key.F1"/>), otherwise false.</returns>
        public static bool IsFunctionKey(this Key key)
        {
            switch (key)
            {
                case Key.F1:
                case Key.F2:
                case Key.F3:
                case Key.F4:
                case Key.F5:
                case Key.F6:
                case Key.F7:
                case Key.F8:
                case Key.F9:
                case Key.F10:
                case Key.F11:
                case Key.F12:
                case Key.F13:
                case Key.F14:
                case Key.F15:
                case Key.F16:
                case Key.F17:
                case Key.F18:
                case Key.F19:
                case Key.F20:
                case Key.F21:
                case Key.F22:
                case Key.F23:
                case Key.F24:
                    return true;
                default:
                    return false;
            }
        }
    }
}
