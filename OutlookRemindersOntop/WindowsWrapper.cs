using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Collections;
using System.Text;
using System.Collections.Generic;


namespace OutlookRemindersOntop
{

    public class WindowsWrapper
    {
        internal static event EventHandler WindowActivatedChanged;
        internal static Timer TimerWatcher = new Timer();
        internal static WindowInfo WindowActive = new WindowInfo();
        internal static void DoStartWatcher()
        {
            TimerWatcher.Interval = 500;
            TimerWatcher.Elapsed += TimerWatcher_Tick;
            TimerWatcher.Start();
        }

        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static IDictionary<IntPtr, WindowInfo> GetOpenedWindows()
        {
            IntPtr shellWindow = GetShellWindow();
            Console.WriteLine($"shellwindow {shellWindow}");
            Dictionary<IntPtr, WindowInfo> windows = new Dictionary<IntPtr, WindowInfo>();

            EnumWindows(new EnumWindowsProc(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;
                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;
                var info = new WindowInfo();
                info.Handle = hWnd;
                info.File = new FileInfo(GetProcessPath(hWnd));
                //info.Title = builder.ToString();
                windows[hWnd] = info;
                return true;
            }), 0);
            return windows;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        public static string GetProcessPath(IntPtr hwnd)
        {
            uint pid = 0;
            GetWindowThreadProcessId(hwnd, out pid);
            if (hwnd != IntPtr.Zero)
            {
                if (pid != 0)
                {
                    var process = Process.GetProcessById((int)pid);
                    if (process != null)
                    {
                        return process.MainModule.FileName.ToString();
                    }
                }
            }
            return "";
        }

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        //WARN: Only for "Any CPU":
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out uint processId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_TOPMOST = 0x0008;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos( 
           IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);


        public static bool IsWindowTopMost(IntPtr hWnd)
        {
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            return (exStyle & WS_EX_TOPMOST) == WS_EX_TOPMOST;
        }

        static void TimerWatcher_Tick(object sender, EventArgs e)
        {
            var windowActive = new WindowInfo();
            windowActive.Handle = GetForegroundWindow();
            string path = GetProcessPath(windowActive.Handle);
            if (string.IsNullOrEmpty(path)) return;
            windowActive.File = new FileInfo(path);
            int length = GetWindowTextLength(windowActive.Handle);
            if (length == 0) return;

            if (windowActive.ToString() != WindowActive.ToString())
            {
                //fire:
                WindowActive = windowActive;
                if (WindowActivatedChanged != null) WindowActivatedChanged(sender, e);
                Console.WriteLine("Window: " + WindowActive.ToString());
            }
        }


        static void showallProcessTitles()
        {
            var processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    var handle = process.MainWindowHandle;
                    Logger.Debug($"Process: {process.ProcessName} Handle {handle} Window title:{ process.MainWindowTitle}");
                }
            }
        }
        static IEnumerable<IntPtr> getWindowPtrForTitle(string processFilter, string titleFilter, Action<string, string> showNotification)
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {

                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    var handle = process.MainWindowHandle;
                    if (process.ProcessName.Contains(processFilter, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (process.MainWindowTitle.Contains(titleFilter, StringComparison.CurrentCultureIgnoreCase))
                        {
                            Logger.Debug($"Matched Process: {process.ProcessName} Handle {handle} Window title:{ process.MainWindowTitle}");
                            showNotification(process.ProcessName, process.MainWindowTitle);

                            yield return handle;
                        }
                        else
                        {
                            Logger.Debug($"Matched process Window process:{ process.ProcessName} titleunmatched {process.MainWindowTitle}");
                        }
                    }

                }
            }
        }


        public static void changeWindowOnTopSetting(string proc, string windowTitle, Action<string, string> showNotification, bool setOnTop = true)
        {
            foreach (var winptr in getWindowPtrForTitle(proc, windowTitle, showNotification))
            {
                changeWindowOnTopSetting(winptr);
            }

        }
        static void changeWindowOnTopSetting(IntPtr windowHandle, bool setOnTop = true)
        {
            if (windowHandle == IntPtr.Zero)
            {
                Trace.WriteLine($"Window handle is zero");
                return;
            }
            const int HWND_TOPMOST = -1;
            const int HWND_NOTOPMOST = -2;
            const int SWP_NOMOVE = 0x0002;
            const int SWP_NOSIZE = 0x0001;
            var windowSetting = setOnTop ? HWND_TOPMOST : HWND_NOTOPMOST;
            SetWindowPos(
                windowHandle,
                windowSetting, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

        }
    }
}
