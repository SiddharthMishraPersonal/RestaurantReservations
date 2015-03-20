using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Reservations.Shared.Helper
{
  public class Constants
  {
    public const string MutexName = "RestaurantMutex";

    public const string ErrorLoadingTable = "Error occurred while loading Table. See the logs for more information.";

    public const string ErrorLoadingReservations =
      "Error occurred while loading Reservations. See the logs for more information.";

    public const string ErrorLoadingSettings =
      "Error occurred while loading Settings. See the logs for more information.";

    public const string ErrorSavingSettings = "Error occurred while saving Settings. See the logs for more information.";

    public const string ErrorSavingTable = "Error occurred while saving Tables. See the logs for more information.";

    public const string ErrorSavingReservations =
      "Error occurred while saving Reservations. See the logs for more information.";

    public const string FileNotFoundError = "File not found!";

    public const string DirectoryNotFoundError = "Directory not found!";

    public const string ValueRequiredError = "Value required!";
  }
}