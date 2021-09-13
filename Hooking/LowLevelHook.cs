namespace Memorandum.Hooking
{
    using System;
    using System.Runtime.InteropServices;

    public abstract class LowLevelHook : IDisposable
    {
        private NativeMethods.HookProc callback;

        private bool disposedValue;

        protected LowLevelHook(HookTypes hookType)
        {
            this.HookType = hookType;
            this.callback = new NativeMethods.HookProc(this.HookCallback);
        }

        ~LowLevelHook()
        {
            this.Dispose(false);
        }

        protected enum HookTypes
        {
            KeyboardLowLevel = 13,

            MouseLowLevel = 14,
        }

        public IntPtr Handle { get; private set; }

        public bool IsHooking
        {
            get { return this.Handle != IntPtr.Zero; }
        }

        protected HookTypes HookType { get; private set; }

        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Start()
        {
            if (this.disposedValue)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }

            if (!this.IsHooking)
            {
                this.AttachHook();
            }
        }

        public void Stop()
        {
            if (this.disposedValue)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }

            if (this.IsHooking)
            {
                this.DetachHook();
            }
        }

        protected IntPtr GetForegroundWindow()
        {
            return NativeMethods.GetForegroundWindow();
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

        protected virtual int HookCallback(int code, IntPtr message, IntPtr state)
        {
            return NativeMethods.CallNextHookEx(this.Handle, code, message, state);
        }

        protected void AttachHook()
        {
            this.Handle = NativeMethods.SetWindowsHookEx(
                this.HookType,
                this.callback,
                IntPtr.Zero,
                0);

            if (this.Handle == IntPtr.Zero)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new System.ComponentModel.Win32Exception(errorCode);
            }
        }

        protected void DetachHook()
        {
            var ret = NativeMethods.UnhookWindowsHookEx(this.Handle);

            if (!ret)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new System.ComponentModel.Win32Exception(errorCode);
            }

            this.Handle = IntPtr.Zero;
        }

        private static class NativeMethods
        {
            public delegate int HookProc(int code, IntPtr message, IntPtr state);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(HookTypes hookType, HookProc callback, IntPtr module, uint thredId);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int CallNextHookEx(IntPtr hookHandle, int code, IntPtr message, IntPtr state);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr GetForegroundWindow();
        }
    }
}