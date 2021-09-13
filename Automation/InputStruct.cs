namespace Memorandum.Automation
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct InputStruct
    {
        public InputTypes Type;

        public InputUnion As;

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)]
            public MouseInputStruct Mouse;

            [FieldOffset(0)]
            public KeyboardInputStruct Keyboard;

            [FieldOffset(0)]
            public HardwareInputStruct Hardware;
        }
    }
}