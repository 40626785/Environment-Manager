using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using EnvironmentManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EnvironmentManager.Data;

/// <summary>
/// Provides data access logic for user and role management operations.
/// Implements <see cref="IUserManagementDataStore"/>.
/// </summary>
public class UserManagementDataStore : IUserManagementDataStore
{
    private readonly UserManagementDbContext _context;
    private readonly IUserLogService _logService;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UserManagementDataStore"/> class.
    /// </summary>
    /// <param name="context">The database context for user management.</param>
    /// <param name="logService">The service for logging user actions.</param>
    public UserManagementDataStore(UserManagementDbContext context, IUserLogService logService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
    }
    
    /// <summary>
    /// Gets all users from the database.
    /// </summary>
    /// <returns>An enumerable collection of all users.</returns>
    public IEnumerable<User> GetAllUsers()
    {
        // Consider adding .Include(u => u.RoleNavigation) if Role details are needed immediately.
        // Using AsNoTracking() if this is purely for display list performance.
        // var users = _context.Users.AsNoTracking().ToList(); 
        var users = _context.Users.ToList(); // Keep tracking for potential updates/deletes via the same instance
        return users;
    }
    
    /// <summary>
    /// Gets a specific user by their username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>The found <see cref="User"/> or null if not found.</returns>
    public User? GetUser(string username)
    {
        // Uses SingleOrDefault to find a unique user or return null.
        return _context.Users.SingleOrDefault(u => u.Username == username);
    }
    
    /// <summary>
    /// Searches for users whose usernames contain the specified query string (case-sensitive).
    /// </summary>
    /// <param name="query">The search term to look for within usernames.</param>
    /// <returns>An enumerable collection of matching users.</returns>
    public IEnumerable<User> SearchUsers(string query)
    {
        // Basic substring search. Consider case-insensitivity or more advanced search if needed.
        var results = _context.Users.Where(u => u.Username != null && u.Username.Contains(query)).ToList();
        return results;
    }
    
    /// <summary>
    /// Gets all defined roles from the database.
    /// </summary>
    /// <returns>An enumerable collection of all roles.</returns>
    public IEnumerable<Role> GetAllRoles()
    {
        return _context.Roles.ToList();
    }
    
    /// <summary>
    /// Gets a specific role by its ID.
    /// </summary>
    /// <param name="roleId">The ID of the role to find.</param>
    /// <returns>The found <see cref="Role"/> or null if not found.</returns>
    public Role? GetRole(int roleId)
    {
        return _context.Roles.Find(roleId);
    }
    
    /// <summary>
    /// Creates a new user in the database and logs the action.
    /// </summary>
    /// <param name="user">The user object to create.</param>
    /// <returns>The created user object, potentially with database-generated values.</returns>
    public async Task<User> CreateUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        // Log the user creation event after successful save
        await _logService.LogUserCreatedAsync(user);
        
        return user;
    }
    
    /// <summary>
    /// Updates an existing user in the database and logs the changes.
    /// </summary>
    /// <param name="user">The user object with updated information.</param>
    /// <returns>The updated user object.</returns>
    /// <exception cref="UserManagementException">Thrown if the user cannot be found or if the update fails.</exception>
    public async Task<User> UpdateUser(User user)
    {
        // Find the existing entity to ensure we are updating, not inserting.
        // Use AsNoTracking for the initial check to avoid tracking conflicts.
        var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == user.Username);
        if (existingUser == null)
        {
            throw new UserManagementException($"User '{user.Username}' not found for update.");
        }
        
        // Log changes requires knowing the state *before* the update.
        // Create a representation of the old state for logging.
        var oldUserForLog = new User
        {
            Username = existingUser.Username,
            Password = null, // Avoid logging old password hash
            RoleId = existingUser.RoleId // Log the old RoleId
        };
        
        try
        {
            // Retrieve the tracked entity to apply updates
            var entityToUpdate = await _context.Users.FindAsync(user.Username);
            if (entityToUpdate == null)
            {
                 // This should ideally not happen if the AsNoTracking check passed, but handle defensively.
                 throw new UserManagementException($"Could not find tracked user '{user.Username}' for update.");
            }

            // Apply changes from the passed user object to the tracked entity
            entityToUpdate.RoleId = user.RoleId; // Update RoleId
            // Only update the password if a new one was actually provided
            if (!string.IsNullOrEmpty(user.Password))
            {
                entityToUpdate.Password = user.Password; // Assume password is pre-hashed if necessary
            }
            // Note: Username (Primary Key) should generally not be updated.

            // Save changes to the database
            await _context.SaveChangesAsync();
            
            // Log the update operation after successful save
            // Pass the representation of the old state and the *updated* tracked entity.
            await _logService.LogUserUpdatedAsync(oldUserForLog, entityToUpdate);
            
            return entityToUpdate;
        }
        catch (DbUpdateException dbEx)
        {
            Debug.WriteLine($"Database error updating user '{user?.Username}': {dbEx.Message}");
            throw new UserManagementException($"Database error updating user '{user?.Username}'. See inner exception.", dbEx);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"General error updating user '{user?.Username}': {ex.Message}");
            throw new UserManagementException($"Failed to update user '{user?.Username}'. See inner exception.", ex);
        }
    }
    
    /// <summary>
    /// Deletes a user from the database and logs the action.
    /// </summary>
    /// <param name="user">The user object to delete.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
    public async Task<bool> DeleteUser(User user)
    {
        // Log the deletion intent first
        // Pass a representation of the user being deleted
        var userToDeleteForLog = new User { Username = user.Username, RoleId = user.RoleId }; 
        await _logService.LogUserDeletedAsync(userToDeleteForLog);
        
        // Remove the user from the context and save changes
        _context.Users.Remove(user); // Assumes 'user' is tracked; if not, find first: var trackedUser = await _context.Users.FindAsync(user.Username);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    
    /// <summary>
    /// Creates a new role in the database.
    /// </summary>
    /// <param name="role">The role object to create.</param>
    /// <returns>The created role object.</returns>
    public async Task<Role> CreateRole(Role role)
    {
        role.CreatedDate = DateTime.UtcNow; // Use UtcNow for consistency
        role.LastModifiedDate = DateTime.UtcNow;
        
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }
    
    /// <summary>
    /// Updates an existing role in the database.
    /// </summary>
    /// <param name="role">The role object with updated information.</param>
    /// <returns>The updated role object.</returns>
    public async Task<Role> UpdateRole(Role role)
    {
        role.LastModifiedDate = DateTime.UtcNow;
        
        _context.Entry(role).State = EntityState.Modified;
        // Consider loading the existing entity first to apply specific changes if needed
        await _context.SaveChangesAsync();
        return role;
    }
    
    /// <summary>
    /// Deletes a role from the database by its ID.
    /// </summary>
    /// <param name="roleId">The ID of the role to delete.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
    public async Task<bool> DeleteRole(int roleId)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
        {
            Debug.WriteLine($"Role with ID {roleId} not found for deletion.");
            return false;
        }
            
        // Add check: ensure role is not currently assigned to any users before deleting
        bool isRoleInUse = await _context.Users.AnyAsync(u => u.RoleId == roleId);
        if (isRoleInUse)
        {
            Debug.WriteLine($"Attempted to delete Role ID {roleId} which is currently assigned to users.");
            throw new UserManagementException($"Cannot delete role '{role.RoleName}' as it is currently assigned to one or more users.");
        }

        _context.Roles.Remove(role);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
} 

/// <summary>
/// Custom exception for user management operations.
/// Provides more specific context than general exceptions.
/// </summary>
public class UserManagementException : Exception
{
    public UserManagementException() { }
    public UserManagementException(string message) : base(message) { }
    public UserManagementException(string message, Exception innerException) 
        : base(message, innerException) { }
} 