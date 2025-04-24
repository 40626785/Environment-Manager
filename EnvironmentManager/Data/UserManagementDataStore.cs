using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using System.Diagnostics;

namespace EnvironmentManager.Data;

/// <summary>
/// Implementation of the user management data store interface
/// </summary>
public class UserManagementDataStore : IUserManagementDataStore
{
    private readonly UserManagementDbContext _context;
    
    public UserManagementDataStore(UserManagementDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Gets all users in the database for the management view. 
    /// </summary>
    /// <returns>Collection of all users</returns>
    public IEnumerable<User> GetAllUsers()
    {
        var users = _context.Users.ToList();
        Debug.WriteLine($"GetAllUsers: Retrieved {users.Count} users from database");
        
        // Log the roles of all users for debugging
        foreach (var user in users)
        {
            Debug.WriteLine($"GetAllUsers: User {user.Username} has role {user.Role} (enum value: {(int)user.Role})");
        }
        
        return users;
    }
    
    public User GetUser(string username)
    {
        return _context.Users.SingleOrDefault(u => u.Username == username);
    }
    
    /// <summary>
    /// Searches for users with usernames that contain the query string
    /// </summary>
    /// <param name="query">Search term</param>
    /// <returns>Collection of matching users</returns>
    public IEnumerable<User> SearchUsers(string query)
    {
        var results = _context.Users.Where(u => u.Username.Contains(query)).ToList();
        Debug.WriteLine($"SearchUsers: Retrieved {results.Count} users from database matching query '{query}'");
        
        // Log the roles of all users for debugging
        foreach (var user in results)
        {
            Debug.WriteLine($"SearchUsers: User {user.Username} has role {user.Role} (enum value: {(int)user.Role})");
        }
        
        return results;
    }
    
    public IEnumerable<Role> GetAllRoles()
    {
        return _context.Roles.ToList();
    }
    
    public Role GetRole(int roleId)
    {
        return _context.Roles.Find(roleId);
    }
    
    public async Task<User> CreateUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task<User> UpdateUser(User user)
    {
        try
        {
            Debug.WriteLine($"UpdateUser: Starting update for user {user.Username}");
            Debug.WriteLine($"UpdateUser: Role before update is {user.Role} (enum value: {(int)user.Role})");
            
            // Check if user exists in the database before updating
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser == null)
            {
                Debug.WriteLine($"UpdateUser ERROR: User {user.Username} not found in the database");
                throw new Exception($"User {user.Username} not found in the database");
            }
            
            Debug.WriteLine($"UpdateUser: Existing user found with role {existingUser.Role} (enum value: {(int)existingUser.Role})");
            
            // Get the correct role ID for the database (from the DatabaseRoleId property)
            var roleId = user.DatabaseRoleId;
            Debug.WriteLine($"UpdateUser: Using enum role {user.Role} (enum value: {(int)user.Role}) as database role ID {roleId}");
            
            // Apply properties to existing user
            existingUser.Role = user.Role;
            if (!string.IsNullOrEmpty(user.Password))
            {
                existingUser.Password = user.Password;
            }
            
            Debug.WriteLine($"UpdateUser: User updated in memory with role {existingUser.Role} (enum value: {(int)existingUser.Role})");
            
            // Use raw SQL to update the user
            _context.Database.ExecuteSqlRaw(
                "UPDATE Users SET Role = {0}, Password = {1} WHERE Username = {2}",
                roleId, // Use the database role ID from DatabaseRoleId property
                existingUser.Password,
                existingUser.Username
            );
            
            Debug.WriteLine($"UpdateUser: Executed SQL update with role ID {roleId}");
            
            return existingUser;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"UpdateUser ERROR: {ex.GetType().Name} - {ex.Message}");
            Debug.WriteLine($"UpdateUser ERROR DETAILS: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Debug.WriteLine($"UpdateUser INNER ERROR: {ex.InnerException.Message}");
            }
            throw;
        }
    }
    
    public async Task<bool> DeleteUser(User user)
    {
        _context.Users.Remove(user);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    
    public async Task<Role> CreateRole(Role role)
    {
        role.CreatedDate = DateTime.Now;
        role.LastModifiedDate = DateTime.Now;
        
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }
    
    public async Task<Role> UpdateRole(Role role)
    {
        role.LastModifiedDate = DateTime.Now;
        
        _context.Entry(role).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return role;
    }
    
    public async Task<bool> DeleteRole(int roleId)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
            return false;
            
        _context.Roles.Remove(role);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
} 