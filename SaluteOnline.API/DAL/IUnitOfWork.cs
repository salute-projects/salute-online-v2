using System.Threading.Tasks;

namespace SaluteOnline.API.DAL
{
    public interface IUnitOfWork
    {
        void Save();
        Task<int> SaveAsync();
    }
}