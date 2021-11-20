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
            var bitmap = new Bitmap(bounds.Width, bounds.Height);
            var g = Graphics.FromImage(bitmap);
            var dcFrom = NativeMethods.GetDC(IntPtr.Zero);
            var dcTo = g.GetHdc();

            try
            {
                var flags = CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt;
                NativeMethods.BitBlt(dcTo, 0, 0, bitmap.Width, bitmap.Height, dcFrom, bounds.Left, bounds.Top, flags);
            }
            finally
            {
                NativeMethods.ReleaseDC(IntPtr.Zero, dcFrom);
                g.ReleaseHdc(dcTo);
                g.Dispose();
            }

            this.Image = bitmap;
            this.Timestamp = DateTime.Now;
        }

        public Screenshot(Window window)
        {
            var rect = window.ClientRectangle;
            var bitmap = new Bitmap(rect.Width, rect.Height);
            var g = Graphics.FromImage(bitmap);
            var dcFrom = NativeMethods.GetDC(window.Handle);
            var dcTo = g.GetHdc();

            try
            {
                var flags = CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt;
                NativeMethods.BitBlt(dcTo, 0, 0, bitmap.Width, bitmap.Height, dcFrom, rect.Left, rect.Top, flags);
            }
            finally
            {
                NativeMethods.ReleaseDC(window.Handle, dcFrom);
                g.ReleaseHdc(dcTo);
                g.Dispose();
            }

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