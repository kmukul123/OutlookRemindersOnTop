using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OutlookRemindersOntop
{
    class WindowsWrapper
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos
            (IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

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
                        } else
                        {
                            Logger.Debug($"Matched process Window process:{ process.ProcessName} titleunmatched {process.MainWindowTitle}");
                        }
                    }
                    
                }
            }
        }


        public static void changeWindowOnTopSetting(string proc, string windowTitle, Action<string, string> showNotification, bool setOnTop = true)
        {
            foreach (var winptr in getWindowPtrForTitle(proc, windowTitle, showNotification)) {
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
