using System;
using Application.Mediator;

namespace Application.Features.Auth.Register
{
    // Returns newly created User Id (Guid)
    public class RegisterUserCommand : IRequest<Guid>
    {
        public string UserName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string? PhoneNumber { get; init; }
        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;
    }
}
