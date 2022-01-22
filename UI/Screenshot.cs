namespace Memorandum.UI
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    public class Screenshot : IDisposable
    {
        private bool disposedValue;

        public Screenshot(Rectangle bounds)
        {
            var bitmap = this.CopyFrom(IntPtr.Zero, bounds);
            this.Image = bitmap;
            this.Timestamp = DateTime.Now;
        }

        public Screenshot(IntPtr hwnd)
        {
            var bitmap = this.CopyFrom(hwnd, Rectangle.Empty);
            this.Image = bitmap;
            this.Timestamp = DateTime.Now;
        }

        public Screenshot(IntPtr hwnd, Rectangle bounds)
        {
            var bitmap = this.CopyFrom(hwnd, bounds);
            this.Image = bitmap;
            this.Timestamp = DateTime.Now;
        }

        public Screenshot(Window window, bool clientOnly, bool layerBlend)
        {
            var bitmap = this.CopyFrom(window, clientOnly, layerBlend);
            this.Image = bitmap;
            this.Timestamp = DateTime.Now;
        }

        ~Screenshot()
        {
            this.Dispose(false);
        }

        public Bitmap Image { get; private set; }

        public DateTime Timestamp { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected Bitmap CopyFrom(IntPtr hwnd, Rectangle bounds)
        {
            var dcFrom = default(IntPtr);
            var dcTo = default(IntPtr);
            var bitmap = default(Bitmap);
            var g = default(Graphics);

            if (bounds == Rectangle.Empty)
            {
                bounds = new Window(hwnd).Bounds;
            }

            try
            {
                bitmap = new Bitmap(bounds.Width, bounds.Height);
                g = Graphics.FromImage(bitmap);
                dcFrom = NativeMethods.GetDC(hwnd);
                dcTo = g.GetHdc();
                var flags = CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt;
                NativeMethods.BitBlt(dcTo, 0, 0, bitmap.Width, bitmap.Height, dcFrom, bounds.Left, bounds.Top, flags);
            }
            finally
            {
                NativeMethods.ReleaseDC(hwnd, dcFrom);
                if (g != null)
                {
                    g.ReleaseHdc(dcTo);
                    g.Dispose();
                }
            }

            return bitmap;
        }

        protected Bitmap CopyFrom(Window w, bool clientOnly, bool layerBlend)
        {
            var dcFrom = default(IntPtr);
            var dcTo = default(IntPtr);
            var bitmap = default(Bitmap);
            var g = default(Graphics);
            var hwnd = default(IntPtr);

            Func<IntPtr, IntPtr> getDC = NativeMethods.GetDC;

            try
            {
                var bounds = default(Rectangle);
                if (clientOnly)
                {
                    if (layerBlend)
                    {
                        var p = default(Point);
                        NativeMethods.ClientToScreen(w.Handle, out p);
                        bounds = new Rectangle(p, w.ClientRectangle.Size);
                    }
                    else
                    {
                        hwnd = w.Handle;
                        bounds = w.ClientRectangle;
                    }
                }
                else
                {
                    if (layerBlend)
                    {
                        bounds = w.Bounds;
                    }
                    else
                    {
                        hwnd = w.Handle;
                        bounds = new Rectangle(
                            new Point(w.Bounds.Left - w.Rectangle.Left, w.Bounds.Top - w.Rectangle.Top),
                            w.Bounds.Size);
                        getDC = NativeMethods.GetWindowDC;
                    }
                }

                bitmap = new Bitmap(bounds.Width, bounds.Height);
                g = Graphics.FromImage(bitmap);
                dcFrom = getDC.Invoke(hwnd);
                dcTo = g.GetHdc();
                var flags = CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt;
                NativeMethods.BitBlt(dcTo, 0, 0, bitmap.Width, bitmap.Height, dcFrom, bounds.Left, bounds.Top, flags);
            }
            finally
            {
                NativeMethods.ReleaseDC(hwnd, dcFrom);
                if (g != null)
                {
                    g.ReleaseHdc(dcTo);
                    g.Dispose();
                }
            }

            return bitmap;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                }

                this.disposedValue = true;
            }
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool ClientToScreen(IntPtr hwnd, out Point p);

            [DllImport("user32.dll")]
            public static extern IntPtr GetDC(IntPtr handle);

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr handle);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ReleaseDC(IntPtr handle, IntPtr dc);

            [DllImport("gdi32.dll")]
            public static extern int BitBlt(IntPtr dcTo, int x1, int y1, int width, int height, IntPtr dcFrom, int x2, int y2, CopyPixelOperation flags);
        }
    }
}