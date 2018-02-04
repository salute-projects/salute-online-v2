namespace SaluteOnline.LogService.Handlers.Abstraction
{
    public interface IHandler<in TEntity>
    {
        void HandleAndInsert<T>(TEntity message);
    }
}