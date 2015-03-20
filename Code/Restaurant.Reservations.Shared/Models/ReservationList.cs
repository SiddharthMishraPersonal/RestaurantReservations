using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Restaurant.Reservations.Shared.Models
{
  [Serializable, XmlRoot("Reservations"), XmlType("Reservations")]
  public class ReservationList
  {
    private List<Reservation> _todayReservations = null;

    public ReservationList()
    {
      _todayReservations = new List<Reservation>();
    }

    [XmlElement("Reservation")]
    public List<Reservation> TodayReservations
    {
      get { return _todayReservations; }
      set { _todayReservations = value; }
    }
  }
}