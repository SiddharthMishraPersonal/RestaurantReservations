using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Restaurant.Reservations.Shared.Models
{
  [Serializable, XmlRoot("Settings"), XmlType("Settings")]
  public class SettingsModel
  {
    [XmlElement("TableFilePath")]
    public string TableFilePath { get; set; }

    [XmlElement("ReservationFileName")]
    public string ReservationFileName { get; set; }

    [XmlElement("ReservationFileLocation")]
    public string ReservationFileLocation { get; set; }
  }
}