namespace Memorandum.Hooking
{
    using System;

    public abstract class LowLevelHookedEventArgs : System.ComponentModel.CancelEventArgs
    {
        protected LowLevelHookedEventArgs(IntPtr message, IntPtr state, IntPtr foregroundWindow)
        {
            this.Message = message;
            this.State = state;
            this.ForegroundWindow = foregroundWindow;
            this.Initialize(message, state);
        }

        public IntPtr ExtraInfo { get; protected set; }

        public IntPtr ForegroundWindow { get; private set; }

        public bool Injected { get; protected set; }

        public bool LowIntegrityLevelInjected { get; protected set; }

        public IntPtr Message { get; private set; }

        public IntPtr State { get; private set; }

        public uint Time { get; protected set; }

        protected abstract void Initialize(IntPtr message, IntPtr state);
    }
}