using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Restaurant.Reservations.UserControls
{
  /// <summary>
  /// Interaction logic for ucDateTimeUpDown.xaml
  /// </summary>
  public partial class ucDateTimeUpDown : UserControl, INotifyPropertyChanged
  {
    #region Private Member variable

    private DateTime _currentTime = DateTime.Now;
    private bool _adHours;
    private bool _addMinutes;
    private ObservableCollection<string> _amPmTypes = new ObservableCollection<string>();
    private string _displayAmPm;

    #endregion

    #region Constructors

    public ucDateTimeUpDown()
    {
      InitializeComponent();
      this.DataContext = this;
      AmPmTypes.Add("AM");
      AmPmTypes.Add("PM");
      CurrentTime = DateTime.Now;
      SelectedTime = CurrentTime.ToString("t");
    }

    #endregion

    #region Public Properties

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
            CurrentTime = CurrentTime.AddHours(12);
          else
          {
            CurrentTime = CurrentTime.AddHours(-12);
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
      }
    }

    #endregion

    #region Dependency Properties

    public static readonly DependencyProperty SelectedTimeProperty = DependencyProperty.Register(
      "SelectedTime", typeof (string), typeof (ucDateTimeUpDown), new PropertyMetadata(default(string)));

    public string SelectedTime
    {
      get { return ((DateTime) GetValue(SelectedTimeProperty)).ToString("t"); }
      set { SetValue(SelectedTimeProperty, value); }
    }

    #endregion

    #region Methods

    private void MinutesUpButton_OnClick(object sender, RoutedEventArgs e)
    {
      CurrentTime = CurrentTime.AddMinutes(1);
      SelectedTime = CurrentTime.ToString("t");
    }

    private void MinutesDownButton_OnClick(object sender, RoutedEventArgs e)
    {
      CurrentTime = CurrentTime.AddMinutes(-1);
      SelectedTime = CurrentTime.ToString("t");
    }

    private void HourUpButton_OnClick(object sender, RoutedEventArgs e)
    {
      CurrentTime = CurrentTime.AddHours(1);
      SelectedTime = CurrentTime.ToString("t");
    }

    private void HourDownButton_OnClick(object sender, RoutedEventArgs e)
    {
      CurrentTime = CurrentTime.AddHours(-1);
      SelectedTime = CurrentTime.ToString("t");
    }

    #endregion

    #region INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
      if (null != PropertyChanged)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion
  }
}