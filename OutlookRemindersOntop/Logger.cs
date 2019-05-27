using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
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
    }
}
