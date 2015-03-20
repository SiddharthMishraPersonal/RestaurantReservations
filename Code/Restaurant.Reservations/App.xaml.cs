using Restaurant.Reservations.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Restaurant.Reservations.AutoFac;
using Restaurant.Reservations.Helper;
using Restaurant.Reservations.Shared.Helper;
using Restaurant.Reservations.Shared.Log;
using Restaurant.Reservations.View;

namespace Restaurant.Reservations
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    #region Private Member Variables

    private MainWindow _view = null;
    private ApplicationViewModel _appViewModel = null;
    private static IContainer _container;
    private static Mutex _applicationMutex = null;

    #endregion

    protected override void OnStartup(StartupEventArgs e)
    {
      try
      {
        if (IsMutexBound())
        {
          NLogger.LogError("Another instance of the application is running");
          MessageBox.Show("Another instance of the application is running.", "Restaurant Reservation System",
            MessageBoxButton.OK, MessageBoxImage.Information);
          Environment.Exit(1);
        }

        base.OnStartup(e);

        var builder = new ContainerBuilder();
        builder.RegisterModule<ApplicationRegisteration>();

        _container = builder.Build();
        _appViewModel = _container.Resolve<ApplicationViewModel>();
        _appViewModel.ShowWindow();
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        MessageBox.Show("An error occurred. Please see the log files.", "Application Error", MessageBoxButton.OK,
          MessageBoxImage.Error);
      }
    }

    private bool IsMutexBound()
    {
      try
      {
        Mutex.OpenExisting(Constants.MutexName);
      }
      catch (WaitHandleCannotBeOpenedException)
      {
        _applicationMutex = new Mutex(true, Constants.MutexName);
        return false;
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
      }
      return true;
    }
  }
}