namespace Memorandum.UI
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    public class Window
    {
        public Window(IntPtr handle)
        {
            this.Handle = handle;

            int processId;
            NativeMethods.GetWindowThreadProcessId(handle, out processId);
            this.ProcessId = processId;
        }

        public IntPtr Handle { get; private set; }

        public int ProcessId { get; private set; }

        private class NativeMethods
        {
            [Flags]
            private enum DwmWindowAttributes : int
            {
                ExtendedFrameBounds = 9,
            }

            public static Rectangle GetClientRect(IntPtr handle)
            {
                Rect rect;
                NativeMethods.GetClientRect(handle, out rect);
                return rect.ToRectangle();
            }

            public static Rectangle GetWindowBounds(IntPtr handle)
            {
                Rect bounds;
                NativeMethods.DwmGetWindowAttribute(handle, DwmWindowAttributes.ExtendedFrameBounds, out bounds, Marshal.SizeOf(typeof(Rect)));
                return bounds.ToRectangle();
            }

            public static Rectangle GetWindowRect(IntPtr handle)
            {
                Rect rect;
                NativeMethods.GetWindowRect(handle, out rect);
                return rect.ToRectangle();
            }

            [DllImport("user32.dll")]
            public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

            [DllImport("dwmapi.dll")]
            private static extern int DwmGetWindowAttribute(IntPtr handle, DwmWindowAttributes attribute, out Rect rect, int size);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetClientRect(IntPtr handle, out Rect rect);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetWindowRect(IntPtr handle, out Rect rect);

            [StructLayout(LayoutKind.Sequential)]
            private struct Rect
            {
                public Rect(int left, int top, int right, int bottom)
                    : this()
                {
                    this.Left = left;
                    this.Top = top;
                    this.Right = right;
                    this.Bottom = bottom;
                }

                public int Left { get; set; }

                public int Top { get; set; }

                public int Right { get; set; }

                public int Bottom { get; set; }

                public int Width
                {
                    get { return this.Right - this.Left; }
                }

                public int Height
                {
                    get { return this.Bottom - this.Top; }
                }

                public Rectangle ToRectangle()
                {
                    return new Rectangle(this.Left, this.Top, this.Width, this.Height);
                }
            }
        }
    }
}