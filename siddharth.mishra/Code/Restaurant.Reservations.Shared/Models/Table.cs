using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Restaurant.Reservations.Shared.Models
{
  [XmlType("Table")]
  public class Table
  {
    #region Private Member Variables

    private int _id;
    private int _maxOccupancy;

    #endregion

    #region Properties

    [XmlAttribute]
    public int MaxOccupancy
    {
      get { return _maxOccupancy; }
      set { _maxOccupancy = value; }
    }

    [XmlAttribute]
    public int Id
    {
      get { return _id; }
      set { _id = value; }
    }

    #endregion
  }
}