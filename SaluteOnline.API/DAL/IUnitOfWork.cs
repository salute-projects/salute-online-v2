using System.Threading.Tasks;
using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.Domain.Mongo;

namespace SaluteOnline.API.DAL
{
    public interface IUnitOfWork
    {
        GenericRepository<User> Users { get; }
        GenericRepository<Club> Clubs { get; }
        GenericRepository<Activity> Activities { get; }
        GenericRepository<Country> Countries { get; }
        GenericRepository<Player> Players { get;  }
        GenericRepository<InnerMessage> InnerMessages { get; }
        GenericRepository<ConnectedUser> ConnectedUsers { get; }
        void Save();
        Task<int> SaveAsync();
    }
}