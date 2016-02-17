using Microsoft.Win32;
using System.Windows.Forms;

namespace ThreeTap
{
    public static class AutoRun
    {
        public static void Startup()
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (!IsStartupItem())
            {
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue("ThreeTap", Application.ExecutablePath.ToString());
            }
            else
            {

                if (IsStartupItem())
                {
                    // Remove the value from the registry so that the application doesn't start
                    rkApp.DeleteValue("ThreeTap", false);
                }
            }
        }

        public static bool IsStartupItem()
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rkApp.GetValue("ThreeTap") == null)
                // The value doesn't exist, the application is not set to run at startup
                return false;
            else
                // The value exists, the application is set to run at startup
                return true;
        }
    }
}
