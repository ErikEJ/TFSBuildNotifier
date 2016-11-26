using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TFSBuildNotifier.TfsBuildStatusProvider;
using System.Windows;

namespace TFSBuildNotifier
{
    public static class MenuItemHelper
    {
        public static void ConfigureMenuItem(MenuItem menuItem, BuildStatus buildStatus)
        {
            var imageName = ResourceHelper.GetImageName(buildStatus);
            menuItem.Icon = new Image
            {
                Source = new BitmapImage(new Uri(imageName, UriKind.Relative))
            };
            menuItem.Header = BuildMenuItemHeader(buildStatus);
            menuItem.Tag = buildStatus.Key;
        }

        public static void LaunchUrl(MenuItem menuItem, List<BuildStatus> buildStatuses)
        {
            var uri = menuItem?.Tag as Uri;
            if (uri == null) return;
            var link = buildStatuses.Where(b => b.Key == uri).Select(b => b.Link).Single();
            Process.Start(link.ToString());
        }

        public static bool GetStartupStatus()
        {
            var rk = Registry.CurrentUser.CreateSubKey(
                "SOFTWARE\\ErikEJ\\TFSNotifier", RegistryKeyPermissionCheck.Default);

            if (rk == null) return true;
            //If the value has never been set, default to true!
            if (rk.GetValue("AutoLaunch") == null)
            {
                rk.SetValue("AutoLaunch", bool.TrueString);
                return true;
            }
            return bool.Parse((string)rk.GetValue("AutoLaunch"));
        }

        private static void SetStartupStatus(bool autoLaunch)
        {
            var rk = Registry.CurrentUser.CreateSubKey(
                "SOFTWARE\\ErikEJ\\TFSNotifier", RegistryKeyPermissionCheck.Default);

            rk?.SetValue("AutoLaunch", autoLaunch.ToString());
        }


        public static void SetStartup(bool isChecked)
        {
            SetStartupStatus(isChecked);
            var rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rk == null) return;
            if (isChecked)
                // ReSharper disable once AssignNullToNotNullAttribute
                rk.SetValue(ResourceHelper.AppName, Assembly.GetExecutingAssembly().Location);
            else
                rk.DeleteValue(ResourceHelper.AppName, false);
        }

        private static string BuildMenuItemHeader(BuildStatus buildStatus)
        {
            return string.Format("{0} - {1} {2} {3} ({4} sec)", buildStatus.BuildName, buildStatus.RequestedBy, buildStatus.BuildDateTime.ToString("HH:mm"), buildStatus.BuildDateTime.ToShortDateString(), buildStatus.BuildTime.ToString("F0"));
        }

    }
}
