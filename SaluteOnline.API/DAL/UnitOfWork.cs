using System;
using System.Threading.Tasks;
using SaluteOnline.API.Domain;

namespace SaluteOnline.API.DAL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly SaluteOnlineDbContext _context;

        public UnitOfWork(SaluteOnlineDbContext context)
        {
            _context = context;
        }

        private GenericRepository<User> _users;
        public GenericRepository<User> Users
            => _users ?? (_users = new GenericRepository<User>(_context));

        private GenericRepository<Club> _clubs;
        public GenericRepository<Club> Clubs
            => _clubs ?? (_clubs = new GenericRepository<Club>(_context));


        private GenericRepository<Player> _players;
        public GenericRepository<Player> Players
            => _players ?? (_players = new GenericRepository<Player>(_context));

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
