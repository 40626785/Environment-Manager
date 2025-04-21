using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.Data;
/// <summary>
/// Abstracts the DbContext implementation, providing ViewModels access to any required read/write access without depending on a concrete data management implementation.
/// 
/// Implements IUserDataStore which is passed into constructors via Dependency Injection.
/// </summary>
public class UserDataStore : DbContext, IUserDataStore
{
    private UserDbContext _context;

    public UserDataStore(UserDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a single user object based on username
    /// </summary>
    /// <param name="username">The user to retrieve</param>
    /// <returns>Matching user or null</returns>
    public User GetUser(string username)
    {
        return _context.User.SingleOrDefault(n => n.Username == username);
    }
}
