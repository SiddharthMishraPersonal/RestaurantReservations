using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Reservations.Shared.Log
{
  public static class NLogger
  {
    #region Private Member Variable

    private static readonly Logger Logger;

    #endregion

    #region Constructors

    static NLogger()
    {
      Logger = LogManager.GetLogger("Restaurant.Reservations");
    }

    #endregion

    #region Public Methods

    public static void LogError(Exception exception)
    {
      Trace.WriteLine(exception);
      Logger.Error(exception);
    }

    public static void LogError(string message)
    {
      Trace.WriteLine(message);
      Logger.Error(message);
    }

    public static void LogDebug(string debugContent)
    {
      Trace.WriteLine(debugContent);
      Logger.Debug(debugContent);
    }

    #endregion
  }
}