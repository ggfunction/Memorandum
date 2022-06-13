namespace Memorandum.Hooking
{
    using System;

    public class WinEventHookedEventArgs : EventArgs
    {
        public WinEventHookedEventArgs(IntPtr winEventHook, uint eventValue, IntPtr windowHandle, int idObject, int idChild, uint idEventThread, uint eventTime)
        {
            this.WinEventHook = winEventHook;
            this.EventValue = eventValue;
            this.WindowHandle = windowHandle;
            this.IdObject = idObject;
            this.IdChild = idChild;
            this.IdEventThread = idEventThread;
            this.EventTime = eventTime;
        }

        public IntPtr WinEventHook { get; private set; }

        public uint EventValue { get; private set; }

        public IntPtr WindowHandle { get; private set; }

        public int IdObject { get; private set; }

        public int IdChild { get; private set; }

        public uint IdEventThread { get; private set; }

        public uint EventTime { get; private set; }
    }
}