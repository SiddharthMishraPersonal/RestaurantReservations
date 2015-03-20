using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Restaurant.Reservations.Shared.Models
{
  [XmlType("Reservation")]
  public class Reservation
  {
    #region Private member variables

    private DateTime _checkInDate;
    private string _customerName;
    private string _contactNumber;
    private int _occupants;
    private List<Table> _table;

    #endregion

    #region Constructors

    public Reservation()
    {
      Table = new List<Table>();
    }

    #endregion

    #region Properties

    [XmlElement("CheckInDate")]
    public DateTime CheckInDate
    {
      get { return _checkInDate; }
      set { _checkInDate = value; }
    }


    [XmlElement("CustomerName")]
    public string CustomerName
    {
      get { return _customerName; }
      set { _customerName = value; }
    }

    [XmlElement("ContactNumber")]
    public string ContactNumber
    {
      get { return _contactNumber; }
      set { _contactNumber = value; }
    }

    [XmlElement("Table")]
    public List<Table> Table
    {
      get { return _table; }
      set { _table = value; }
    }

    [XmlElement("Occupants")]
    public int Occupants
    {
      get { return _occupants; }
      set { _occupants = value; }
    }

    #endregion
  }
}