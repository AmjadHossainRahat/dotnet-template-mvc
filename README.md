src/
│
├── Web/                             # MVC App (Presentation Layer)
│   ├── Controllers/
│   ├── Views/
│   ├── Models/                      # ViewModels only (NOT domain models)
│   ├── Middleware/
│   ├── Filters/
│   ├── Extensions/                  # DI Extensions (Session config, etc.)
│   ├── Mappings/                    # Mapster configuration
│   ├── Validators/                  # FluentValidation validators (ViewModel)
│   ├── Program.cs
│   ├── Startup.cs
│   └── web.config
│
├── Application/                     # UseCases Layer
│   ├── Interfaces/
│   │   ├── Repository/
│   │   ├── Services/
│   │   └── Cache/                   # Redis + InMemory cache abstraction
│   ├── Models/                      # Application DTOs
│   ├── Commands/                    # CQRS Command objects
│   ├── Queries/                     # CQRS Query objects
│   ├── Handlers/                    # CommandHandlers + QueryHandlers
│   ├── Mediator/                    # Custom Mediator (fully written by you)
│   │   ├── IMediator.cs
│   │   ├── Mediator.cs
│   │   └── Registration.cs          # Extension for DI
│   ├── Validators/                  # FluentValidation validators (domain-level)
│   └── Exceptions/
│
├── Domain/                          # Domain Models + Interfaces
│   ├── Entities/                    # EFCore entities: User, Role, OTP, etc.
│   ├── Enums/
│   ├── ValueObjects/
│   ├── Interfaces/
│   │   ├── IUserRepository.cs
│   │   ├── IRoleRepository.cs
│   │   └── IUnitOfWork.cs
│   └── Events/                      # if any domain notifications later
│
├── Infrastructure/                  # Data + Redis + Repos Layer
│   ├── EF/
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/          # EF entity configurations
│   │   ├── Repository/
│   │   │   ├── GenericRepository.cs
│   │   │   ├── UserRepository.cs
│   │   │   └── RoleRepository.cs
│   │   └── UnitOfWork.cs
│   ├── Cache/
│   │   ├── ICacheService.cs
│   │   ├── RedisCacheService.cs
│   │   └── InMemoryCacheService.cs
│   └── Services/
│
├── Tests/
│   ├── UnitTests/
│   └── IntegrationTests/
│
└── Build/
    ├── Dockerfile
    └── docker-compose.yml
