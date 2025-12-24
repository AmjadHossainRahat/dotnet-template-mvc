using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        // Optional: cache repository instances to avoid multiple creations
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserRepository _userRepository;

        public UnitOfWork(AppDbContext dbContext, IServiceProvider serviceProvider, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            _userRepository = userRepository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(_dbContext);
        }

        public IUserRepository UserRepository()
        {
            return _userRepository;
        }
    }
}
