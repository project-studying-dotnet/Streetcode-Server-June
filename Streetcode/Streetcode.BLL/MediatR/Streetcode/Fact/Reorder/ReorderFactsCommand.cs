﻿using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.Services.Cache;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;

public record ReorderFactsCommand(IEnumerable<FactUpdatePositionDto> newPositionsOfFacts, int streetcodeId) : IValidatableRequest<Result<IEnumerable<FactDto>>>, ICachibleCommandPostProcessor<Result<IEnumerable<FactDto>>>;