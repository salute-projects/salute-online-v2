namespace SaluteOnline.IdentityServer.Service.Declaration
{
    public interface IBusService
    {
        void Publish<TEntity>(TEntity message);
    }
}