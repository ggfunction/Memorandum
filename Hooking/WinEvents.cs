namespace Memorandum.Hooking
{
    using System;

    public enum WinEvents : uint
    {
        ConsoleApplication16Bit = 0x00000001,

        ConsoleCaretSelection = 0x00000001,

        SystemSound = 0x00000001,

        EventMin = 0x00000001,

        SystemAlert = 0x00000002,

        ConsoleCaretVisible = 0x00000002,

        SystemForeground = 0x00000003,

        SystemMenuStart = 0x00000004,

        SystemMenuEnd = 0x00000005,

        SystemMenuPopupStart = 0x00000006,

        SystemMenuPopupEnd = 0x00000007,

        SystemCaptureStart = 0x00000008,

        SystemCaptureEnd = 0x00000009,

        SystemMoveSizeStart = 0x0000000A,

        SystemMoveSizeEnd = 0x0000000B,

        SystemContextHelpStart = 0x0000000C,

        SystemContextHelpEnd = 0x0000000D,

        SystemDragDropStart = 0x0000000E,

        SystemDragDropEnd = 0x0000000F,

        SystemDialogStart = 0x00000010,

        SystemDialogEnd = 0x00000011,

        SystemScrollingStart = 0x00000012,

        SystemScrollingEnd = 0x00000013,

        SystemSwitchStart = 0x00000014,

        SystemSwitchEnd = 0x00000015,

        SystemMinimizeStart = 0x00000016,

        SystemMinimizeEnd = 0x00000017,

        SystemDesktopSwitch = 0x00000020,

        ConsoleCaret = 0x00004001,

        ConsoleUpdateRegion = 0x00004002,

        ConsoleUpdateSimple = 0x00004003,

        ConsoleUpdateScroll = 0x00004004,

        ConsoleLayout = 0x00004005,

        ConsoleStartApplication = 0x00004006,

        ConsoleEndApplication = 0x00004007,

        ObjectCreate = 0x00008000,

        ObjectDestroy = 0x00008001,

        ObjectShow = 0x00008002,

        ObjectHide = 0x00008003,

        ObjectReorder = 0x00008004,

        ObjectFocus = 0x00008005,

        ObjectSelection = 0x00008006,

        ObjectSelectionAdd = 0x00008007,

        ObjectSelectionRemove = 0x00008008,

        ObjectSelectionWithIn = 0x00008009,

        ObjectStateChange = 0x0000800A,

        ObjectLocationChange = 0x0000800B,

        ObjectNameChange = 0x0000800C,

        ObjectDescriptionChange = 0x0000800D,

        ObjectValueChange = 0x0000800E,

        ObjectParentChange = 0x0000800F,

        ObjectHelpChange = 0x00008010,

        ObjectDefactionChange = 0x00008011,

        ObjectAcceleratorChange = 0x00008012,

        ObjectInvoked = 0x00008013,

        ObjectTextSelectionChanged = 0x00008014,

        ObjectContentScrolled = 0x00008015,

        ObjectCloaked = 0x00008017,

        ObjectUncloaked = 0x00008018,

        ObjectLiveRegionChanged = 0x00008019,

        EventMax = 0x7FFFFFFF,
    }
}
