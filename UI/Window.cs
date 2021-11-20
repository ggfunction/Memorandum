namespace Memorandum.UI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.InteropServices;

    public class Window
    {
        public Window(IntPtr handle)
        {
            this.Handle = handle;
            this.Update();
        }

        public Rectangle Bounds { get; private set; }

        public string ClassName { get; private set; }

        public Rectangle ClientRectangle { get; private set; }

        public IntPtr Handle { get; private set; }

        public bool Iconic { get; private set; }

        public Point Location { get; private set; }

        public IntPtr Parent { get; private set; }

        public IntPtr Root { get; private set; }

        public IntPtr RootOwner { get; private set; }

        public int ProcessId { get; private set; }

        public Rectangle Rectangle { get; private set; }

        public string Text { get; private set; }

        public DateTime Timestamp { get; private set; }

        public bool Visible { get; private set; }

        public static IEnumerable<Window> Enumerate()
        {
            return NativeMethods.EnumWindows();
        }

        public static IEnumerable<Window> Enumerate(IntPtr handle)
        {
            return NativeMethods.EnumWindows(handle);
        }

        public static IEnumerable<Window> Enumerate(Window window)
        {
            return NativeMethods.EnumWindows(window.Handle);
        }

        public void Update()
        {
            this.Bounds = NativeMethods.GetWindowBounds(this.Handle);
            this.ClassName = NativeMethods.GetClassName(this.Handle);
            this.ClientRectangle = NativeMethods.GetClientRect(this.Handle);
            this.Iconic = NativeMethods.IsIconic(this.Handle);
            this.Location = this.Bounds.Location;
            this.Parent = NativeMethods.GetParent(this.Handle);
            this.Rectangle = NativeMethods.GetWindowRect(this.Handle);
            this.Root = NativeMethods.GetRoot(this.Handle);
            this.RootOwner = NativeMethods.GetRootOwner(this.Handle);
            this.Text = NativeMethods.GetWindowText(this.Handle);
            this.Timestamp = DateTime.Now;
            this.Visible = NativeMethods.IsWindowVisible(this.Handle);

            int processId;
            NativeMethods.GetWindowThreadProcessId(this.Handle, out processId);
            this.ProcessId = processId;
        }

        private class NativeMethods
        {
            private delegate bool EnumWindowsProc(IntPtr handle, IntPtr lParam);

            [Flags]
            private enum DwmWindowAttributes : int
            {
                ExtendedFrameBounds = 9,
            }

            private enum GetAncestorFlags : int
            {
                Parent = 1,

                Root = 2,

                RootOwner = 3,
            }

            public static IEnumerable<Window> EnumWindows()
            {
                var list = new List<Window>();
                var gc = GCHandle.Alloc(list);
                try
                {
                    NativeMethods.EnumWindows(EnumWindowsCallback, GCHandle.ToIntPtr(gc));
                    return list;
                }
                finally
                {
                    gc.Free();
                }
            }

            public static IEnumerable<Window> EnumWindows(IntPtr handle)
            {
                var list = new List<Window>();
                var gc = GCHandle.Alloc(list);
                try
                {
                    NativeMethods.EnumChildWindows(handle, EnumWindowsCallback, GCHandle.ToIntPtr(gc));
                    return list;
                }
                finally
                {
                    gc.Free();
                }
            }

            public static string GetClassName(IntPtr handle)
            {
                const int MaxCount = 260;
                var sb = new System.Text.StringBuilder(MaxCount);
                NativeMethods.GetClassName(handle, sb, sb.Capacity);
                return sb.ToString();
            }

            public static Rectangle GetClientRect(IntPtr handle)
            {
                Rect rect;
                NativeMethods.GetClientRect(handle, out rect);
                return rect.ToRectangle();
            }

            public static IntPtr GetParent(IntPtr handle)
            {
                return NativeMethods.GetAncestor(handle, GetAncestorFlags.Parent);
            }

            public static IntPtr GetRoot(IntPtr handle)
            {
                return NativeMethods.GetAncestor(handle, GetAncestorFlags.Root);
            }

            public static IntPtr GetRootOwner(IntPtr handle)
            {
                return NativeMethods.GetAncestor(handle, GetAncestorFlags.RootOwner);
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

            public static string GetWindowText(IntPtr handle)
            {
                var length = NativeMethods.GetWindowTextLength(handle);
                var sb = new System.Text.StringBuilder(length + 1);
                NativeMethods.GetWindowText(handle, sb, sb.Capacity);
                return sb.ToString();
            }

            [DllImport("user32.dll")]
            public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsIconic(IntPtr handle);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsWindowVisible(IntPtr handle);

            private static bool EnumWindowsCallback(IntPtr handle, IntPtr lParam)
            {
                var gc = GCHandle.FromIntPtr(lParam);
                var list = gc.Target as List<Window>;
                list.Add(new Window(handle));
                return true;
            }

            [DllImport("dwmapi.dll")]
            private static extern int DwmGetWindowAttribute(IntPtr handle, DwmWindowAttributes attribute, out Rect rect, int size);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr lParam);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool EnumChildWindows(IntPtr handle, EnumWindowsProc callback, IntPtr lParam);

            [DllImport("user32.dll")]
            private static extern IntPtr GetAncestor(IntPtr handle, GetAncestorFlags flags);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            private static extern int GetClassName(IntPtr handle, System.Text.StringBuilder text, int maxCount);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetClientRect(IntPtr handle, out Rect rect);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetWindowRect(IntPtr handle, out Rect rect);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            private static extern int GetWindowText(IntPtr handle, System.Text.StringBuilder text, int maxCount);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            private static extern int GetWindowTextLength(IntPtr handle);

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