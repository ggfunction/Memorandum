namespace Memorandum.UI
{
    using System;
    using System.Runtime.InteropServices;

    public static class WindowExtensions
    {
        public static string GetProcessName(this Window w)
        {
            return NativeMethods.GetProcessName(w);
        }

        private static class NativeMethods
        {
            [Flags]
            private enum ProcessAccessFlags : uint
            {
                VirtualMemoryRead = 0x00000010,

                QueryInformation = 0x00000400,

                VirtualMemoryReadQueryInformation = VirtualMemoryRead | QueryInformation,
            }

            public static string GetProcessName(Window w)
            {
                var processHandle = IntPtr.Zero;

                try
                {
                    processHandle = NativeMethods.OpenProcess(
                        (uint)NativeMethods.ProcessAccessFlags.VirtualMemoryReadQueryInformation,
                        false,
                        (uint)w.ProcessId);
                    var sb = new System.Text.StringBuilder(260);
                    NativeMethods.GetModuleBaseName(processHandle, IntPtr.Zero, sb, (uint)sb.Capacity);
                    return sb.ToString();
                }
                finally
                {
                    NativeMethods.CloseHandle(processHandle);
                }
            }

            [DllImport("kernel32.dll")]
            private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

            [DllImport("kernel32.dll")]
            private static extern bool CloseHandle(IntPtr handle);

            [DllImport("psapi.dll")]
            private static extern uint GetModuleBaseName(IntPtr hWnd, IntPtr hModule, System.Text.StringBuilder lpBaseName, uint nSize);
        }
    }
}
