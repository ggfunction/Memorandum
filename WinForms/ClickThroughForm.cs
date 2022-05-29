namespace Memorandum.WinForms
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public partial class ClickThroughForm : Form
    {
        private bool allowAltF4;

        private bool clickThrough;

        private bool showSystemMenu;

        public ClickThroughForm()
            : base()
        {
            this.allowAltF4 = true;
            this.clickThrough = false;
            this.showSystemMenu = true;
        }

        public bool AllowAltF4
        {
            get
            {
                return this.allowAltF4;
            }

            set
            {
                this.SetAllowAltF4(value);
            }
        }

        public bool ClickThrough
        {
            get
            {
                return this.clickThrough && this.AllowTransparency;
            }

            set
            {
                this.SetClickThrough(value);
            }
        }

        public bool ShowSystemMenu
        {
            get
            {
                return this.showSystemMenu;
            }

            set
            {
                this.SetShowSystemMenu(value);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (!this.AllowAltF4 && NativeMethods.AltF4Contained(ref m))
            {
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        private void SetAllowAltF4(bool value)
        {
            this.allowAltF4 = value;
        }

        private void SetClickThrough(bool value)
        {
            this.AllowTransparency = value;
            NativeMethods.SetClickThrough(this.Handle, value);
            this.clickThrough = value;
        }

        private void SetShowSystemMenu(bool value)
        {
            NativeMethods.SetShowSystemMenu(this.Handle, value);
            this.showSystemMenu = value;
        }

        private static class NativeMethods
        {
            private enum WindowLongFlags
            {
                WndProc = -4,
                HInstance = -6,
                HWndParent = -8,
                Id = -12,
                Style = -16,
                ExStyle = -20,
            }

            private enum WindowMessages
            {
                SysKeyDown = 0x0104,
            }

            [Flags]
            private enum WindowStyles
            {
                SysMenu = 0x00080000,
                ExTransparent = 0x00000020,
                ExLayered = 0x00080000,
            }

            public static bool AltF4Contained(ref Message m)
            {
                return m.Msg == (int)WindowMessages.SysKeyDown &&
                    m.WParam.ToInt32() == (int)Keys.F4;
            }

            public static void SetClickThrough(IntPtr handle, bool value)
            {
                var extendedStyle = GetWindowLongPtr(handle, WindowLongFlags.ExStyle);

                if (value)
                {
                    extendedStyle |= WindowStyles.ExTransparent;
                }
                else
                {
                    extendedStyle &= ~WindowStyles.ExTransparent;
                }

                SetWindowLongPtr(handle, WindowLongFlags.ExStyle, extendedStyle);
            }

            public static void SetShowSystemMenu(IntPtr handle, bool value)
            {
                var windowStyle = GetWindowLongPtr(handle, WindowLongFlags.Style);

                if (value)
                {
                    windowStyle |= WindowStyles.SysMenu;
                }
                else
                {
                    windowStyle &= ~WindowStyles.SysMenu;
                }

                SetWindowLongPtr(handle, WindowLongFlags.Style, windowStyle);
            }

            [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
            private static extern WindowStyles GetWindowLongPtr32(IntPtr handle, WindowLongFlags index);

            [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
            private static extern WindowStyles GetWindowLongPtr64(IntPtr handle, WindowLongFlags index);

            [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
            private static extern WindowStyles SetWindowLongPtr32(IntPtr handle, WindowLongFlags index, WindowStyles newLong);

            [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
            private static extern WindowStyles SetWindowLongPtr64(IntPtr handle, WindowLongFlags index, WindowStyles newLong);

            private static WindowStyles GetWindowLongPtr(IntPtr handle, WindowLongFlags index)
            {
                if (IntPtr.Size == 4)
                {
                    return GetWindowLongPtr32(handle, index);
                }
                else
                {
                    return GetWindowLongPtr64(handle, index);
                }
            }

            private static void SetWindowLongPtr(IntPtr handle, WindowLongFlags index, WindowStyles newLong)
            {
                if (IntPtr.Size == 4)
                {
                    SetWindowLongPtr32(handle, index, newLong);
                }
                else
                {
                    SetWindowLongPtr64(handle, index, newLong);
                }
            }
        }
    }
}
