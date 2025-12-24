using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Login
{
    using Application.Interfaces;
    using Application.Mediator;
    using Domain.Entities;

    public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;

        private const int MaxFailedAttempts = 5;                                // TODO: make it configurable
        private readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(30);   // TODO: make it configurable

        public LoginUserHandler(IUnitOfWork uow, IPasswordHasher passwordHasher)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginUserResponse> HandleAsync(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _uow.UserRepository().GetByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("Invalid email or password.");

            if (user.IsLockedOut)
            {
                return new LoginUserResponse
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}",
                    IsLockedOut = true,
                    LockoutEnd = user.LockoutEnd
                };
            }

            var validPassword = _passwordHasher.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt);

            if (!validPassword)
            {
                user.FailedLoginAttempts += 1;

                if (user.FailedLoginAttempts >= MaxFailedAttempts)
                {
                    user.LockoutEnd = DateTime.UtcNow.Add(LockoutDuration);
                }

                await _uow.SaveChangesAsync();
                throw new Exception("Invalid email or password.");
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _uow.SaveChangesAsync();

            return new LoginUserResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                IsLockedOut = false
            };
        }
    }
}
