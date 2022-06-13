namespace Memorandum.Hooking
{
    using System;

    public enum ObjectIdentifiers : uint
    {
        Window = 0x00000000,

        Sound = 0xFFFFFFF5,

        Alert = 0xFFFFFFF6,

        Cursor = 0xFFFFFFF7,

        Caret = 0xFFFFFFF8,

        Sizegrip = 0xFFFFFFF9,

        HScroll = 0xFFFFFFFA,

        VScroll = 0xFFFFFFFB,

        Client = 0xFFFFFFFC,

        Menu = 0xFFFFFFFD,

        Titlebar = 0xFFFFFFFE,

        SysMenu = 0xFFFFFFFF,
    }
}
