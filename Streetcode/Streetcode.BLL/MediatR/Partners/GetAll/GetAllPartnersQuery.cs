﻿using FluentResults;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Services.Cache;

namespace Streetcode.BLL.MediatR.Partners.GetAll;

public record GetAllPartnersQuery : ICachibleQueryPreProcessor<Result<IEnumerable<PartnerDTO>>>
{
    public object? CachedResponse { get; set; }
}
