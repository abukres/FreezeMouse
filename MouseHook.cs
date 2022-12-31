using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FreezeMouse
{
    class MouseHook
    {
        private IntPtr HookId = IntPtr.Zero;
        private bool _enableMouseMove = true;
        private System.Drawing.Rectangle screenRectangle;
        private double x_min;
        
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Destructor
        /// </summary>
        ~MouseHook()
        {
            // the application has closed so uninstall the mouse hook
            UninstallHook();
        }

        /// <summary>
        /// Install mouse hook to _stopWatch for Shift+Left Mouse Click
        /// and prevent the mouse from moving when enabled
        /// </summary>
        /// <returns>
        ///     true: Hook was installed successfully
        ///     false: Failed to install hook
        /// </returns>
        public bool InstallHook()
        {
            SetDpiAwareness();
            screenRectangle = Screen.PrimaryScreen.Bounds;
            x_min = screenRectangle.Width*0.97;

            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    // install the mouse hook
                    HookId = SetWindowsHookEx(WH_MOUSE_LL, HookCallback,
                        GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            return HookId != IntPtr.Zero;
        }

        /// <summary>
        /// Uninstall the mouse hook
        /// </summary>
        public void UninstallHook()
        {
            if (HookId != IntPtr.Zero)
                UnhookWindowsHookEx(HookId);
        }

        /// <summary>
        /// Mouse hook procedure for Left Mouse Click
        /// and prevent the mouse from moving when enabled
        /// </summary>
        /// <param name="nCode">
        ///     A code the hook procedure uses to 
        ///     determine how to process the message
        /// </param>
        /// <param name="wParam">Identifier of the mouse message</param>
        /// <param name="lParam">Pointer to an MSLLHOOKSTRUCT structure</param>
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {

                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                if ((MouseMessages) wParam == MouseMessages.WM_LBUTTONDOWN && hookStruct.pt.x > x_min)
                    _enableMouseMove = !_enableMouseMove;

                // if mouse movement is disabled and the user is trying to move
                // the mouse return 1 to prevent the message from being processed
                // hence prevent the mouse/cursor from moving
                if (!_enableMouseMove && (MouseMessages) wParam == MouseMessages.WM_MOUSEMOVE)
                    return new IntPtr(1);
            }

            // call the next hook
            return CallNextHookEx(HookId, nCode, wParam, lParam);
        }

        private static void SetDpiAwareness()
        {
            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    SetProcessDpiAwareness(ProcessDPIAwareness.ProcessPerMonitorDPIAware);
                }
            }
            catch (EntryPointNotFoundException) //this exception occurs if OS does not implement this API, just ignore it.
            {
            }
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            MK_MBUTTON = 0x0010,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_DBLCLICK = 0xA3
        }


        private enum ProcessDPIAwareness
        {
            ProcessDPIUnaware = 0,
            ProcessSystemDPIAware = 1,
            ProcessPerMonitorDPIAware = 2
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDPIAwareness value);

    }
}
