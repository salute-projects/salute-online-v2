using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.Domain.Mongo;

namespace SaluteOnline.API.DAL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly SaluteOnlineDbContext _context;
        private readonly IConfiguration _configuration;

        public UnitOfWork(SaluteOnlineDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private GenericRepository<User> _users;
        public GenericRepository<User> Users
            => _users ?? (_users = new GenericRepository<User>(_context, _configuration));

        private GenericRepository<Club> _clubs;
        public GenericRepository<Club> Clubs
            => _clubs ?? (_clubs = new GenericRepository<Club>(_context, _configuration));

        private GenericRepository<Activity> _activities;
        public GenericRepository<Activity> Activities
            => _activities ?? (_activities = new GenericRepository<Activity>(_context, _configuration));

        private GenericRepository<Country> _countries;
        public GenericRepository<Country> Countries
            => _countries ?? (_countries = new GenericRepository<Country>(_context, _configuration));

        private GenericRepository<Player> _players;
        public GenericRepository<Player> Players
            => _players ?? (_players = new GenericRepository<Player>(_context, _configuration));

        private GenericRepository<InnerMessage> _innerMessages;
        public GenericRepository<InnerMessage> InnerMessages
            => _innerMessages ?? (_innerMessages = new GenericRepository<InnerMessage>(_context, _configuration));

        private GenericRepository<ConnectedUser> _connectedUsers;
        public GenericRepository<ConnectedUser> ConnectedUsers
            => _connectedUsers ?? (_connectedUsers = new GenericRepository<ConnectedUser>(_context, _configuration));

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
