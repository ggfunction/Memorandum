namespace Memorandum.Hooking
{
    using System;

    public enum Messages
    {
        KeyDown = 0x100,

        KeyUp = 0x101,

        SystemKeyDown = 0x104,

        SystemKeyUp = 0x105,

        MouseMove = 0x0200,

        LeftButtonDown = 0x0201,

        LeftButtonUp = 0x0202,

        LeftButtonDoubleClick = 0x0203,

        RightButtonDown = 0x0204,

        RightButtonUp = 0x0205,

        RightButtonDoubleClick = 0x0206,

        MiddleButtonDown = 0x0207,

        MiddleButtonUp = 0x0208,

        MiddleButtonDoubleClick = 0x0209,

        MouseWheel = 0x020A,

        XButtonDown = 0x020B,

        XButtonUp = 0x020C,

        XButtonDoubleClick = 0x020D,
    }
}