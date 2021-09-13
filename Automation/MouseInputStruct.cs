namespace Memorandum.Automation
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInputStruct
    {
        public int X;

        public int Y;

        public int Data;

        public int Flags;

        public int Time;

        public IntPtr ExtraInfo;
    }
}