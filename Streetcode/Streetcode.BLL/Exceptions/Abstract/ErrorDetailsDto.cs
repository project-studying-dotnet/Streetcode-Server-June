using Streetcode.BLL.Enums;

namespace Streetcode.BLL.Exceptions.Abstract;

public record ErrorDetailsDto(string Message, ErrorType ErrorType);