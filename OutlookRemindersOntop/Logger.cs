﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace OutlookRemindersOntop
{
    static class Logger
    {
        public static void Info(string logline)
        {
            Trace.TraceInformation(logline);
            Console.WriteLine(logline);
        }

        public static void Debug(string logline)
        {
            System.Diagnostics.Debug.WriteLine(logline);
            Info(logline);
        }

        public static void Error(string logline)
        {
            Trace.TraceError(logline);
#if DEBUG
            MessageBox.Show($"Error {logline}");
#endif
            Console.WriteLine(logline);
        }
    }
}
