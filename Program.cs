using System;
using System.Windows.Forms;

namespace FreezeMouse
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MouseHook mouseHook = new MouseHook();

            // install the mouse hook to watch for Shift+Left Mouse Click
            // and prevent the mouse from moving when enabled
            mouseHook.InstallHook();

            TrayIcon sc = new TrayIcon();
            Application.Run();            // start the application
        }
    }
}
