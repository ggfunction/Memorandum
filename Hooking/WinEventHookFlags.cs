namespace Memorandum.Hooking
{
    using System;

    [Flags]
    public enum WinEventHookFlags : uint
    {
        OutOfContext = 0,

        SkipOwnThread = 1,

        SkipOwnProcess = 2,

        InContext = 4,
    }
}
