namespace SaluteOnline.LogService.DAL
{
    public interface IGenericRepository<in TEntity>
    {
        void Insert(TEntity entity);
    }
}