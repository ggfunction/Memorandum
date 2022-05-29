namespace Memorandum.UI
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class ClipboardListener : NativeWindow, IDisposable
    {
        private bool disposedValue;

        public ClipboardListener(IntPtr hwnd)
        {
            this.AssignHandle(hwnd);
            NativeMethods.AddClipboardFormatListener(this.Handle);
        }

        public ClipboardListener(Form form)
            : this(form.Handle)
        {
        }

        ~ClipboardListener()
        {
            this.Dispose(false);
        }

        public event EventHandler<ClipboardUpdatedEventArgs> ClipboardUpdated;

        public int ClipboardSequenceNumber { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override void WndProc(ref Message m)
        {
            const int WMClipboardUpdate = 0x031D;
            if (m.Msg == WMClipboardUpdate)
            {
                var latest = NativeMethods.GetClipboardSequenceNumber();
                if (this.ClipboardSequenceNumber != latest)
                {
                    using (var e = new ClipboardUpdatedEventArgs(
                        Clipboard.GetDataObject(),
                        latest))
                    {
                        this.OnClipboardUpdated(e);
                    }

                    this.ClipboardSequenceNumber = latest;
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                var hwnd = this.Handle;

                if (disposing)
                {
                    this.ReleaseHandle();
                }

                NativeMethods.RemoveClipboardFormatListener(hwnd);

                this.disposedValue = true;
            }
        }

        private void OnClipboardUpdated(ClipboardUpdatedEventArgs e)
        {
            if (this.ClipboardUpdated != null)
            {
                this.ClipboardUpdated(this, e);
            }
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern int AddClipboardFormatListener(IntPtr hwnd);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int GetClipboardSequenceNumber();

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int RemoveClipboardFormatListener(IntPtr hwnd);
        }
    }
}