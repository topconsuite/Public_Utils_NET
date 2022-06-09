using System.ComponentModel.DataAnnotations;
using System.Resources;
using Telluria.Utils.Crud.Constants.Enums;
using Telluria.Utils.Crud.Constants.Enums.HTTP;
using RES = Telluria.Utils.Crud.Resources;

namespace Telluria.Utils.Crud.Constants.Errors.HTTP
{
  public enum EClientError
  {
    [Display(Name = "BAD_REQUEST")]
    BAD_REQUEST = 400,

    [Display(Name = "UNAUTHORIZED")]
    UNAUTHORIZED = 401,

    [Display(Name = "PAYMENT_REQUIRED")]
    PAYMENT_REQUIRED = 402,

    [Display(Name = "FORBIDDEN")]
    FORBIDDEN = 403,

    [Display(Name = "NOT_FOUND")]
    NOT_FOUND = 404,

    [Display(Name = "METHOD_NOT_ALLOWED")]
    METHOD_NOT_ALLOWED = 405,

    [Display(Name = "NOT_ACCEPTABLE")]
    NOT_ACCEPTABLE = 406,

    [Display(Name = "PROXY_AUTHENTICATION_REQUIRED")]
    PROXY_AUTHENTICATION_REQUIRED = 407,

    [Display(Name = "REQUEST_TIMEOUT")]
    REQUEST_TIMEOUT = 408,

    [Display(Name = "CONFLICT")]
    CONFLICT = 409,

    [Display(Name = "GONE")]
    GONE = 410,

    [Display(Name = "LENGTH_REQUIRED")]
    LENGTH_REQUIRED = 411,

    [Display(Name = "PRECONDITION_FAILED")]
    PRECONDITION_FAILED = 412,

    [Display(Name = "PAYLOAD_TOO_LARGE")]
    PAYLOAD_TOO_LARGE = 413,

    [Display(Name = "URI_TOO_LONG")]
    URI_TOO_LONG = 414,

    [Display(Name = "UNSUPPORTED_MEDIA_TYPE")]
    UNSUPPORTED_MEDIA_TYPE = 415,

    [Display(Name = "RANGE_NOT_SATISFIABLE")]
    RANGE_NOT_SATISFIABLE = 416,

    [Display(Name = "EXPECTATION_FAILED")]
    EXPECTATION_FAILED = 417,

    [Display(Name = "I_AM_A_TEAPOT")]
    IM_A_TEAPOT = 418,

    [Display(Name = "MISDIRECTED_REQUEST")]
    MISDIRECTED_REQUEST = 421,

    [Display(Name = "UNPROCESSABLE_ENTITY")]
    UNPROCESSABLE_ENTITY = 422,

    [Display(Name = "LOCKED")]
    LOCKED = 423,

    [Display(Name = "FAILED_DEPENDENCY")]
    FAILED_DEPENDENCY = 424,

    [Display(Name = "TOO_EARLY")]
    TOO_EARLY = 425,

    [Display(Name = "UPGRADE_REQUIRED")]
    UPGRADE_REQUIRED = 426,

    [Display(Name = "PRECONDITION_REQUIRED")]
    PRECONDITION_REQUIRED = 428,

    [Display(Name = "TOO_MANY_REQUESTS")]
    TOO_MANY_REQUESTS = 429,

    [Display(Name = "REQUEST_HEADER_FIELDS_TOO_LARGE")]
    REQUEST_HEADER_FIELDS_TOO_LARGE = 431,

    [Display(Name = "CONNECTION_CLOSED_WITHOUT_RESPONSE")]
    CONNECTION_CLOSED_WITHOUT_RESPONSE = 444,

    [Display(Name = "UNAVAILABLE_FOR_LEGAL_REASONS")]
    UNAVAILABLE_FOR_LEGAL_REASONS = 451,

    [Display(Name = "CLIENT_CLOSED_REQUEST")]
    CLIENT_CLOSED_REQUEST = 499
  }

  public static class EClientErrorExtensions
  {
    public static string GetErrorMessage(this EClientError error)
    {
      var rm = new ResourceManager(typeof(RES.Resources));

      return error switch
      {
        EClientError.BAD_REQUEST => rm.GetString(EResourcesClientError.ERROR_BAD_REQUEST.ToString()) ?? "",
        EClientError.UNAUTHORIZED => rm.GetString(EResourcesClientError.ERROR_UNAUTHORIZED.ToString()) ?? "",
        EClientError.PAYMENT_REQUIRED => rm.GetString(EResourcesClientError.ERROR_PAYMENT_REQUIRED.ToString()) ?? "",
        EClientError.FORBIDDEN => rm.GetString(EResourcesClientError.ERROR_FORBIDDEN.ToString()) ?? "",
        EClientError.NOT_FOUND => rm.GetString(EResourcesClientError.ERROR_NOT_FOUND.ToString()) ?? "",
        EClientError.METHOD_NOT_ALLOWED => rm.GetString(EResourcesClientError.ERROR_METHOD_NOT_ALLOWED.ToString()) ?? "",
        EClientError.NOT_ACCEPTABLE => rm.GetString(EResourcesClientError.ERROR_NOT_ACCEPTABLE.ToString()) ?? "",
        EClientError.PROXY_AUTHENTICATION_REQUIRED => rm.GetString(EResourcesClientError.ERROR_PROXY_AUTHENTICATION_REQUIRED.ToString()) ?? "",
        EClientError.REQUEST_TIMEOUT => rm.GetString(EResourcesClientError.ERROR_REQUEST_TIMEOUT.ToString()) ?? "",
        EClientError.CONFLICT => rm.GetString(EResourcesClientError.ERROR_CONFLICT.ToString()) ?? "",
        EClientError.GONE => rm.GetString(EResourcesClientError.ERROR_GONE.ToString()) ?? "",
        EClientError.LENGTH_REQUIRED => rm.GetString(EResourcesClientError.ERROR_LENGTH_REQUIRED.ToString()) ?? "",
        EClientError.PRECONDITION_FAILED => rm.GetString(EResourcesClientError.ERROR_PRECONDITION_FAILED.ToString()) ?? "",
        EClientError.PAYLOAD_TOO_LARGE => rm.GetString(EResourcesClientError.ERROR_PAYLOAD_TOO_LARGE.ToString()) ?? "",
        EClientError.URI_TOO_LONG => rm.GetString(EResourcesClientError.ERROR_URI_TOO_LONG.ToString()) ?? "",
        EClientError.UNSUPPORTED_MEDIA_TYPE => rm.GetString(EResourcesClientError.ERROR_UNSUPPORTED_MEDIA_TYPE.ToString()) ?? "",
        EClientError.RANGE_NOT_SATISFIABLE => rm.GetString(EResourcesClientError.ERROR_RANGE_NOT_SATISFIABLE.ToString()) ?? "",
        EClientError.EXPECTATION_FAILED => rm.GetString(EResourcesClientError.ERROR_EXPECTATION_FAILED.ToString()) ?? "",
        EClientError.IM_A_TEAPOT => rm.GetString(EResourcesClientError.ERROR_IM_A_TEAPOT.ToString()) ?? "",
        EClientError.MISDIRECTED_REQUEST => rm.GetString(EResourcesClientError.ERROR_MISDIRECTED_REQUEST.ToString()) ?? "",
        EClientError.UNPROCESSABLE_ENTITY => rm.GetString(EResourcesClientError.ERROR_UNPROCESSABLE_ENTITY.ToString()) ?? "",
        EClientError.LOCKED => rm.GetString(EResourcesClientError.ERROR_LOCKED.ToString()) ?? "",
        EClientError.FAILED_DEPENDENCY => rm.GetString(EResourcesClientError.ERROR_FAILED_DEPENDENCY.ToString()) ?? "",
        EClientError.UPGRADE_REQUIRED => rm.GetString(EResourcesClientError.ERROR_UPGRADE_REQUIRED.ToString()) ?? "",
        EClientError.PRECONDITION_REQUIRED => rm.GetString(EResourcesClientError.ERROR_PRECONDITION_REQUIRED.ToString()) ?? "",
        EClientError.TOO_MANY_REQUESTS => rm.GetString(EResourcesClientError.ERROR_TOO_MANY_REQUESTS.ToString()) ?? "",
        EClientError.REQUEST_HEADER_FIELDS_TOO_LARGE => rm.GetString(EResourcesClientError.ERROR_REQUEST_HEADER_FIELDS_TOO_LARGE.ToString()) ?? "",
        EClientError.CONNECTION_CLOSED_WITHOUT_RESPONSE => rm.GetString(EResourcesClientError.ERROR_CONNECTION_CLOSED_WITHOUT_RESPONSE.ToString()) ?? "",
        EClientError.UNAVAILABLE_FOR_LEGAL_REASONS => rm.GetString(EResourcesClientError.ERROR_UNAVAILABLE_FOR_LEGAL_REASONS.ToString()) ?? "",
        EClientError.CLIENT_CLOSED_REQUEST => rm.GetString(EResourcesClientError.ERROR_CLIENT_CLOSED_REQUEST.ToString()) ?? "",
        _ => rm.GetString(EResources.ERROR_UNKNOWN.ToString()) ?? ""
      };
    }
  }
}
