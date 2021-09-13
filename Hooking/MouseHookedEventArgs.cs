namespace Memorandum.Hooking
{
    using System;
    using System.Runtime.InteropServices;

    public class MouseHookedEventArgs : LowLevelHookedEventArgs
    {
        public MouseHookedEventArgs(IntPtr message, IntPtr state, IntPtr foregroundWindow)
            : base(message, state, foregroundWindow)
        {
        }

        public System.Drawing.Point Location { get; private set; }

        public int MouseData { get; private set; }

        public int X
        {
            get { return this.Location.X; }
        }

        public int Y
        {
            get { return this.Location.Y; }
        }

        protected override void Initialize(IntPtr message, IntPtr state)
        {
            var value = (NativeMethods.MouseLowLevelHookStruct)Marshal.PtrToStructure(state, typeof(NativeMethods.MouseLowLevelHookStruct));
            this.Time = value.Time;
            this.ExtraInfo = value.DwExtraInfo;
            this.Injected = (value.Flags & 0x0001) != 0;
            this.LowIntegrityLevelInjected = (value.Flags & 0x0002) != 0;
            this.Location = value.Pt;
            this.MouseData = (int)value.MouseData;
        }

        private class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct MouseLowLevelHookStruct
            {
                public System.Drawing.Point Pt;

                public uint MouseData;

                public uint Flags;

                public uint Time;

                public IntPtr DwExtraInfo;
            }
        }
    }
}
