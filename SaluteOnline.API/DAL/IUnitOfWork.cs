using System.Threading.Tasks;
using SaluteOnline.API.DAL.Entities;

namespace SaluteOnline.API.DAL
{
    public interface IUnitOfWork
    {
        GenericRepository<User> Users { get; }
        void Save();
        Task<int> SaveAsync();
    }
}