﻿using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Validations;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Create;

public record CreateTextCommand(TextCreateDTO TextCreate) : IValidatableRequest<Result<TextDTO>>
{
}