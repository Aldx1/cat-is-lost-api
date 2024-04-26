using cat_is_lost_api.Models;

namespace cat_is_lost_api.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Find user with id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<User> GetUser(int userId);

        /// <summary>
        /// Add user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        /// Tuple
        ///     Item1 - Success
        ///     Item2 - Status Message
        /// </returns>
        Task<(bool, string)> AddUser(User user);

        /// <summary>
        /// Authenticate login
        /// </summary>
        /// <param name="login"></param>
        /// <returns>token</returns>
        Task<string?> AuthenticateUser(Login login);

        /// <summary>
        /// Get the user id
        /// </summary>
        /// <returns>user id</returns>
        int? GetUserId();
    }
}
