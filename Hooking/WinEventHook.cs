namespace Memorandum.Hooking
{
    using System;
    using System.Runtime.InteropServices;

    public class WinEventHook : IDisposable
    {
        private NativeMethods.WinEventProc callback;

        private bool disposedValue;

        public WinEventHook(uint eventMin, uint eventMax, IntPtr module, uint processId, uint threadId, WinEventHookFlags flags)
        {
            this.callback = new NativeMethods.WinEventProc(this.WinEventCallback);

            this.EventMin = eventMin;
            this.EventMax = eventMax;
            this.Module = module;
            this.ProcessId = processId;
            this.ThreadId = threadId;
            this.Flags = flags;

            this.AttachHook();
        }

        ~WinEventHook()
        {
            this.Dispose(false);
        }

        public event EventHandler<WinEventHookedEventArgs> WinEventHooked;

        public uint EventMin { get; private set; }

        public uint EventMax { get; private set; }

        public IntPtr HookHandle { get; private set; }

        public bool IsHooking
        {
            get { return this.HookHandle != IntPtr.Zero; }
        }

        public IntPtr Module { get; private set; }

        public uint ProcessId { get; private set; }

        public uint ThreadId { get; private set; }

        public WinEventHookFlags Flags { get; private set; }

        public void Close()
        {
            this.DetachHook();
        }

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

                if (this.IsHooking)
                {
                    this.DetachHook();
                }

                this.callback = null;

                this.disposedValue = true;
            }
        }

        protected virtual void OnWinEventHooked(WinEventHookedEventArgs e)
        {
            if (this.WinEventHooked != null)
            {
                this.WinEventHooked(this, e);
            }
        }

        private void WinEventCallback(IntPtr winEventHook, uint eventValue, IntPtr windowHandle, int idObject, int idChild, uint idEventThread, uint eventTime)
        {
            var e = new WinEventHookedEventArgs(winEventHook, eventValue, windowHandle, idObject, idChild, idEventThread, eventTime);
            this.OnWinEventHooked(e);
        }

        private void AttachHook()
        {
            this.HookHandle = NativeMethods.SetWinEventHook(
                this.EventMin,
                this.EventMax,
                this.Module,
                this.callback,
                this.ProcessId,
                this.ThreadId,
                (uint)this.Flags);
        }

        private void DetachHook()
        {
            NativeMethods.UnhookWinEvent(this.HookHandle);
            this.HookHandle = IntPtr.Zero;
        }

        private static class NativeMethods
        {
            public delegate void WinEventProc(IntPtr winEventHook, uint eventValue, IntPtr windowHandle, int idObject, int idChild, uint idEventThread, uint eventTime);

            [DllImport("user32.dll")]
            public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr module, WinEventProc callback, uint idProcess, uint idThread, uint flags);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UnhookWinEvent(IntPtr winEventHook);
        }
    }
}