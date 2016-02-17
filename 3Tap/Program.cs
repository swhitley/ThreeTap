using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace ThreeTap
{
    static class Program
    {
        private static TrayIcon ti;
        private static KeyCapture keys = new KeyCapture();
        /// <summary>
        /// Start a systray app that can detect accidental Yubikey taps. 
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show the system tray icon.
            using (ti = new TrayIcon())
            {
                ti.Display();
                ti.RunAtStartupClick += Ti_RunAtStartupClick;
                ti.ExitClick += Ti_ExitClick;
                ti.YubikeyDetectionTypeClick += Ti_YubikeyDetectionTypeClick;
                ti.ShortcutClick += Ti_ShortcutClick;
                
                KeyCapture.YubikeyDetected += KeyCapture_YubiDetected;
                KeyCapture.YubikeyDetectionToggle += KeyCapture_YubikeyDetectionToggle;

                if(Properties.Settings.Default.YubikeyDetection == 0)
                {
                    keys.Stop();
                }

                // Make sure the application runs!
                Application.Run();
                keys.Stop();
            }
        }

        private static void Ti_YubikeyDetectionTypeClick(object sender, EventArgs<MenuItem> e)
        {
            if ((int)e.Value.Tag == 0 && keys.Started)
            {
                keys.Stop();
            }
            else
            {
                if (!keys.Started)
                {
                    keys.Start();
                }
            }
       
            MenuItem item = (MenuItem)e.Value.Parent;
            YubikeyMenuItemClick(item, (int) e.Value.Tag);
            Properties.Settings.Default.YubikeyDetection = (int) e.Value.Tag;
            Properties.Settings.Default.Save();
        }

        public static void YubikeyMenuItemClick(MenuItem item, int ndx)
        {
            for (int ctr = 0; ctr < 3; ctr++)
            {
                item.MenuItems[ctr].Checked = false;
            }
            item.MenuItems[ndx].Checked = true;
        }


        private static void Ti_ExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private static void Ti_RunAtStartupClick(object sender, EventArgs<MenuItem> e)
        {
            AutoRun.Startup();
            if (AutoRun.IsStartupItem())
            {
                e.Value.Checked = true;
            }
            else
            {
                e.Value.Checked = false;
            }
        }

        private static void Ti_ShortcutClick(object sender, EventArgs<string> e)
        {
            Process.Start(e.Value);
        }

        private static void KeyCapture_YubikeyDetectionToggle(object sender, bool e)
        {
            string message = "Yubikey detection ";
            if(e)
            {
                message += "on.";
            }
            else
            {
                message += "off.";
            }
            ti.Balloon(message);

        }

        private static void KeyCapture_YubiDetected(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.YubikeyDetection == 2)
            {
                MessageBox.Show(new Form() { TopMost = true }, "Yubikey Detected!");
            }
            if (Properties.Settings.Default.YubikeyDetection == 1)
            {
                ti.Balloon("Yubikey Detected!");
            }
        }
    }
}
