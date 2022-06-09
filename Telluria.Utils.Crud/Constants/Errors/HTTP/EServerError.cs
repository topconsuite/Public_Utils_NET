using System.ComponentModel.DataAnnotations;
using System.Resources;
using Telluria.Utils.Crud.Constants.Enums;
using Telluria.Utils.Crud.Constants.Enums.HTTP;
using RES = Telluria.Utils.Crud.Resources;

namespace Telluria.Utils.Crud.Constants.Errors.HTTP
{
  public enum EServerError
  {
    [Display(Name = "INTERNAL_SERVER_ERROR")]
    INTERNAL_SERVER_ERROR = 500,

    [Display(Name = "NOT_IMPLEMENTED")]
    NOT_IMPLEMENTED = 501,

    [Display(Name = "BAD_GATEWAY")]
    BAD_GATEWAY = 502,

    [Display(Name = "SERVICE_UNAVAILABLE")]
    SERVICE_UNAVAILABLE = 503,

    [Display(Name = "GATEWAY_TIMEOUT")]
    GATEWAY_TIMEOUT = 504,

    [Display(Name = "HTTP_VERSION_NOT_SUPPORTED")]
    HTTP_VERSION_NOT_SUPPORTED = 505,

    [Display(Name = "VARIANT_ALSO_NEGOTIATES")]
    VARIANT_ALSO_NEGOTIATES = 506,

    [Display(Name = "INSUFFICIENT_STORAGE")]
    INSUFFICIENT_STORAGE = 507,

    [Display(Name = "LOOP_DETECTED")]
    LOOP_DETECTED = 508,

    [Display(Name = "BANDWIDTH_LIMIT_EXCEEDED")]
    BANDWIDTH_LIMIT_EXCEEDED = 509,

    [Display(Name = "NOT_EXTENDED")]
    NOT_EXTENDED = 510,

    [Display(Name = "NETWORK_AUTHENTICATION_REQUIRED")]
    NETWORK_AUTHENTICATION_REQUIRED = 511,

    [Display(Name = "NETWORK_READ_TIMEOUT")]
    NETWORK_READ_TIMEOUT = 598,

    [Display(Name = "NETWORK_CONNECT_TIMEOUT")]
    NETWORK_CONNECT_TIMEOUT = 599
  }

  public static class EServerErrorExtensions
  {
    public static string GetErrorCode(this EServerError error)
    {
      return error switch
      {
        EServerError.INTERNAL_SERVER_ERROR => "500",
        EServerError.NOT_IMPLEMENTED => "501",
        EServerError.BAD_GATEWAY => "502",
        EServerError.SERVICE_UNAVAILABLE => "503",
        EServerError.GATEWAY_TIMEOUT => "504",
        EServerError.HTTP_VERSION_NOT_SUPPORTED => "505",
        EServerError.VARIANT_ALSO_NEGOTIATES => "506",
        EServerError.INSUFFICIENT_STORAGE => "507",
        EServerError.LOOP_DETECTED => "508",
        EServerError.BANDWIDTH_LIMIT_EXCEEDED => "509",
        EServerError.NOT_EXTENDED => "510",
        EServerError.NETWORK_AUTHENTICATION_REQUIRED => "511",
        EServerError.NETWORK_READ_TIMEOUT => "598",
        EServerError.NETWORK_CONNECT_TIMEOUT => "599",
        _ => "Unknown."
      };
    }

    public static string GetErrorMessage(this EServerError error)
    {
      var rm = new ResourceManager(typeof(RES.Resources));

      return error switch
      {
        EServerError.INTERNAL_SERVER_ERROR => rm.GetString(EResourcesServerError.ERROR_INTERNAL_SERVER_ERROR.ToString()) ?? "",
        EServerError.NOT_IMPLEMENTED => rm.GetString(EResourcesServerError.ERROR_NOT_IMPLEMENTED.ToString()) ?? "",
        EServerError.BAD_GATEWAY => rm.GetString(EResourcesServerError.ERROR_BAD_GATEWAY.ToString()) ?? "",
        EServerError.SERVICE_UNAVAILABLE => rm.GetString(EResourcesServerError.ERROR_SERVICE_UNAVAILABLE.ToString()) ?? "",
        EServerError.GATEWAY_TIMEOUT => rm.GetString(EResourcesServerError.ERROR_GATEWAY_TIMEOUT.ToString()) ?? "",
        EServerError.HTTP_VERSION_NOT_SUPPORTED => rm.GetString(EResourcesServerError.ERROR_HTTP_VERSION_NOT_SUPPORTED.ToString()) ?? "",
        EServerError.VARIANT_ALSO_NEGOTIATES => rm.GetString(EResourcesServerError.ERROR_VARIANT_ALSO_NEGOTIATES.ToString()) ?? "",
        EServerError.INSUFFICIENT_STORAGE => rm.GetString(EResourcesServerError.ERROR_INSUFFICIENT_STORAGE.ToString()) ?? "",
        EServerError.LOOP_DETECTED => rm.GetString(EResourcesServerError.ERROR_LOOP_DETECTED.ToString()) ?? "",
        EServerError.BANDWIDTH_LIMIT_EXCEEDED => rm.GetString(EResourcesServerError.ERROR_BANDWIDTH_LIMIT_EXCEEDED.ToString()) ?? "",
        EServerError.NOT_EXTENDED => rm.GetString(EResourcesServerError.ERROR_NOT_EXTENDED.ToString()) ?? "",
        EServerError.NETWORK_AUTHENTICATION_REQUIRED => rm.GetString(EResourcesServerError.ERROR_NETWORK_AUTHENTICATION_REQUIRED.ToString()) ?? "",
        EServerError.NETWORK_READ_TIMEOUT => rm.GetString(EResourcesServerError.ERROR_NETWORK_READ_TIMEOUT.ToString()) ?? "",
        EServerError.NETWORK_CONNECT_TIMEOUT => rm.GetString(EResourcesServerError.ERROR_NETWORK_CONNECT_TIMEOUT.ToString()) ?? "",
        _ => rm.GetString(EResources.ERROR_UNKNOWN.ToString()) ?? ""
      };
    }
  }
}
