using System;
using System.ComponentModel.DataAnnotations;
using System.Resources;
using Telluria.Utils.Crud.Resources;
using RES = Telluria.Utils.Crud.Resources;

namespace Telluria.Utils.Crud.Errors
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

      return rm.GetString(error.GetRerourceError().ToString()) ?? "";
    }

    public static Enum GetRerourceError(this EClientError error)
    {
      return error switch
      {
        EClientError.BAD_REQUEST => EResourcesClientError.ERROR_BAD_REQUEST,
        EClientError.UNAUTHORIZED => EResourcesClientError.ERROR_UNAUTHORIZED,
        EClientError.PAYMENT_REQUIRED => EResourcesClientError.ERROR_PAYMENT_REQUIRED,
        EClientError.FORBIDDEN => EResourcesClientError.ERROR_FORBIDDEN,
        EClientError.NOT_FOUND => EResourcesClientError.ERROR_NOT_FOUND,
        EClientError.METHOD_NOT_ALLOWED => EResourcesClientError.ERROR_METHOD_NOT_ALLOWED,
        EClientError.NOT_ACCEPTABLE => EResourcesClientError.ERROR_NOT_ACCEPTABLE,
        EClientError.PROXY_AUTHENTICATION_REQUIRED => EResourcesClientError.ERROR_PROXY_AUTHENTICATION_REQUIRED,
        EClientError.REQUEST_TIMEOUT => EResourcesClientError.ERROR_REQUEST_TIMEOUT,
        EClientError.CONFLICT => EResourcesClientError.ERROR_CONFLICT,
        EClientError.GONE => EResourcesClientError.ERROR_GONE,
        EClientError.LENGTH_REQUIRED => EResourcesClientError.ERROR_LENGTH_REQUIRED,
        EClientError.PRECONDITION_FAILED => EResourcesClientError.ERROR_PRECONDITION_FAILED,
        EClientError.PAYLOAD_TOO_LARGE => EResourcesClientError.ERROR_PAYLOAD_TOO_LARGE,
        EClientError.URI_TOO_LONG => EResourcesClientError.ERROR_URI_TOO_LONG,
        EClientError.UNSUPPORTED_MEDIA_TYPE => EResourcesClientError.ERROR_UNSUPPORTED_MEDIA_TYPE,
        EClientError.RANGE_NOT_SATISFIABLE => EResourcesClientError.ERROR_RANGE_NOT_SATISFIABLE,
        EClientError.EXPECTATION_FAILED => EResourcesClientError.ERROR_EXPECTATION_FAILED,
        EClientError.IM_A_TEAPOT => EResourcesClientError.ERROR_IM_A_TEAPOT,
        EClientError.MISDIRECTED_REQUEST => EResourcesClientError.ERROR_MISDIRECTED_REQUEST,
        EClientError.UNPROCESSABLE_ENTITY => EResourcesClientError.ERROR_UNPROCESSABLE_ENTITY,
        EClientError.LOCKED => EResourcesClientError.ERROR_LOCKED,
        EClientError.FAILED_DEPENDENCY => EResourcesClientError.ERROR_FAILED_DEPENDENCY,
        EClientError.UPGRADE_REQUIRED => EResourcesClientError.ERROR_UPGRADE_REQUIRED,
        EClientError.PRECONDITION_REQUIRED => EResourcesClientError.ERROR_PRECONDITION_REQUIRED,
        EClientError.TOO_MANY_REQUESTS => EResourcesClientError.ERROR_TOO_MANY_REQUESTS,
        EClientError.REQUEST_HEADER_FIELDS_TOO_LARGE => EResourcesClientError.ERROR_REQUEST_HEADER_FIELDS_TOO_LARGE,
        EClientError.CONNECTION_CLOSED_WITHOUT_RESPONSE => EResourcesClientError.ERROR_CONNECTION_CLOSED_WITHOUT_RESPONSE,
        EClientError.UNAVAILABLE_FOR_LEGAL_REASONS => EResourcesClientError.ERROR_UNAVAILABLE_FOR_LEGAL_REASONS,
        EClientError.CLIENT_CLOSED_REQUEST => EResourcesClientError.ERROR_CLIENT_CLOSED_REQUEST,
        _ => EResources.ERROR_UNKNOWN
      };
    }
  }
}
