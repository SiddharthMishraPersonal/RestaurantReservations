using Restaurant.Reservations.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Restaurant.Reservations.View;
using System.Windows;
using Restaurant.Reservations.Shared.Helper;
using Restaurant.Reservations.Shared.Models;
using Restaurant.Reservations.Shared.Log;

namespace Restaurant.Reservations.ViewModel
{
  public class ApplicationViewModel : ViewModelBase
  {
    #region Private Member Variables

    private readonly int _monthRange;
    private bool _isBusy;
    private DateTime _selectedDate;
    private DateTime _startDate;
    private DateTime _endDate;
    private MainWindow _view;
    private Func<ReservationViewModel> _reservationViewModel;
    private Func<TableViewModel> _tableViewModel;
    private SettingsViewModel _settingsViewModel;
    private int _currentReservationCount;
    private int _futureReservationCount;
    private int _usedTablesCount;
    private int _availableTablesCount;

    private ReservationViewModel _selectedReservation;

    private ObservableCollection<ReservationViewModel> _reservations =
      new ObservableCollection<ReservationViewModel>();

    private ObservableCollection<ReservationViewModel> _todayReservations =
      new ObservableCollection<ReservationViewModel>();

    private ObservableCollection<ReservationViewModel> _futureReservations =
      new ObservableCollection<ReservationViewModel>();


    private ObservableCollection<TableViewModel> _tableList = new ObservableCollection<TableViewModel>();

    #endregion

    #region Properties

    public ObservableCollection<ReservationViewModel> Reservations
    {
      get { return _reservations; }
      set { _reservations = value; }
    }

    public ObservableCollection<ReservationViewModel> TodayReservations
    {
      get { return _todayReservations; }
      set { _todayReservations = value; }
    }

    public ObservableCollection<ReservationViewModel> FutureReservations
    {
      get { return _futureReservations; }
      set { _futureReservations = value; }
    }

    public ObservableCollection<TableViewModel> TableList
    {
      get { return _tableList; }
      set { _tableList = value; }
    }

    public bool IsBusy
    {
      get { return _isBusy; }
      set
      {
        _isBusy = value;
        OnPropertyChanged("IsBusy");
      }
    }


    public ReservationViewModel SelectedReservation
    {
      get { return _selectedReservation; }
      set
      {
        _selectedReservation = value;
        OnPropertyChanged("SelectedReservation");
      }
    }

    public DateTime SelectedDate
    {
      get { return _selectedDate; }
      set
      {
        _selectedDate = value;
        OnPropertyChanged("SelectedDate");
      }
    }

    public DateTime StartDate
    {
      get { return _startDate; }
      set
      {
        _startDate = value;
        OnPropertyChanged("StartDate");
        EndDate = _startDate.AddMonths(_monthRange);
      }
    }

    public DateTime EndDate
    {
      get { return _endDate; }
      set
      {
        _endDate = value;
        OnPropertyChanged("EndDate");
      }
    }

    public int CurrentReservationCount
    {
      get { return _currentReservationCount; }
      set
      {
        _currentReservationCount = value;
        OnPropertyChanged("CurrentReservationCount");
      }
    }

    public int FutureReservationCount
    {
      get { return _futureReservationCount; }
      set
      {
        _futureReservationCount = value;
        OnPropertyChanged("FutureReservationCount");
      }
    }

    public int UsedTablesCount
    {
      get { return _usedTablesCount; }
      set
      {
        _usedTablesCount = value;
        OnPropertyChanged("UsedTablesCount");
      }
    }

    public int AvailableTablesCount
    {
      get { return _availableTablesCount; }
      set
      {
        _availableTablesCount = value;
        OnPropertyChanged("AvailableTablesCount");
      }
    }

    #endregion

    #region Constructors

    public ApplicationViewModel(MainWindow view, SettingsViewModel settingsViewModel,
      Func<ReservationViewModel> reservationViewModel, Func<TableViewModel> tableViewModel)
    {
      try
      {
        _view = view;
        _view.Closed += (sender, args) =>
        {
          SaveReservationsAsync();
          Application.Current.Shutdown(1);
        };
        _reservationViewModel = reservationViewModel;
        _tableViewModel = tableViewModel;
        _settingsViewModel = settingsViewModel;
        _monthRange = 12;
        StartDate = DateTime.Now;
        var defaultAppPath =
          Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Reservations");
        if (!Directory.Exists(defaultAppPath))
        {
          Directory.CreateDirectory(defaultAppPath);
        }
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
      }
    }

    #endregion

    #region Commands

    #region New Reservation Command

    private ICommand _createNewReservationCommand;

    public ICommand CreateNewReservationCommand
    {
      get
      {
        _createNewReservationCommand = _createNewReservationCommand ??
                                       new RelayCommands(CreateNewReservationCommand_Execute,
                                         CreateNewReservationCommand_CanExecute);
        return _createNewReservationCommand;
      }
    }

    private void CreateNewReservationCommand_Execute(object param)
    {
      try
      {
        var currentTime = DateTime.Now.TimeOfDay;
        var openTime = DateTime.Parse("10:00 AM").TimeOfDay;
        var closeTime = DateTime.Parse("10:00 PM").TimeOfDay;

        var areWeOpen = currentTime >= openTime && currentTime <= closeTime;

        if (AvailableTablesCount == 0 && UsedTablesCount > 0)
        {
          _view.ShowMessageAsync("House Full!!",
            "All tables are booked. Please remove a reservation before creating new one.",
            MessageDialogStyle.Affirmative,
            new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
          return;
        }

        if (!areWeOpen)
        {
          var resultTask = _view.ShowMessageAsync("Closed!!",
            "We are closed for the day.\r\nWe are open between 10 A.M. to 10 P.M.\r\n\nDo you still want to continue?",
            MessageDialogStyle.AffirmativeAndNegative,
            new MetroDialogSettings() {AffirmativeButtonText = "Yes", NegativeButtonText = "No"});
          resultTask.ContinueWith(s =>
          {
            if (s.Result != MessageDialogResult.Affirmative)
              return;
            else
            {
              var newReservationVm = _reservationViewModel();
              newReservationVm.ShowWindow(this._view);
            }
          }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        else
        {
          var newReservationVm = _reservationViewModel();
          newReservationVm.ShowWindow(this._view);
        }
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        _view.ShowMessageAsync("Error",
          exception.Message,
          MessageDialogStyle.Affirmative,
          new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
      }
    }

    private bool CreateNewReservationCommand_CanExecute(object param)
    {
      return true;
    }

    #endregion

    #region Delete Reservation

    private ICommand _deleteReservationCommand;

    public ICommand DeleteReservationCommand
    {
      get
      {
        _deleteReservationCommand = _deleteReservationCommand ??
                                    new RelayCommands(DeleteReservationCommand_Execute,
                                      DeleteReservationCommand_CanExecute);
        return _deleteReservationCommand;
      }
    }

    private void DeleteReservationCommand_Execute(object param)
    {
      try
      {
        if (SelectedReservation == null)
        {
          _view.ShowMessageAsync("Not Selected!", "No reservation is selected for deletion.",
            MessageDialogStyle.Affirmative,
            new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
        }
        else
        {
          var resultTask = _view.ShowMessageAsync("Delete?",
            string.Format("Do you want to delete {0}'s reservation?", SelectedReservation.CustomerName),
            MessageDialogStyle.AffirmativeAndNegative,
            new MetroDialogSettings() {AffirmativeButtonText = "Yes", NegativeButtonText = "No"});

          resultTask.ContinueWith(s =>
          {
            if (s.Result.Equals(MessageDialogResult.Affirmative))
            {
              RemoveReservationAsync(SelectedReservation);
            }
          });
        }
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        _view.ShowMessageAsync("Error",
          exception.Message,
          MessageDialogStyle.Affirmative,
          new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
      }
    }

    private bool DeleteReservationCommand_CanExecute(object param)
    {
      return SelectedReservation != null;
    }

    #endregion

    #region Update Reservation

    private ICommand _updateReservationCommand;

    public ICommand UpdateReservationCommand
    {
      get
      {
        _updateReservationCommand = _updateReservationCommand ??
                                    new RelayCommands(UpdateReservationCommand_Execute,
                                      UpdateReservationCommand_CanExecute);
        return _updateReservationCommand;
      }
    }

    private void UpdateReservationCommand_Execute(object param)
    {
      try
      {
        if (SelectedReservation == null)
        {
          _view.ShowMessageAsync("Not Selected!", "No reservation is selected for deletion.",
            MessageDialogStyle.Affirmative,
            new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
        }
        else
        {
          var resultTask = _view.ShowMessageAsync("Update?",
            string.Format("Do you want to update {0}'s reservation details?", SelectedReservation.CustomerName),
            MessageDialogStyle.AffirmativeAndNegative,
            new MetroDialogSettings() {AffirmativeButtonText = "Yes", NegativeButtonText = "No"});

          resultTask.ContinueWith(s =>
          {
            if (s.Result.Equals(MessageDialogResult.Affirmative))
            {
              UpdateReservation(SelectedReservation);
            }
          });
        }
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        _view.ShowMessageAsync("Error",
          exception.Message,
          MessageDialogStyle.Affirmative,
          new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
      }
    }

    private bool UpdateReservationCommand_CanExecute(object param)
    {
      return SelectedReservation != null;
    }

    #endregion

    #region Refresh Reservation Command

    private ICommand _refreshReservationCommand;

    public ICommand RefreshReservationCommand
    {
      get
      {
        _refreshReservationCommand = _refreshReservationCommand ??
                                     new RelayCommands(RefreshReservationCommand_Execute,
                                       RefreshReservationCommand_CanExecute);
        return _refreshReservationCommand;
      }
    }

    private void RefreshReservationCommand_Execute(object param)
    {
      try
      {
        //We should put load data methods in different thread. Here data is not that big so we won't expect any delay on UI refreshing.
        Task.Factory.StartNew(() =>
        {
          IsBusy = true;
          //Deliberately put this sleep to demonstrate responsiveness of UI if save method takes more time than estimated.
          Thread.Sleep(2000);
          LoadReservationsAsync();
          IsBusy = false;
        });
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        _view.ShowMessageAsync("Error",
          exception.Message,
          MessageDialogStyle.Affirmative,
          new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
      }
    }

    private bool RefreshReservationCommand_CanExecute(object param)
    {
      return true;
    }

    #endregion

    #region Settings Command

    private ICommand _settingsCommand;

    public ICommand SettingsCommand
    {
      get
      {
        _settingsCommand = _settingsCommand ?? new RelayCommands(SettingsCommand_Execute, SettingsCommand_CanExecute);
        return _settingsCommand;
      }
    }

    private void SettingsCommand_Execute(object param)
    {
      try
      {
        _settingsViewModel.ShowWindow(this._view);
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        _view.ShowMessageAsync("Error",
          exception.Message,
          MessageDialogStyle.Affirmative,
          new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
      }
    }

    private bool SettingsCommand_CanExecute(object param)
    {
      return true;
    }

    #endregion

    #region Close Application Command

    private ICommand _closeApplicationCommand;

    public ICommand CloseApplicationCommand
    {
      get
      {
        _closeApplicationCommand = _closeApplicationCommand ??
                                   new RelayCommands(CloseApplicationCommand_Execute, CloseApplicationCommand_CanExecute);
        return _closeApplicationCommand;
      }
    }

    private void CloseApplicationCommand_Execute(object param)
    {
      try
      {
        var resultTask = _view.ShowMessageAsync("Reservation System", "Do you want to close application?",
          MessageDialogStyle.AffirmativeAndNegative,
          new MetroDialogSettings() {AffirmativeButtonText = "Yes", NegativeButtonText = "No"});

        resultTask.ContinueWith(t =>
        {
          if (t.Result != MessageDialogResult.Affirmative)
            return;

          SaveReservationsAsync();
          _view.Close();
        }, TaskScheduler.FromCurrentSynchronizationContext());
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        _view.ShowMessageAsync("Error",
          exception.Message,
          MessageDialogStyle.Affirmative,
          new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
      }
    }

    private bool CloseApplicationCommand_CanExecute(object param)
    {
      return true;
    }

    #endregion

    #endregion

    #region Public Methods

    internal void ShowWindow()
    {
      if (this._view == null)
        return;

      this._view.DataContext = this;
      this._view.Show();
      this.LoadTableDetails();
      if (File.Exists(_settingsViewModel.ReservationFileFullpath))
      {
        Task.Factory.StartNew(() =>
        {
          IsBusy = true;
          //Deliberately put this sleep to demonstrate responsiveness of UI if save method takes more time than estimated.
          Thread.Sleep(2000);
          LoadReservationsAsync();
          IsBusy = false;
        });
      }
    }

    public void AddReservation(ReservationViewModel reservationViewModel, bool canSaveToFile = false)
    {
      _view.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (!Reservations.Contains(reservationViewModel))
        {
          this.AddReservationToCollection(reservationViewModel);
        }

        GetTodayReservations();
        GetFutureReservations();
        GetTablesStatus();

        if (canSaveToFile)
          SaveReservationsAsync();
      }));
    }

    public void RemoveReservationAsync(ReservationViewModel reservationViewModel)
    {
      _view.Dispatcher.BeginInvoke((Action) ((() => { this.RemoveReservationFromCollection(reservationViewModel); })));

      SaveReservationsAsync();
    }

    public void UpdateReservation(ReservationViewModel reservationViewModel)
    {
      reservationViewModel.ShowWindow(this._view, true);
      GetReservationCounts();

      //Count the table
      GetTablesStatus();

      SaveReservationsAsync();
    }

    #endregion

    #region Private Methods

    private void LoadTableDetails()
    {
      try
      {
        var xmlFilePath = string.Empty;
        if (_settingsViewModel != null)
        {
          xmlFilePath = _settingsViewModel.TableFilePath;
          if (!File.Exists(xmlFilePath))
          {
            var resultTask = _view.ShowMessageAsync("Settings",
              "Table data doesn't exist. Please visit 'Settings' window and update file paths.",
              MessageDialogStyle.Affirmative,
              new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
            resultTask.ContinueWith(s =>
            {
              if (s.Result == MessageDialogResult.Affirmative)
              {
                _settingsViewModel.ShowWindow(this._view);
                LoadTableDetails();
              }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            return;
          }
        }

        _tableList.Clear();
        var tableModelList = XmlOperations.DeSerializeTableList(xmlFilePath);

        foreach (var tableModel in tableModelList)
        {
          var tableVm = _tableViewModel();
          tableVm.TableNumber = tableModel.Id;
          tableVm.MaxOccupancy = tableModel.MaxOccupancy;
          _tableList.Add(tableVm);
        }
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        _view.ShowMessageAsync("Error",
          exception.Message,
          MessageDialogStyle.Affirmative,
          new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
      }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Saves data in different non-ui thread.
    /// </summary>
    public void SaveReservationsAsync()
    {
      try
      {
        Task.Factory.StartNew(() => { SaveReservations(); });
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        _view.ShowMessageAsync("Error",
          exception.Message,
          MessageDialogStyle.Affirmative,
          new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
      }
    }

    /// <summary>
    /// Save data on UI thread.
    /// </summary>
    public void SaveReservations()
    {
      try
      {
        var reservationList = new ReservationList();
        foreach (var reservationViewModel in Reservations)
        {
          var newReservationModel = new Reservation()
          {
            ContactNumber = reservationViewModel.ContactNumber,
            CheckInDate = reservationViewModel.CheckInDate,
            CustomerName = reservationViewModel.CustomerName,
            Occupants = reservationViewModel.Occupants
          };

          foreach (var tableViewModel in reservationViewModel.TablesSelected)
          {
            var table = new Table()
            {
              Id = tableViewModel.TableNumber,
              MaxOccupancy = tableViewModel.MaxOccupancy
            };
            newReservationModel.Table.Add(table);
          }

          reservationList.TodayReservations.Add(newReservationModel);
        }

        //Serialize the data
        XmlOperations.SerializeReservations(_settingsViewModel.ReservationFileFullpath, reservationList);
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        _view.ShowMessageAsync("Error",
          exception.Message,
          MessageDialogStyle.Affirmative,
          new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
      }
    }

    private void LoadReservations()
    {
      try
      {
        var reservationModelList = XmlOperations.DeSerializeReservationLists(_settingsViewModel.ReservationFileFullpath);
        foreach (var resModel in reservationModelList)
        {
          var resVm = _reservationViewModel();
          resVm.CustomerName = resModel.CustomerName;
          resVm.ContactNumber = resModel.ContactNumber;
          resVm.CheckInDate = resModel.CheckInDate;
          resVm.CheckInTime = resModel.CheckInDate.TimeOfDay;
          resVm.ReservationGuid = Guid.NewGuid();

          foreach (var tableModel in resModel.Table)
          {
            resVm.MaxOccupancy += tableModel.MaxOccupancy;
            var tableVm =
              TableList.FirstOrDefault(
                s => s.TableNumber.Equals(tableModel.Id) && s.MaxOccupancy.Equals(tableModel.MaxOccupancy));
            if (tableVm == null)
              continue;

            tableVm.IsSelected = true;
            resVm.TablesSelected.Add(tableVm);
          }

          resVm.Occupants = resModel.Occupants;
          resVm.GetTableSelectedString();
          AddReservation(resVm);
        }

        GetTablesStatus();
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        _view.ShowMessageAsync("Error",
          exception.Message,
          MessageDialogStyle.Affirmative,
          new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
      }
    }

    private void LoadReservationsAsync()
    {
      try
      {
        Reservations.Clear();
        Task.Factory.StartNew(() => { this._view.Dispatcher.BeginInvoke((Action) (() => { LoadReservations(); })); });
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        _view.ShowMessageAsync("Error",
          exception.Message,
          MessageDialogStyle.Affirmative,
          new MetroDialogSettings() {AffirmativeButtonText = "Ok", NegativeButtonText = "No"});
      }
    }

    private void AddReservationToCollection(ReservationViewModel newReservationVm)
    {
      var existingReservation =
        Reservations.FirstOrDefault(s => s.ReservationGuid.Equals(newReservationVm.ReservationGuid));

      if (existingReservation != null)
      {
        Reservations.Remove(existingReservation);
      }
      Reservations.Add(newReservationVm);

      //_view.Dispatcher.BeginInvoke((Action) (() => { Reservations.Add(newReservationVm); }));
      GetTodayReservations();
      GetFutureReservations();

      GetReservationCounts();
    }

    private void GetReservationCounts()
    {
      CurrentReservationCount = TodayReservations.Count;
      FutureReservationCount = FutureReservations.Count;
    }

    private void RemoveReservationFromCollection(ReservationViewModel reservationViewModel)
    {
      try
      {
        foreach (var tableViewModel in reservationViewModel.TablesSelected)
        {
          tableViewModel.IsSelected = false;
          var table =
            TableList.FirstOrDefault(
              s =>
                s.TableNumber.Equals(tableViewModel.TableNumber) && s.MaxOccupancy.Equals(tableViewModel.MaxOccupancy));
          if (table != null)
          {
            table.IsSelected = false;
          }
        }

        GetTablesStatus();

        if (TodayReservations.Contains(reservationViewModel))
        {
          _view.Dispatcher.BeginInvoke((Action) (() => { TodayReservations.Remove(reservationViewModel); }));
        }

        if (FutureReservations.Contains(reservationViewModel))
        {
          _view.Dispatcher.BeginInvoke((Action) (() => { FutureReservations.Remove(reservationViewModel); }));
        }

        if (Reservations.Contains(reservationViewModel))
        {
          _view.Dispatcher.BeginInvoke((Action) (() => { Reservations.Remove(reservationViewModel); }));
        }

        GetReservationCounts();


        SaveReservationsAsync();
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
      }
    }

    private void GetTodayReservations()
    {
      var todayReservation =
        Reservations.Where(s => s.CheckInDate.Date.Equals(DateTime.Now.Date));
      var newlyAdded =
        todayReservation.Where(t => !TodayReservations.Any(s => s.ReservationGuid.Equals(t.ReservationGuid)));
      TodayReservations.Clear();
      foreach (var newReservation in newlyAdded)
      {
        TodayReservations.Add(newReservation);
      }
    }

    private void GetFutureReservations()
    {
      var todayReservation =
        Reservations.Where(s => s.CheckInDate.Date > (DateTime.Now.Date));
      var newlyAdded =
        todayReservation.Where(t => !TodayReservations.Any(s => s.ReservationGuid.Equals(t.ReservationGuid)));
      FutureReservations.Clear();
      foreach (var newReservation in newlyAdded)
      {
        FutureReservations.Add(newReservation);
      }
    }

    private void GetTablesStatus()
    {
      UsedTablesCount = TableList.Count(s => s.IsSelected == true);
      AvailableTablesCount = TableList.Count(s => s.IsSelected == false);
    }

    #endregion
  }
}