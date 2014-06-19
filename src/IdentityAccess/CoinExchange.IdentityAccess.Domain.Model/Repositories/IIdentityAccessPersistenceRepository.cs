namespace CoinExchange.IdentityAccess.Domain.Model.Repositories
{
    /// <summary>
    /// For persisting Identity Access Domain models to Db
    /// </summary>
    public interface IIdentityAccessPersistenceRepository
    {
        void SaveUpdate(object entity);
    }
}
