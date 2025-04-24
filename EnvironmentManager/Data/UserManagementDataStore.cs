using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using EnvironmentManager.Services;
using System.Diagnostics;

namespace EnvironmentManager.Data;

/// <summary>
/// Implementation of the user management data store interface
/// </summary>
public class UserManagementDataStore : IUserManagementDataStore
{
    private readonly UserManagementDbContext _context;
    private readonly IUserLogService _logService;
    
    public UserManagementDataStore(UserManagementDbContext context, IUserLogService logService)
    {
        _context = context;
        _logService = logService;
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
        
        // Log the user creation
        await _logService.LogUserCreatedAsync(user);
        
        return user;
    }
    
    public async Task<User> UpdateUser(User user)
    {
        try
        {
            Debug.WriteLine($"UpdateUser: STARTING UPDATE FOR USER {user.Username}");
            Debug.WriteLine($"UpdateUser: New user role is {user.Role} (enum value: {(int)user.Role})");
            
            // Check if user exists in the database before updating
            var existingUser = _context.Users.AsNoTracking().FirstOrDefault(u => u.Username == user.Username);
            if (existingUser == null)
            {
                Debug.WriteLine($"UpdateUser ERROR: User {user.Username} not found in the database");
                throw new Exception($"User {user.Username} not found in the database");
            }
            
            Debug.WriteLine($"UpdateUser: Found existing user with role {existingUser.Role} (enum value: {(int)existingUser.Role})");
            
            // Create a complete detached copy of the existing user before making any changes
            var oldUser = new User
            {
                Username = existingUser.Username,
                Password = existingUser.Password, 
                Role = existingUser.Role  // This will set the internal _role field
            };
            
            Debug.WriteLine($"UpdateUser: Created deep copy of old user with role {oldUser.Role} (enum value: {(int)oldUser.Role})");
            
            // Get the correct role ID for the database
            var roleId = user.DatabaseRoleId;
            Debug.WriteLine($"UpdateUser: Using enum role {user.Role} (enum value: {(int)user.Role}) as database role ID {roleId}");
            
            // Apply properties to existing entity (tracked by EF)
            var entityToUpdate = _context.Users.Find(existingUser.Username);
            if (entityToUpdate == null)
            {
                Debug.WriteLine($"UpdateUser ERROR: Could not find user {user.Username} for tracked update");
                throw new Exception($"Could not find user {user.Username} for tracked update");
            }
            
            Debug.WriteLine($"UpdateUser: Found tracked entity with role {entityToUpdate.Role} (enum value: {(int)entityToUpdate.Role})");
            
            // Store original values for logging
            Debug.WriteLine($"UpdateUser: Before changes - role: {entityToUpdate.Role} (enum value: {(int)entityToUpdate.Role})");
            
            // Apply changes to tracked entity
            entityToUpdate.Role = user.Role;
            if (!string.IsNullOrEmpty(user.Password))
            {
                entityToUpdate.Password = user.Password;
            }
            
            Debug.WriteLine($"UpdateUser: After changes - role: {entityToUpdate.Role} (enum value: {(int)entityToUpdate.Role})");
            
            // Save changes through EF Core 
            await _context.SaveChangesAsync();
            Debug.WriteLine($"UpdateUser: Changes saved to database");
            
            // Log the user update 
            Debug.WriteLine($"UpdateUser: About to call log service...");
            Debug.WriteLine($"UpdateUser: Old user: {oldUser.Username}, Role: {oldUser.Role} ({(int)oldUser.Role})");
            Debug.WriteLine($"UpdateUser: Updated user: {entityToUpdate.Username}, Role: {entityToUpdate.Role} ({(int)entityToUpdate.Role})");
            
            await _logService.LogUserUpdatedAsync(oldUser, entityToUpdate);
            Debug.WriteLine($"UpdateUser: Logging completed");
            
            return entityToUpdate;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"UpdateUser ERROR: {ex.GetType().Name} - {ex.Message}");
            Debug.WriteLine($"UpdateUser ERROR DETAILS: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Debug.WriteLine($"UpdateUser INNER ERROR: {ex.InnerException.Message}");
                Debug.WriteLine($"UpdateUser INNER STACK: {ex.InnerException.StackTrace}");
            }
            throw;
        }
    }
    
    public async Task<bool> DeleteUser(User user)
    {
        // Log the user deletion before removing
        await _logService.LogUserDeletedAsync(user);
        
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