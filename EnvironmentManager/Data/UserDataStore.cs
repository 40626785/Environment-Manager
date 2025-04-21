using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.Data;
public class UserDataStore : DbContext, IUserDataStore
{
    private UserDbContext _context;

    public UserDataStore(UserDbContext context)
    {
        _context = context;
    }

    //Returns single User object based on username, if none exists returns null
    public User GetUser(string username)
    {
        return _context.User.SingleOrDefault(n => n.Username == username);
    }
}
