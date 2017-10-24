using System.Threading.Tasks;
using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.Domain.Mongo;

namespace SaluteOnline.API.DAL
{
    public interface IUnitOfWork
    {
        GenericRepository<User> Users { get; }
        GenericRepository<Activity> Activities { get; }
        void Save();
        Task<int> SaveAsync();
    }
}