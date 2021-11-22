﻿using Autofac;
using FriendStorage.DataAccess;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Startup;
using FriendStorage.UI.View;
using FriendStorage.UI.ViewModel;
using System.Windows;

namespace FriendStorage.UI
{
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      var bootStrapper = new BootStrapper();
      var container = bootStrapper.Bootstrap();

      var mainWindow = container.Resolve<MainWindow>();
      mainWindow.Show();
    }
  }
}
