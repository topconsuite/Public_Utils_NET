namespace Telluria.Utils.Crud.Resources
{
  // ALWAYS follow this pattern:
  // Error messages -> ERROR_<codeerror>
  // Info messages -> INFO_<identifier>
  // Success messages -> SUCCESS_<identifier>
  public enum EResourcesServerError
  {
    ERROR_INTERNAL_SERVER_ERROR, // The server encountered an unexpected condition that prevented it from fulfilling the request
    ERROR_NOT_IMPLEMENTED, // The server does not support the functionality required to fulfill the request
    ERROR_BAD_GATEWAY, // The server, while acting as a gateway or proxy, received an invalid response from the upstream server it accessed in attempting to fulfill the request
    ERROR_SERVICE_UNAVAILABLE, // The server is currently unable to handle the request due to a temporary overloading or maintenance of the server
    ERROR_GATEWAY_TIMEOUT, // The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server specified by the URI (e.g. HTTP, FTP, LDAP) or some other auxiliary server (e.g. DNS) it needed to access in attempting to complete the request
    ERROR_HTTP_VERSION_NOT_SUPPORTED, // The server does not support, or refuses to support, the HTTP protocol version that was used in the request message
    ERROR_VARIANT_ALSO_NEGOTIATES, // The server has an internal configuration error: the chosen variant resource is configured to engage in transparent content negotiation itself, and is therefore not a proper end point in the negotiation process
    ERROR_INSUFFICIENT_STORAGE, // The server is unable to store the representation needed to complete the request
    ERROR_LOOP_DETECTED, // The server detected an infinite loop while processing the request
    ERROR_BANDWIDTH_LIMIT_EXCEEDED, // The server has exceeded the bandwidth specified by the server administrator; this is often used by shared hosting providers to limit the bandwidth of customers
    ERROR_NOT_EXTENDED, // The server does not support, or refuses to support, the protocol version that was used in the request message
    ERROR_NETWORK_AUTHENTICATION_REQUIRED, // The client needs to authenticate to gain network access
    ERROR_NETWORK_READ_TIMEOUT, // The server timed out waiting for the network connection
    ERROR_NETWORK_CONNECT_TIMEOUT // The server timed out while waiting for a proxy connection
  }
}
