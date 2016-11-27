using System.Drawing;
using System.Windows.Media.Imaging;
using TFSBuildNotifier.TfsBuildStatusProvider;

namespace TFSBuildNotifier
{
    public class ResourceHelper
    {
        public static string AppName => "TFS Build Notifier";

        public static string GetImageName(BuildStatus buildStatus)
        {
            var imageName = "StatusHelp_256x.png";
            switch (buildStatus.Status)
            {
                case Status.Error:
                    imageName = "StatusCriticalError_256x.png";
                    break;
                case Status.Success:
                    imageName = "StatusOK_256x.png";
                    break;
                case Status.Undetermined:
                    break;
            }
            return "pack://application:,,,/Resources/" + imageName;
        }

        public static Icon GetIcon(BuildStatus buildStatus)
        {
            var icon = Resource.BuildSolution;
            switch (buildStatus.Status)
            {
                case Status.Error:
                    icon = Resource.StatusCriticalError;
                    break;
                case Status.Success:
                    icon = Resource.StatusOK;
                    break;
                case Status.Undetermined:
                    icon = Resource.StatusHelp;
                    break;
            }
            return icon;
        }
    }
}