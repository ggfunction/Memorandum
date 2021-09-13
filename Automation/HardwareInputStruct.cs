namespace Memorandum.Automation
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInputStruct
    {
        public int Message;

        public short ParamLow;

        public short ParamHigh;
    }
}