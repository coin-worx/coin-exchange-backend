namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// User reading repository
    /// </summary>
    public interface IUserRepository
    {
        User GetUserByUserName(string username);
        User GetUserByActivationKey(string activationKey);
        User GetUserByEmail(string email);
        User GetUserByEmailAndUserName(string username,string email);
        User GetUserByForgotPasswordCode(string forgotPasswordCode);
        bool DeleteUser(User user);
    }
}
