using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces;

/// <summary>
/// Interface for user management operations
/// </summary>
public interface IUserManagementDataStore
{
    /// <summary>
    /// Gets all users in the system
    /// </summary>
    IEnumerable<User> GetAllUsers();
    
    /// <summary>
    /// Gets a specific user by username
    /// </summary>
    User GetUser(string username);
    
    /// <summary>
    /// Searches for users based on search criteria
    /// </summary>
    IEnumerable<User> SearchUsers(string searchQuery);
    
    /// <summary>
    /// Gets all roles in the system
    /// </summary>
    IEnumerable<Role> GetAllRoles();
    
    /// <summary>
    /// Gets a specific role by ID
    /// </summary>
    Role GetRole(int roleId);
    
    /// <summary>
    /// Creates a new user
    /// </summary>
    Task<User> CreateUser(User user);
    
    /// <summary>
    /// Updates an existing user
    /// </summary>
    Task<User> UpdateUser(User user);
    
    /// <summary>
    /// Deletes a user
    /// </summary>
    Task<bool> DeleteUser(User user);
    
    /// <summary>
    /// Creates a new role
    /// </summary>
    Task<Role> CreateRole(Role role);
    
    /// <summary>
    /// Updates an existing role
    /// </summary>
    Task<Role> UpdateRole(Role role);
    
    /// <summary>
    /// Deletes a role
    /// </summary>
    Task<bool> DeleteRole(int roleId);
} 