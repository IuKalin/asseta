using Asseta.Application.Features.Auth.DTOs;
using MediatR;

namespace Asseta.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<UserDto>;
