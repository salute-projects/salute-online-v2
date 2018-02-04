namespace SaluteOnline.API.Services.Interface
{
    public interface IBusService
    {
        void Publish<TEntity>(TEntity message);
    }
}