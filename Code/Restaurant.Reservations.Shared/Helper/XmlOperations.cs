using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Restaurant.Reservations.Shared.Log;
using Restaurant.Reservations.Shared.Models;


namespace Restaurant.Reservations.Shared.Helper
{
  public static class XmlOperations
  {
    private static string _xmlFilePath;

    public static void SerializeReservations(string xmlFilePath, ReservationList reservationList)
    {
      try
      {
        var serializer = new XmlSerializer(typeof (ReservationList));

        using (var streamWriter = new StreamWriter(xmlFilePath))
        {
          serializer.Serialize(streamWriter, reservationList);
        }
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        throw new Exception(Constants.ErrorSavingReservations);
      }
    }

    public static void SerializeSettings(string xmlFilePath, SettingsModel settingsModel)
    {
      try
      {
        var serializer = new XmlSerializer(typeof (SettingsModel));

        using (var streamWriter = new StreamWriter(xmlFilePath))
        {
          serializer.Serialize(streamWriter, settingsModel);
        }
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        throw new Exception(Constants.ErrorSavingSettings);
      }
    }

    public static List<Table> DeSerializeTableList(string xmlFilePath)
    {
      List<Table> _tableList = null;

      try
      {
        _xmlFilePath = xmlFilePath;

        var xmlSerializer = new XmlSerializer(typeof (TableList));
        using (var fileStream = new FileStream(xmlFilePath, FileMode.Open))
        {
          var tableModelsObj = (TableList) xmlSerializer.Deserialize(fileStream);

          if (tableModelsObj != null)
            _tableList = tableModelsObj.ListTableModel;
        }
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        throw new Exception(Constants.ErrorLoadingTable);
      }

      return _tableList;
    }

    public static List<Reservation> DeSerializeReservationLists(string xmlFilePath)
    {
      List<Reservation> _reservationList = null;

      try
      {
        _xmlFilePath = xmlFilePath;

        var xmlSerializer = new XmlSerializer(typeof (ReservationList));
        using (var fileStream = new FileStream(xmlFilePath, FileMode.Open))
        {
          var tableModelsObj = (ReservationList) xmlSerializer.Deserialize(fileStream);

          if (tableModelsObj != null)
            _reservationList = tableModelsObj.TodayReservations;
        }
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        throw new Exception(Constants.ErrorLoadingReservations);
      }

      return _reservationList;
    }

    public static SettingsModel DeSerializeSettings(string xmlFilePath)
    {
      SettingsModel settingsObject = null;

      try
      {
        _xmlFilePath = xmlFilePath;

        var xmlSerializer = new XmlSerializer(typeof (SettingsModel));
        using (var fileStream = new FileStream(xmlFilePath, FileMode.Open))
        {
          settingsObject = (SettingsModel) xmlSerializer.Deserialize(fileStream);
        }
      }
      catch (Exception exception)
      {
        NLogger.LogError(exception);
        throw new Exception(Constants.ErrorLoadingSettings);
      }
      return settingsObject;
    }
  }
}