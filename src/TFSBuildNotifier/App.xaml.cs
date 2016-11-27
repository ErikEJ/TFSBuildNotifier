using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using TFSBuildNotifier.TfsBuildStatusProvider;

namespace TFSBuildNotifier
{
    public partial class App
    {
        private bool _isExit;
        private TaskbarIcon _taskbarIcon;
        private readonly ContextMenu _contextMenu = new ContextMenu();
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private TfsStatusProvider _statusProvider;
        private List<BuildStatus> _buildStatuses;
        private readonly List<Uri> _uriList = new List<Uri>();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.Closing += MainWindow_Closing;
            BuildTaskBarIcon();
            _statusProvider = new TfsStatusProvider();
            if (e.Args.Length == 0)
            {
                MessageBox.Show("Invalid command line", ResourceHelper.AppName, MessageBoxButton.OK);
                Current.Shutdown();
            }
            try
            {
                foreach (var arg in e.Args)
                {
                    _uriList.Add(new Uri(arg));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid command line", ResourceHelper.AppName, MessageBoxButton.OK);
                Current.Shutdown();
            }

            AddMenuItems(_uriList);
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

            var latestStatuses = _statusProvider.GetStatusList(_uriList);
            foreach (var newStatus in latestStatuses)
            {
                var oldStatus = _buildStatuses.Single(b => b.Key == newStatus.Key);
                if (newStatus.BuildId != oldStatus.BuildId)
                {
                    var title = newStatus.BuildName;
                    var text = newStatus.RequestedBy;
                    _taskbarIcon.ShowBalloonTip(title, text, ResourceHelper.GetIcon(newStatus), true);

                    foreach (var contextMenuItem in _contextMenu.Items)
                    {
                        var menuItem = contextMenuItem as MenuItem;
                        var tag = menuItem?.Tag as Uri;
                        if (tag == null || tag != oldStatus.Key) continue;
                        MenuItemHelper.ConfigureMenuItem(menuItem, newStatus);
                    }
                }
                oldStatus.Status = newStatus.Status;
                oldStatus.BuildId = newStatus.BuildId;
                oldStatus.RequestedBy = newStatus.RequestedBy;
            }
            _dispatcherTimer.Start();
        }

        private void AddMenuItems(List<Uri> uriList)
        {
            _buildStatuses = _statusProvider.GetStatusList(uriList);
            foreach (var buildStatus in _buildStatuses)
            {
                var item = new MenuItem();
                MenuItemHelper.ConfigureMenuItem(item, buildStatus);
                item.Click += ItemOnClick;
                _contextMenu.Items.Insert(0, item);
            }
        }

        private void ItemOnClick(object sender, RoutedEventArgs e)
        {
            MenuItemHelper.LaunchUrl(sender as MenuItem, _buildStatuses);
        }

        private void BuildTaskBarIcon()
        {
            _taskbarIcon = new TaskbarIcon
            {
                Icon = Resource.BuildSolution,
                ToolTipText = ResourceHelper.AppName,
                MenuActivation = PopupActivationMode.LeftOrRightClick
            };

            var exitItem = new MenuItem {Header = "Exit"};
            exitItem.Click += ExitItem_Click;

            var launchItem = new MenuItem
            {
                Header = "Launch at Startup",
                IsCheckable = true,
                IsChecked = MenuItemHelper.GetStartupStatus()
            };
            MenuItemHelper.SetStartup(launchItem.IsChecked);
            launchItem.Click += LaunchItem_Click;

            if (MenuItemHelper.CheckVersion())
            {
                var newVersionItem = new MenuItem
                {
                    Header = "Update available",
                    Icon = Resource.StatusWarning
                };
                newVersionItem.Click += NewVersionItem_Click;
                _contextMenu.Items.Add(newVersionItem);
            }

            _contextMenu.Items.Add(launchItem);
            _contextMenu.Items.Add(exitItem);
            _taskbarIcon.ContextMenu = _contextMenu;
        }

        private void NewVersionItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void LaunchItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            MenuItemHelper.SetStartup(menuItem != null && menuItem.IsChecked);
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
