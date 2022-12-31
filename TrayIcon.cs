using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using System.Text;
using System.Windows.Forms;

namespace FreezeMouse
{
    class TrayIcon
    {
        ControlContainer container = new ControlContainer();
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem exitToolStripMenuItem;

        public TrayIcon()
        {
            // create a ResourceManager object so we can get the icon
            // the system tray
            ResourceManager resourceManager = new ResourceManager(
                "FreezeMouse.Resource", GetType().Assembly);

            this.notifyIcon = new NotifyIcon(this.container);
            this.contextMenuStrip = new ContextMenuStrip(this.container);
            this.exitToolStripMenuItem = new ToolStripMenuItem();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = (Icon)resourceManager.GetObject("main");
            this.notifyIcon.Text = "Freeze Mouse";
            this.notifyIcon.Visible = true;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new Size(93, 26);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click +=
                new EventHandler(exitToolStripMenuItem_Click);

            resourceManager.ReleaseAllResources();
        }

        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // remove the system tray icon
            this.notifyIcon.Visible = false;

            // close the application
            Application.Exit();
        }
    }
}
