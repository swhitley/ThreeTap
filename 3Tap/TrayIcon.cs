using System;
using System.Drawing;
using System.Windows.Forms;
using ThreeTap.Properties;
using System.IO;

namespace ThreeTap
{
    public enum YubikeyDetectionType
    {
        None, Warning, Popup
    }
    class TrayIcon : IDisposable
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private MenuItem RunAtStartupMenuItem;
        private MenuItem YubikeyMenuItem;
        public event EventHandler ExitClick;
        public event EventHandler<EventArgs<MenuItem>> RunAtStartupClick;
        public event EventHandler<EventArgs<MenuItem>> YubikeyDetectionTypeClick;
        public event EventHandler<EventArgs<string>> ShortcutClick;

        public TrayIcon()
        {
            trayIcon = new NotifyIcon();
        }

        protected virtual void OnShortcutClick(EventArgs<string> e)
        {
            EventHandler<EventArgs<string>> handler = ShortcutClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnYubikeyDetectionTypeClick(EventArgs<MenuItem> e)
        {
            EventHandler<EventArgs<MenuItem>> handler = YubikeyDetectionTypeClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRunAtStartupClick(EventArgs<MenuItem> e)
        {
            EventHandler<EventArgs<MenuItem>> handler = RunAtStartupClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnExitClick(EventArgs e)
        {
            EventHandler handler = ExitClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Balloon(string msg)
        {
            trayIcon.BalloonTipText = msg;
            trayIcon.ShowBalloonTip(5000);
        }

        public void Display()
        {

            trayMenu = new ContextMenu();

            trayIcon.Click += TrayIcon_Click;


            RunAtStartupMenuItem = trayMenu.MenuItems.Add("Run at Startup", new EventHandler(OnRunAtStartupClick));
            if (AutoRun.IsStartupItem())
            {
                RunAtStartupMenuItem.Checked = true;
            }

            MenuItem shortcuts = trayMenu.MenuItems.Add("Shortcuts");
            try {
                string path = ClickOnce.GetDataDirectory();
                string[] lines = File.ReadAllLines(path + "\\shortcuts.txt");
                foreach (string line in lines)
                {
                    string name = line.Split('|')[0];
                    string command = line.Split('|')[1];
                    shortcuts.MenuItems.Add(name, new EventHandler(OnShortcutClick)).Tag = command;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("An error occurred while loading shortcuts. Please check the shortcuts file.");
            }

            YubikeyMenuItem = trayMenu.MenuItems.Add("Yubikey Detection");
            YubikeyMenuItem.MenuItems.Add("None", new EventHandler(OnYubikeyDetectionTypeClick)).Tag = (int) YubikeyDetectionType.None;
            YubikeyMenuItem.MenuItems.Add("Warning - System Tray Balloon", new EventHandler(OnYubikeyDetectionTypeClick)).Tag = (int) YubikeyDetectionType.Warning;
            YubikeyMenuItem.MenuItems.Add("Popup - Message Box", new EventHandler(OnYubikeyDetectionTypeClick)).Tag = (int) YubikeyDetectionType.Popup;

            trayMenu.MenuItems.Add("Exit", new EventHandler(OnExitClick));

            YubikeyMenuItemClick(YubikeyMenuItem, Settings.Default.YubikeyDetection);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "3Tap";
            trayIcon.Icon = new Icon(Resources.icon, 40, 40);

            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

        public void YubikeyMenuItemClick(MenuItem item, int ndx)
        {
            for(int ctr = 0; ctr < 3; ctr++)
            {
                item.MenuItems[ctr].Checked = false;
            }
            item.MenuItems[ndx].Checked = true;
        }

        private void TrayIcon_Click(object sender, EventArgs e)
        {
        }

        private void OnShortcutClick(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;

            OnShortcutClick(new EventArgs<string>(item.Tag.ToString()));
        }


        private void OnYubikeyDetectionTypeClick(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;

            OnYubikeyDetectionTypeClick(new EventArgs<MenuItem>(item));
        }

        private void OnRunAtStartupClick(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            OnRunAtStartupClick(new EventArgs<MenuItem>(item));
        }

        private void OnExitClick(object sender, EventArgs e)
        {
            OnExitClick(e);
        }

        public void Dispose()
        {
            trayIcon.Visible = false; 
            trayIcon.Dispose();
        }
    }
}
