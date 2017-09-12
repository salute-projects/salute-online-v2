using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
