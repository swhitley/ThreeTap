using System.Deployment.Application;
using System.Windows.Forms;

namespace ThreeTap
{
    public static class ClickOnce
    {
        public static string GetDataDirectory()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.DataDirectory;
            }
            else
            {
                return Application.StartupPath;
            }
        }
    }
}
