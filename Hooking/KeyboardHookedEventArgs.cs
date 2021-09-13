namespace Memorandum.Hooking
{
    using System;
    using System.Runtime.InteropServices;

    public class KeyboardHookedEventArgs : LowLevelHookedEventArgs
    {
        public KeyboardHookedEventArgs(IntPtr message, IntPtr state, IntPtr foregroundWindow)
            : base(message, state, foregroundWindow)
        {
        }

        public bool Alt { get; private set; }

        public bool Extended { get; private set; }

        public int KeyCode { get; private set; }

        public int ScanCode { get; private set; }

        public bool Up { get; private set; }

        protected override void Initialize(IntPtr message, IntPtr state)
        {
            var value = (NativeMethods.MouseLowLevelHookStruct)Marshal.PtrToStructure(state, typeof(NativeMethods.MouseLowLevelHookStruct));
            this.Time = value.Time;
            this.ExtraInfo = value.DwExtraInfo;
            this.Extended = (value.Flags & 0x0001) != 0;
            this.LowIntegrityLevelInjected = (value.Flags & 0x0002) != 0;
            this.Injected = (value.Flags & 0x0010) != 0;
            this.Alt = (value.Flags & 0x0020) != 0;
            this.Up = (value.Flags & 0x0080) != 0;
            this.KeyCode = (int)value.VkCode;
            this.ScanCode = (int)value.ScanCode;
        }

        private class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct MouseLowLevelHookStruct
            {
                public uint VkCode;

                public uint ScanCode;

                public uint Flags;

                public uint Time;

                public IntPtr DwExtraInfo;
            }
        }
    }
}