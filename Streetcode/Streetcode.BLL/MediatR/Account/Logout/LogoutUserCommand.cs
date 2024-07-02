using FluentResults;
using MediatR;

namespace Streetcode.BLL.MediatR.Account.Logout;

public record LogoutUserCommand : IRequest<Result<string>>;