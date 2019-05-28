using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutlookRemindersOntop
{
    public partial class OutlookRemindersOnTop : Form
    {
        public OutlookRemindersOnTop()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            var windowWatcher = new WindowWatcher();
            notifyIcon1.Visible = true;
            notifyIcon1.Icon = SystemIcons.Application;

            windowWatcher.WindowFoundHandler += WindowWatcher_WindowFoundHandler;
        }
        private void WindowWatcher_WindowFoundHandler(object sender, WindowFoundEventArgs e)
        {
            notifyIcon1.BalloonTipText = $"Bringing {e.WindowProcess}'s window with title {e.WindowTitle} on top";
            notifyIcon1.ShowBalloonTip(500);

        }


    }
}
