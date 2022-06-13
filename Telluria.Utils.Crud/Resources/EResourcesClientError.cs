namespace Telluria.Utils.Crud.Resources
{
  // ALWAYS follow this pattern:
  // Error messages -> ERROR_<codeerror>
  // Info messages -> INFO_<identifier>
  // Success messages -> SUCCESS_<identifier>
  public enum EResourcesClientError
  {
    ERROR_BAD_REQUEST, // The request could not be understood by the server due to malformed syntax
    ERROR_UNAUTHORIZED, // The request requires user authentication
    ERROR_PAYMENT_REQUIRED, // This code has not yet been implemented and is reserved for use in an upcoming specification
    ERROR_FORBIDDEN, // The server understood the request, but is refusing to fulfill it
    ERROR_NOT_FOUND, // The server has not found anything matching the request URI
    ERROR_METHOD_NOT_ALLOWED, // The method specified in the Request-Line is not allowed for the resource identified by the Request-URI
    ERROR_NOT_ACCEPTABLE, // The resource identified by the request is only capable of generating response entities which have content characteristics acceptable according to the accept headers sent in the request
    ERROR_PROXY_AUTHENTICATION_REQUIRED, // That the client must first authenticate itself with the proxy
    ERROR_REQUEST_TIMEOUT, // The client did not produce a request within the time that the server was prepared to wait
    ERROR_CONFLICT, // The request could not be completed due to a conflict with the current state of the resource
    ERROR_GONE, // The requested resource is no longer available at the server and no forwarding address is known
    ERROR_LENGTH_REQUIRED, // The server refuses to accept the request without a defined content length
    ERROR_PRECONDITION_FAILED, // The precondition given in one or more of the request header fields evaluated to false when it was tested on the server
    ERROR_PAYLOAD_TOO_LARGE, // The server is refusing to process a request because the request payload is larger than the server is willing or able to process
    ERROR_URI_TOO_LONG, // The server is refusing to service the request because the request URI is longer than the server is willing to interpret
    ERROR_UNSUPPORTED_MEDIA_TYPE, // The server is refusing to service the request because the entity of the request is in a format not supported by the requested resource for the requested method
    ERROR_RANGE_NOT_SATISFIABLE, // None of the range-specifier values in a Range request header field overlap the current extent of the selected resource
    ERROR_EXPECTATION_FAILED, // The expectation given in an Expect request header field could not be met by at least one of the inbound servers
    ERROR_IM_A_TEAPOT, // The server refuses to brew coffee because it is a teapot
    ERROR_MISDIRECTED_REQUEST, // The request was directed at a server that is not able to produce a response
    ERROR_UNPROCESSABLE_ENTITY, // The server understands the content type of the request entity and the syntax of the request entity is correct but was unable to process the contained instructions
    ERROR_LOCKED, // The request has been accepted but processing has not been completed
    ERROR_FAILED_DEPENDENCY, // The request failed due to failure of a previous request
    ERROR_UPGRADE_REQUIRED, // The client must switch to a different protocol such as TLS/1.0
    ERROR_PRECONDITION_REQUIRED, // The origin server requires the request to be conditional
    ERROR_TOO_MANY_REQUESTS, // The user has sent too many requests in a given amount of time
    ERROR_REQUEST_HEADER_FIELDS_TOO_LARGE, // The server is unwilling to process the request because its header fields are too large
    ERROR_CONNECTION_CLOSED_WITHOUT_RESPONSE, // The server closed the connection without returning a response
    ERROR_UNAVAILABLE_FOR_LEGAL_REASONS, // The server is denying access to the resource in response to a legal demand
    ERROR_CLIENT_CLOSED_REQUEST // The client has closed the request
  }
}
