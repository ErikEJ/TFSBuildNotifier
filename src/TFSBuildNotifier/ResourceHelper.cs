using System.Drawing;
using Hardcodet.Wpf.TaskbarNotification;
using TFSBuildNotifier.TfsBuildStatusProvider;

namespace TFSBuildNotifier
{
    public class ResourceHelper
    {
        public static string GetImageName(BuildStatus buildStatus)
        {
            var imageName = "Resources/StatusHelp_256x.png";
            switch (buildStatus.Status)
            {
                case Status.Error:
                    imageName = "Resources/StatusCriticalError_256x.png";
                    break;
                case Status.Success:
                    imageName = "Resources/StatusOK_256x.png";
                    break;
                case Status.Undetermined:
                    break;
            }
            return imageName;
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