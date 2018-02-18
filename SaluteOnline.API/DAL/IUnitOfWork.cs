using System.Threading.Tasks;
using SaluteOnline.API.Domain;

namespace SaluteOnline.API.DAL
{
    public interface IUnitOfWork
    {
        GenericRepository<User> Users { get; }
        GenericRepository<Club> Clubs { get; }
        GenericRepository<Player> Players { get;  }
        void Save();
        Task<int> SaveAsync();
    }
}