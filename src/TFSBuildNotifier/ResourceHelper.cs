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

        public static BalloonIcon GetIcon(BuildStatus buildStatus)
        {
            var icon = BalloonIcon.None;
            switch (buildStatus.Status)
            {
                case Status.Error:
                    icon = BalloonIcon.Error;
                    break;
                case Status.Success:
                    icon = BalloonIcon.Info;
                    break;
                case Status.Undetermined:
                    icon = BalloonIcon.None;
                    break;
            }
            return icon;
        }
    }
}