namespace SaluteOnline.LogService.DAL
{
    public interface IGenericRepository<TEntity>
    {
        void Insert(TEntity entity);
    }
}