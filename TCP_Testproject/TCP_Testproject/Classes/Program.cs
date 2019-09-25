using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Testproject.Classes
{
    class Program
    {
        static void Main(string[] args)
        {
            EnableQuickEditMode();

            new Logic();

            Logic.InitClientServer();
        }

        public static void EnableQuickEditMode()
        {
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            uint mode;

            GetConsoleMode(hStdin, out mode);

            mode &= mode | ENABLE_EXTENDED_FLAGS | ENABLE_QUICK_EDIT_MODE;

            SetConsoleMode(hStdin, mode);
        }

        const int STD_INPUT_HANDLE = -10;

        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        const uint ENABLE_EXTENDED_FLAGS = 0x0080;

        [DllImport("kernel32.dll", SetLastError = true)]

        internal static extern IntPtr GetStdHandle(int hConsoleHandle);

        [DllImport("kernel32.dll", SetLastError = true)]

        internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);

        [DllImport("kernel32.dll", SetLastError = true)]

        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);

        // Check for when chat is minimized
        public static bool ProgramHasFocus()
        {
            if (GetForegroundWindow() == Process.GetCurrentProcess().MainWindowHandle)
            {
                return true;    // Chat is currently focussed
            }
            else
            {
                return false;   // Chat is currently not focussed
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        // Taskbar icon flash funcionality
        public static void StartFlashTaskbarIcon()
        {
            StartFlash(Process.GetCurrentProcess().MainWindowHandle);
        }

        public static void StopFlashTaskbarIcon()
        {
            StopFlash(Process.GetCurrentProcess().MainWindowHandle);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public Int32 dwTimeout;
        }

        public const UInt32 FLASHW_ALL = 3;
        public const UInt32 FLASHW_STOP = 0;

        private static void StartFlash(IntPtr hWnd)
        {
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            fInfo.dwFlags = FLASHW_ALL;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            FlashWindowEx(ref fInfo);

            do
            {
                System.Threading.Thread.Sleep(500);
            } while (!ProgramHasFocus());

            StopFlashTaskbarIcon();
        }

        private static void StopFlash(IntPtr hWnd)
        {
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            fInfo.dwFlags = FLASHW_STOP;
            fInfo.uCount = UInt32.MinValue;
            fInfo.dwTimeout = 0;

            FlashWindowEx(ref fInfo);
        }
    }
}
