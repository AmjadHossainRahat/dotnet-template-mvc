using Application.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Login
{
    public record LoginUserCommand(string Email, string Password, bool RememberMe) : IRequest<LoginUserResponse>;
}
