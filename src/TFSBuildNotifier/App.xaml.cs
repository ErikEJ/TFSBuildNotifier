using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using TFSBuildNotifier.TfsBuildStatusProvider;

namespace TFSBuildNotifier
{
    public partial class App : Application
    {
        private bool _isExit;
        private TaskbarIcon _taskbarIcon;
        private ContextMenu _contextMenu = new ContextMenu();
        private List<BuildStatus> _buildStatuses;
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.Closing += MainWindow_Closing;
            BuildTaskBarIcon();
            IBuildStatusProvider statusProvider;
#if DEBUG
            statusProvider = new DebugStatusProvider();
#else
#endif
            var uriList = new List<Uri>();
            try
            {
                foreach (var arg in e.Args)
                {
                    uriList.Add(new Uri(arg));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid command line");
            }

            AddMenuItems(statusProvider, uriList);
            StartTimer();
        }

        private void StartTimer()
        {
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            _dispatcherTimer.Start();
        }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            _dispatcherTimer.Stop();

            //TODO In timer event 
            //- Get status for each Url
            // Enumerate menu items
            // If BuildId for the Key is different, show Baloon
            // Update menu items with new icon if required
            // Save new status in _buildStatuses

            var title = _buildStatuses[0].BuildName;
            var text = _buildStatuses[0].RequestedBy;
            _taskbarIcon.ShowBalloonTip(title, text, ResourceHelper.GetIcon(_buildStatuses[0]));

            _dispatcherTimer.Start();
        }

        private void AddMenuItems(IBuildStatusProvider statusProvider, List<Uri> uriList)
        {
            _buildStatuses = statusProvider.GetStatusList(uriList);
            foreach (var buildStatus in _buildStatuses)
            {
                var imageName = ResourceHelper.GetImageName(buildStatus);
                var item = new MenuItem
                {
                    Icon = new Image
                    {
                        Source = new BitmapImage(new Uri(imageName, UriKind.Relative))
                    },
                    Header = string.Format("{0} - {1}", buildStatus.BuildName, buildStatus.RequestedBy),
                    Tag = buildStatus.Key
                };
                item.Click += ItemOnClick;
                _contextMenu.Items.Insert(0, item);
            }
        }


        private void ItemOnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.google.dk");
        }

        private void BuildTaskBarIcon()
        {
            _taskbarIcon = new TaskbarIcon
            {
                Icon = Resource.BuildSolution,
                ToolTipText = "TFS Build Notifier",
                MenuActivation = PopupActivationMode.LeftOrRightClick
            };

            var exitItem = new MenuItem {Header = "Exit"};
            exitItem.Click += ExitItem_Click;
            _contextMenu.Items.Add(exitItem);
            _taskbarIcon.ContextMenu = _contextMenu;
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            _isExit = true;
            Current.Shutdown();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                MainWindow.Hide(); // A hidden window can be shown again, a closed one not
            }
        }
    }
}
