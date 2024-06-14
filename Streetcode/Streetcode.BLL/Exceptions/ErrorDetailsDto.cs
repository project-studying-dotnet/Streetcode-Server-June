using System.Net;

namespace Streetcode.BLL.Exceptions;

public record ErrorDetailsDto(string Message, HttpStatusCode StatusCode);