using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using Microsoft.Win32;
using TFSBuildNotifier.TfsBuildStatusProvider;

namespace TFSBuildNotifier
{
    public static class MenuItemHelper
    {
        public static void ConfigureMenuItem(MenuItem menuItem, BuildStatus buildStatus)
        {
            menuItem.Icon = GetImageFromResource(ResourceHelper.GetImageName(buildStatus));
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
            {
                //TODO Construct from config eventually
                var cmdLineArgs = Environment.GetCommandLineArgs();
                var args = string.Empty;
                var first = true;
                foreach (var cmdLineArg in cmdLineArgs)
                {
                    if (first)
                    {
                        first = false;
                        continue;
                    }
                    args += "\"" + cmdLineArg + "\" ";
                }
                rk.SetValue(ResourceHelper.AppName, "\"" + Assembly.GetExecutingAssembly().Location + "\" " + args);
            }
            else
            {
                rk.DeleteValue(ResourceHelper.AppName, false);
            }
        }

        private static string BuildMenuItemHeader(BuildStatus buildStatus)
        {
            return string.Format("{0} - {1} {2} {3} ({4} sec)", buildStatus.BuildName, buildStatus.RequestedBy, buildStatus.BuildDateTime.ToString("HH:mm"), buildStatus.BuildDateTime.ToShortDateString(), buildStatus.BuildTime.ToString("F0"));
        }

        public static bool CheckVersion(string lookingFor = "exe")
        {
            try
            {
                using (var wc = new System.Net.WebClient())
                {
                    wc.Proxy = System.Net.WebRequest.GetSystemWebProxy();
                    var xDoc = new XmlDocument();
                    var s = wc.DownloadString(@"https://github.com/ErikEJ/TFSBuildNotifier/raw/master/TfsTrayVersion.xml");
                    xDoc.LoadXml(s);

                    if (xDoc.DocumentElement != null)
                    {
                        var newVersion = xDoc.DocumentElement.Attributes[lookingFor].Value;

                        var vN = new Version(newVersion);
                        if (vN > Assembly.GetExecutingAssembly().GetName().Version)
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
            return false;
        }

        public static void LaunchUpdateUrl()
        {
            Process.Start("https://ci.appveyor.com/api/projects/ErikEJ/TFSBuildNotifier/artifacts/TfsBuildNotifier.zip?branch=master");
        }

        public static Image GetImageFromResource(string relativeUriFileName)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(relativeUriFileName, UriKind.Absolute);
            bitmap.EndInit();
            return new Image { Source = bitmap };
        }
    }
}
