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
            notifyIcon1.Icon = SystemIcons.Application;

            windowWatcher.WindowFoundHandler += WindowWatcher_WindowFoundHandler;
        }
        private void WindowWatcher_WindowFoundHandler(object sender, WindowFoundEventArgs e)
        {
            var last = notifyIcon1.Visible;
            notifyIcon1.Visible = true;
            notifyIcon1.BalloonTipText = $"Bringing {e.window.WndProcess}'s window with title {e.window.Title} on top isVisible {e.window.IsVisible} ";
            notifyIcon1.ShowBalloonTip(500);
            notifyIcon1.Visible = last;

        }

        private void OutlookRemindersOnTop_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.BalloonTipText = "RemindersOnTop is running here";
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                //notifyIcon1.Visible = false;
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowWindow();
        }
        private void ShowWindow()
        {
            notifyIcon1.Visible = false;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            ShowWindow();
        }
    }
}
