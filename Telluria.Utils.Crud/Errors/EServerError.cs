using System;
using System.ComponentModel.DataAnnotations;
using System.Resources;
using Telluria.Utils.Crud.Resources;
using RES = Telluria.Utils.Crud.Resources;

namespace Telluria.Utils.Crud.Errors
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

      return rm.GetString(error.GetRerourceError().ToString()) ?? "";
    }

    public static Enum GetRerourceError(this EServerError error)
    {
      return error switch
      {
        EServerError.INTERNAL_SERVER_ERROR => EResourcesServerError.ERROR_INTERNAL_SERVER_ERROR,
        EServerError.NOT_IMPLEMENTED => EResourcesServerError.ERROR_NOT_IMPLEMENTED,
        EServerError.BAD_GATEWAY => EResourcesServerError.ERROR_BAD_GATEWAY,
        EServerError.SERVICE_UNAVAILABLE => EResourcesServerError.ERROR_SERVICE_UNAVAILABLE,
        EServerError.GATEWAY_TIMEOUT => EResourcesServerError.ERROR_GATEWAY_TIMEOUT,
        EServerError.HTTP_VERSION_NOT_SUPPORTED => EResourcesServerError.ERROR_HTTP_VERSION_NOT_SUPPORTED,
        EServerError.VARIANT_ALSO_NEGOTIATES => EResourcesServerError.ERROR_VARIANT_ALSO_NEGOTIATES,
        EServerError.INSUFFICIENT_STORAGE => EResourcesServerError.ERROR_INSUFFICIENT_STORAGE,
        EServerError.LOOP_DETECTED => EResourcesServerError.ERROR_LOOP_DETECTED,
        EServerError.BANDWIDTH_LIMIT_EXCEEDED => EResourcesServerError.ERROR_BANDWIDTH_LIMIT_EXCEEDED,
        EServerError.NOT_EXTENDED => EResourcesServerError.ERROR_NOT_EXTENDED,
        EServerError.NETWORK_AUTHENTICATION_REQUIRED => EResourcesServerError.ERROR_NETWORK_AUTHENTICATION_REQUIRED,
        EServerError.NETWORK_READ_TIMEOUT => EResourcesServerError.ERROR_NETWORK_READ_TIMEOUT,
        EServerError.NETWORK_CONNECT_TIMEOUT => EResourcesServerError.ERROR_NETWORK_CONNECT_TIMEOUT,
        _ => EResources.ERROR_UNKNOWN
      };
    }
  }
}
