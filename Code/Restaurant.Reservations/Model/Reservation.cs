using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Restaurant.Reservations.Model
{
  [XmlType("Reservation")]
  public class Reservation
  {
    #region Private member variables

    private DateTime _date;
    private DateTime _time;
    private string _firstName;
    private string _lastName;
    private string _contactNumber;
    private Table _table;

    #endregion

    #region Constructors

    #endregion

    #region Properties

    [XmlElement("Date")]
    public DateTime Date
    {
      get { return _date; }
      set { _date = value; }
    }

    [XmlElement("Time")]
    public DateTime Time
    {
      get { return _time; }
      set { _time = value; }
    }

    [XmlElement("FirstName")]
    public string FirstName
    {
      get { return _firstName; }
      set { _firstName = value; }
    }

    [XmlElement("LastName")]
    public string LastName
    {
      get { return _lastName; }
      set { _lastName = value; }
    }

    [XmlElement("ContactNumber")]
    public string ContactNumber
    {
      get { return _contactNumber; }
      set { _contactNumber = value; }
    }

    [XmlElement("Table")]
    public Table Table
    {
      get { return _table; }
      set { _table = value; }
    }

    #endregion
  }
}