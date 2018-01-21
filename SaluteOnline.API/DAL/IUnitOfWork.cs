using System.Threading.Tasks;
using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.Domain.Mongo;
using SaluteOnline.Domain.Domain.Mongo.Chat;

namespace SaluteOnline.API.DAL
{
    public interface IUnitOfWork
    {
        GenericRepository<User> Users { get; }
        GenericRepository<Club> Clubs { get; }
        GenericRepository<Activity> Activities { get; }
        GenericRepository<Country> Countries { get; }
        GenericRepository<Player> Players { get;  }
        GenericRepository<ConnectedUser> ConnectedUsers { get; }
        GenericRepository<Chat> Chats { get; }
        void Save();
        Task<int> SaveAsync();
    }
}