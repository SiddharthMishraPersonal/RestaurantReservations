using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Reservations.ViewModel
{
  public class TableViewModel : ViewModelBase
  {
    #region Private Member Variables

    private Guid _tableGuid;
    private int _tableNumber;
    private int _maxOccupancy;
    private bool _isSelected;

    #endregion

    #region Properties

    public int TableNumber
    {
      get { return _tableNumber; }
      set
      {
        _tableNumber = value;
        OnPropertyChanged("Id");
      }
    }

    public int MaxOccupancy
    {
      get { return _maxOccupancy; }
      set
      {
        _maxOccupancy = value;
        OnPropertyChanged("MaxOccupancy");
      }
    }

    public Guid TableGuid
    {
      get { return _tableGuid; }
      set
      {
        _tableGuid = value;
        OnPropertyChanged("TableGuid");
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

    #endregion

    #region Constructors

    public TableViewModel()
    {
      TableGuid = Guid.NewGuid();
    }

    public void Initialize(int tableNumber, int maxOccupancy)
    {
      TableNumber = tableNumber;
      MaxOccupancy = maxOccupancy;
    }

    #endregion

    #region Commands

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    #endregion
  }
}