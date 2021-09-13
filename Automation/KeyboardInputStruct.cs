namespace Memorandum.Automation
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardInputStruct
    {
        public short VirtualKey;

        public short ScanCode;

        public int Flags;

        public int Time;

        public IntPtr ExtraInfo;
    }
}