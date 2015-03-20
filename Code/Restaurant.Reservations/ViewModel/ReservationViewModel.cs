using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Restaurant.Reservations.Helper;
using Restaurant.Reservations.Shared.Log;
using Restaurant.Reservations.View;

namespace Restaurant.Reservations.ViewModel
{
  public class ReservationViewModel : ViewModelBase
  {
    #region Private Member Variables

    private Guid _reservationGuid = Guid.Empty;
    private ObservableCollection<TableViewModel> _tablesAvaialble = new ObservableCollection<TableViewModel>();
    private ObservableCollection<TableViewModel> _tablesSelected = new ObservableCollection<TableViewModel>();
    private string _selectedTableString;
    private TableViewModel _selectedTable;
    private string _customerName;
    private string _contactNumber;
    private int _occupants;
    private DateTime _startDate;
    private DateTime _endDate;
    private DateTime _checkInDate;
    private DateTime _selectedDate;
    private TimeSpan _checkInTime;
    private double _maxOccupancy;
    private readonly int _monthRange;
    private Func<NewReservation> _viewFunc;
    private NewReservation _view;
    private ApplicationViewModel _applicationViewModel;
    private bool _isSelected;

    #region Timer Up Down variable

    private DateTime _currentTime = DateTime.Now;
    private bool _adHours;
    private bool _addMinutes;
    private ObservableCollection<string> _amPmTypes = new ObservableCollection<string>();
    private string _displayAmPm;
    private string _selectedTime;

    #endregion

    #endregion

    #region Properties

    public Guid ReservationGuid
    {
      get { return _reservationGuid; }
      set
      {
        _reservationGuid = value;

        OnPropertyChanged("ReservationGuid");
      }
    }

    public double MaxOccupancy
    {
      get { return _maxOccupancy; }
      set
      {
        _maxOccupancy = value;
        OnPropertyChanged("MaxOccupancy");
      }
    }

    public ObservableCollection<TableViewModel> TablesAvaialble
    {
      get { return _tablesAvaialble; }
      set
      {
        _tablesAvaialble = value;
        OnPropertyChanged("TablesAvaialble");
      }
    }

    public string CustomerName
    {
      get { return _customerName; }
      set
      {
        _customerName = value;
        OnPropertyChanged("CustomerName");
      }
    }

    public string ContactNumber
    {
      get { return _contactNumber; }
      set
      {
        _contactNumber = value;
        OnPropertyChanged("ContactNumber");
      }
    }

    public string SelectedTableString
    {
      get { return _selectedTableString; }
      set
      {
        _selectedTableString = value;
        OnPropertyChanged("SelectedTableString");
      }
    }

    public TableViewModel SelectedTable
    {
      get { return _selectedTable; }
      set
      {
        _selectedTable = value;

        OnPropertyChanged("SelectedTable");
      }
    }

    public int Occupants
    {
      get { return _occupants; }
      set
      {
        if (value > MaxOccupancy)
        {
          _occupants = (int) MaxOccupancy;
        }
        else
        {
          _occupants = value;
        }
        OnPropertyChanged("Occupants");
      }
    }

    public DateTime CheckInDate
    {
      get { return _checkInDate; }
      set
      {
        _checkInDate = value;
        _selectedDate = _checkInDate;

        OnPropertyChanged("SelectedDate");
        OnPropertyChanged("CheckInDate");
        OnPropertyChanged("CheckInTime");
      }
    }

    public DateTime SelectedDate
    {
      get { return _selectedDate; }
      set
      {
        _selectedDate = value;
        _checkInDate = _selectedDate.Date;
        _checkInDate = SelectedDate.Date.Add(CheckInTime);
        OnPropertyChanged("SelectedDate");
        OnPropertyChanged("CheckInDate");
        OnPropertyChanged("CheckInTime");
      }
    }

    public TimeSpan CheckInTime
    {
      get { return _checkInTime; }
      set
      {
        _checkInTime = value;
        CheckInDate = SelectedDate.Date.Add(CheckInTime);
        _currentTime = CheckInDate;
        OnPropertyChanged("SelectedDate");
        OnPropertyChanged("CheckInDate");
        OnPropertyChanged("CheckInTime");
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

    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        _isSelected = value;
        OnPropertyChanged("IsSelected");
      }
    }

    public ObservableCollection<TableViewModel> TablesSelected
    {
      get { return _tablesSelected; }
      set { _tablesSelected = value; }
    }

    #region Timer Up Down Public Properties

    public ObservableCollection<string> AmPmTypes
    {
      get { return _amPmTypes; }
      set { _amPmTypes = value; }
    }

    public string DisplayTime
    {
      get { return _currentTime.ToString("t"); }
    }

    public string DisplayAmPm
    {
      get
      {
        if (_currentTime.Hour >= 0 && _currentTime.Hour < 12)
          _displayAmPm = AmPmTypes.FirstOrDefault(s => s.Equals("AM"));
        else
        {
          if (_currentTime.Hour >= 12)
          {
            _displayAmPm = AmPmTypes.FirstOrDefault(s => s.Equals("PM"));
          }
        }

        return _displayAmPm;
      }
      set
      {
        if (!value.Equals(_displayAmPm))
        {
          if (value.Equals("PM"))
          {
            CurrentTime = CurrentTime.AddHours(12);
            if (CurrentTime.Hour >= 22)
            {
              CurrentTime =
                new DateTime(CheckInDate.Year, CheckInDate.Month, CheckInDate.Day, 21, 30, 00);
            }
            if (CurrentTime.Hour < 10)
            {
              CurrentTime =
                new DateTime(CheckInDate.Year, CheckInDate.Month, CheckInDate.Day, 10, 00, 00);
            }
          }
          else
          {
            CurrentTime = CurrentTime.AddHours(-12);
            if (CurrentTime.Hour >= 22)
            {
              CurrentTime =
                new DateTime(CheckInDate.Year, CheckInDate.Month, CheckInDate.Day, 21, 30, 00);
            }
            if (CurrentTime.Hour < 10)
            {
              CurrentTime =
                new DateTime(CheckInDate.Year, CheckInDate.Month, CheckInDate.Day, 10, 00, 00);
            }
          }
        }
        _displayAmPm = value;
      }
    }

    public string DisplayTimeHours
    {
      get
      {
        var hours = _currentTime.Hour;
        return hours > 12 ? (hours - 12).ToString("00") : hours.ToString("00");
        //return hours.ToString();
      }
      set
      {
        var hour = 0;
        Int32.TryParse(value, out hour);
        CurrentTime = CurrentTime.AddHours(hour);
        OnPropertyChanged("DisplayTime");
        OnPropertyChanged("DisplayTimeHours");
        OnPropertyChanged("DisplayTimeMinutes");
      }
    }

    public string DisplayTimeMinutes
    {
      get { return _currentTime.Minute.ToString("00"); }
      set
      {
        var minutes = 0;
        Int32.TryParse(value, out minutes);
        CurrentTime = CurrentTime.AddMinutes(minutes);
        OnPropertyChanged("DisplayTime");
        OnPropertyChanged("DisplayTimeHours");
        OnPropertyChanged("DisplayTimeMinutes");
      }
    }

    public DateTime CurrentTime
    {
      get { return _currentTime; }
      set
      {
        _currentTime = value;

        OnPropertyChanged("CurrentTime");
        OnPropertyChanged("DisplayTime");
        OnPropertyChanged("DisplayTimeHours");
        OnPropertyChanged("DisplayTimeMinutes");
        OnPropertyChanged("DisplayAmPm");
        SelectedTime = value.ToString("t");
        CheckInTime = CurrentTime.TimeOfDay;
      }
    }

    public string SelectedTime
    {
      get { return _selectedTime; }
      set
      {
        _selectedTime = value;
        OnPropertyChanged("SelectedTime");
      }
    }

    #endregion

    #endregion

    #region Constructors

    /// <summary>
    /// Every time when this view model class will get instantiated we will get new View object.
    /// </summary>
    /// <param name="view"></param>
    /// <param name="applicationViewModel"></param>
    public ReservationViewModel(Func<NewReservation> view, ApplicationViewModel applicationViewModel)
    {
      _applicationViewModel = applicationViewModel;
      _viewFunc = view;
      _view = _viewFunc();
      _view.DataContext = this;


      this.CheckInDate = DateTime.Now.Date;
      if (_view == null)
      {
        return;
      }

      #region Time Up Down

      AmPmTypes.Add("AM");
      AmPmTypes.Add("PM");
      CurrentTime = DateTime.Now;
      SelectedTime = CurrentTime.ToString("t");

      #endregion

      _monthRange = 12;
      StartDate = DateTime.Now;

      CheckInTime = DateTime.Now.TimeOfDay;
    }

    #endregion

    #region Commands

    #region Save Command

    private ICommand _saveCommand;

    public ICommand SaveCommand
    {
      get
      {
        _saveCommand = _saveCommand ?? new RelayCommands(SaveCommand_Execute, SaveCommand_CanExecute);
        return _saveCommand;
      }
    }

    private void SaveCommand_Execute(object param)
    {
      try
      {
        //Check whether reservation is between 10AM to 10PM.
        var currentTime = DateTime.Now.TimeOfDay;
        var openTime = DateTime.Parse("10:00 AM").TimeOfDay;
        var closeTime = DateTime.Parse("10:00 PM").TimeOfDay;

#if DEBUG
        currentTime = new TimeSpan(10, 35, 33);
#endif


        var areWeOpen = currentTime >= openTime && currentTime <= closeTime;

        if (!areWeOpen)
        {
          _view.ShowMessageAsync("New Reservation",
            "We are closed.\r\nWe are open between 10 A.M. to 10 P.M.\r\n\nPlease try later.");
          return;
        }

        //Can select multiple tables
        //

        var resultTask = _view.ShowMessageAsync("Save & Close",
          "Your reservation has been saved.\r\nDo you want to close the window?",
          MessageDialogStyle.AffirmativeAndNegative,
          new MetroDialogSettings() {AffirmativeButtonText = "Yes", NegativeButtonText = "No"});
        resultTask.ContinueWith(s =>
        {
          if (s.Result == MessageDialogResult.Affirmative)
          {
            _applicationViewModel.AddReservation(this, true);

            this._view.Hide();
          }
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

    private bool SaveCommand_CanExecute(object param)
    {
      var closingTime =
        new DateTime(CheckInDate.Year, CheckInDate.Month, CheckInDate.Day, 21, 30, 00).TimeOfDay;
      var inStoreTime = CheckInDate.Hour >= 10 &&
                        CheckInTime <= closingTime;
      var isValid = !string.IsNullOrEmpty(CustomerName) && !string.IsNullOrEmpty(ContactNumber) &&
                    CheckInDate.Date >= DateTime.Now.Date && inStoreTime;
      isValid = isValid && Occupants > 0 && Occupants <= MaxOccupancy;

      return isValid;
    }

    #endregion

    #region Cancel Command

    private ICommand _cancelCommand;

    public ICommand CancelCommand
    {
      get { return _cancelCommand ?? new RelayCommands(CancelCommand_Execute, CancelCommand_CanExecute); }
    }

    private void CancelCommand_Execute(object param)
    {
      try
      {
        var resultTask = _view.ShowMessageAsync("Close",
          "All your reservation information will be lost.\r\nDo you still want to continue?",
          MessageDialogStyle.AffirmativeAndNegative,
          new MetroDialogSettings() {AffirmativeButtonText = "Yes", NegativeButtonText = "No"});

        resultTask.ContinueWith(s =>
        {
          if (s.Result == MessageDialogResult.Affirmative)
          {
            _view.Hide();
          }
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

    private bool CancelCommand_CanExecute(object param)
    {
      return true;
    }

    #endregion

    #region SelectingTable Command

    private ICommand _selectingTableCommand;

    public ICommand SelectingTableCommand
    {
      get
      {
        _selectingTableCommand = _selectingTableCommand ??
                                 new RelayCommands(SelectingTableCommand_Execute, SelectingTableCommand_CanExecute);
        return _selectingTableCommand;
      }
    }

    private void SelectingTableCommand_Execute(object param)
    {
      try
      {
        if (param == null)
          return;

        var table = param as TableViewModel;
        var existingTable = TablesSelected.FirstOrDefault(s => s.TableGuid.Equals(table.TableGuid));
        if (null != existingTable)
        {
          TablesSelected.Remove(existingTable);
        }
        else
        {
          TablesSelected.Add(table);
        }

        MaxOccupancy = 0;
        GetTableSelectedString();
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

    public void GetTableSelectedString()
    {
      var comma = "";
      SelectedTableString = string.Empty;
      foreach (var tableViewModel in TablesSelected)
      {
        SelectedTableString = String.Format("{0}{1} {2}", SelectedTableString, comma, tableViewModel.TableNumber);
        comma = ",";
        MaxOccupancy += tableViewModel.MaxOccupancy;
      }
    }

    private bool SelectingTableCommand_CanExecute(object param)
    {
      return true;
    }

    #endregion

    #region Timer Up Down Commands

    #region AddHours Command

    private ICommand _addHoursCommand;

    public ICommand AddHoursCommand
    {
      get
      {
        _addHoursCommand = _addHoursCommand ?? new RelayCommands(AddHoursCommand_Execute, AddHoursCommand_CanExecute);
        return _addHoursCommand;
      }
    }

    private void AddHoursCommand_Execute(object param)
    {
      try
      {
        if (null == param)
          return;
        int code = Convert.ToInt32(param.ToString());

        if (code == 1)
        {
          //Add hours
          CurrentTime = CurrentTime.AddHours(1);

          if (CurrentTime.Hour >= 22)
          {
            CurrentTime = new DateTime(CheckInDate.Year, CheckInDate.Month, CheckInDate.Day, 21, 30, 00);
            return;
          }
        }

        if (code == 0)
        {
          //We can't allow user to reserve table in past.
          if (CurrentTime.Date.Equals(DateTime.Now.Date) && CurrentTime.Hour <= DateTime.Now.Hour)
            return;

          //Subtract Hours
          CurrentTime = CurrentTime.AddHours(-1);

          if (CurrentTime.Hour < 10)
          {
            CurrentTime = new DateTime(CheckInDate.Year, CheckInDate.Month, CheckInDate.Day, 10, 00, 00);
            return;
          }
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

    private bool AddHoursCommand_CanExecute(object param)
    {
      return true;
    }

    #endregion

    #region Add Minutes Command

    private ICommand _addMinutesCommand;

    public ICommand AddMinutesCommand
    {
      get
      {
        _addMinutesCommand = _addMinutesCommand ??
                             new RelayCommands(AddMinutesCommand_Execute, AddMinutesCommand_CanExecute);
        return _addMinutesCommand;
      }
    }

    private void AddMinutesCommand_Execute(object param)
    {
      try
      {
        if (null == param)
          return;

        int code = Convert.ToInt32(param.ToString());

        if (code == 1)
        {
          //Add Minutes
          CurrentTime = CurrentTime.AddMinutes(15);

          if (CurrentTime.Hour >= 21 && CurrentTime.Minute > 30)
          {
            CurrentTime = new DateTime(CheckInDate.Year, CheckInDate.Month, CheckInDate.Day, 21, 30, 00);
            return;
          }
        }

        if (code == 0)
        {
          //We can't allow user to reserve table in past.
          if (CurrentTime.Date.Equals(DateTime.Now.Date) && CurrentTime.Hour == DateTime.Now.Hour &&
              CurrentTime.Minute <= DateTime.Now.Minute)
            return;

          //Subtract Minutes
          CurrentTime = CurrentTime.AddMinutes(-15);

          if (CurrentTime.Hour < 10 && CurrentTime.Minute <= 59)
          {
            CurrentTime = new DateTime(CheckInDate.Year, CheckInDate.Month, CheckInDate.Day, 10, 00, 00);
            return;
          }
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

    private bool AddMinutesCommand_CanExecute(object param)
    {
      return true;
    }

    #endregion

    #endregion

    #endregion

    #region Public Methods

    public void ShowWindow(Window ownerWindow, bool toUpdate = false)
    {
      try
      {
        _view.Dispatcher.BeginInvoke((Action) (() =>
        {
          _view.Owner = ownerWindow;
          LoadTableData(toUpdate);
          GetTableSelectedString();
          _view.Show();
        }));
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
      }
    }

    private void LoadTableData(bool toUpdate = false)
    {
      try
      {
        TablesAvaialble.Clear();

        var allTables = _applicationViewModel.TableList;
        var reservations = _applicationViewModel.Reservations;

        if (toUpdate)
        {
          foreach (var tableViewModel in allTables)
          {
            TablesAvaialble.Add(tableViewModel);
          }
          return;
        }


        var availableTables =
          allTables.Where(
            s => !reservations.Any(r => r.SelectedTable != null && r.SelectedTable.TableGuid.Equals(s.TableGuid)))
            .ToList();

        if (availableTables == null)
          return;

        foreach (var tableViewModel in availableTables)
        {
          if (!tableViewModel.IsSelected)
            TablesAvaialble.Add(tableViewModel);
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

    #region Private Methods

    #endregion
  }
}