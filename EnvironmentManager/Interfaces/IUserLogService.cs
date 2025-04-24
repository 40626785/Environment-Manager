using System.Threading.Tasks;
using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces
{
    public interface IUserLogService
    {
        /// <summary>
        /// Logs a user creation event
        /// </summary>
        Task LogUserCreatedAsync(User user);

        /// <summary>
        /// Logs a user update event
        /// </summary>
        Task LogUserUpdatedAsync(User oldUser, User newUser);

        /// <summary>
        /// Logs a user deletion event
        /// </summary>
        Task LogUserDeletedAsync(User user);
    }
} 