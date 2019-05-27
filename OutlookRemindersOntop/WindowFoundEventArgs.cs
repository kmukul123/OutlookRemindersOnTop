using System;

namespace OutlookRemindersOntop
{
    public class WindowFoundEventArgs : EventArgs
    {
        public string WindowTitle { get; set; }
        public string WindowProcess { get; set; }
    }
}