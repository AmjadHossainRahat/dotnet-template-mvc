using Application.Mediator;
using Domain.Entities;
using Application.Interfaces;

namespace Application.Features.Auth.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _hasher;

        public RegisterUserHandler(IUnitOfWork uow, IPasswordHasher hasher)
        {
            _uow = uow;
            _hasher = hasher;
        }

        public async Task<Guid> HandleAsync(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Check uniqueness (email)
            var userRepo = _uow.Repository<User>();
            var existing = await userRepo.FindAsync(u => u.Email.ToLower() == request.Email.Trim().ToLower());
            if (existing is { } && Enumerable.Any(existing))
                throw new InvalidOperationException("Email already registered.");

            // Create user
            _hasher.CreatePasswordHash(request.Password, out var pwdHash, out var pwdSalt);

            var user = new User
            {
                UserName = request.UserName.Trim(),
                Email = request.Email.Trim(),
                PhoneNumber = request.PhoneNumber?.Trim(),
                PasswordHash = pwdHash,
                PasswordSalt = pwdSalt,
                IsEmailConfirmed = false,
                IsPhoneConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            await userRepo.AddAsync(user);
            await _uow.SaveChangesAsync();

            return user.Id;
        }
    }
}
