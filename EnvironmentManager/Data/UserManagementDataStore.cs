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
            // Check if user exists in the database before updating
            var existingUser = _context.Users.AsNoTracking().FirstOrDefault(u => u.Username == user.Username);
            if (existingUser == null)
            {
                throw new Exception($"User {user.Username} not found in the database");
            }
            
            // Create a complete detached copy of the existing user before making any changes
            var oldUser = new User
            {
                Username = existingUser.Username,
                Password = existingUser.Password, 
                Role = existingUser.Role  // This will set the internal _role field
            };
            
            // Apply properties to existing entity (tracked by EF)
            var entityToUpdate = _context.Users.Find(existingUser.Username);
            if (entityToUpdate == null)
            {
                throw new Exception($"Could not find user {user.Username} for tracked update");
            }
            
            // Apply changes to tracked entity
            entityToUpdate.Role = user.Role;
            if (!string.IsNullOrEmpty(user.Password))
            {
                entityToUpdate.Password = user.Password;
            }
            
            // Save changes through EF Core 
            await _context.SaveChangesAsync();
            
            // Log the user update 
            await _logService.LogUserUpdatedAsync(oldUser, entityToUpdate);
            
            return entityToUpdate;
        }
        catch (Exception ex)
        {
            // Log the exception details
            Debug.WriteLine($"Error updating user {user?.Username}: {ex.Message}");
            
            // Create a more descriptive exception with additional context
            throw new UserManagementException($"Failed to update user {user?.Username}. See inner exception for details.", ex);
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

/// <summary>
/// Custom exception for user management operations to provide better context
/// </summary>
public class UserManagementException : Exception
{
    public UserManagementException(string message) : base(message) { }
    
    public UserManagementException(string message, Exception innerException) 
        : base(message, innerException) { }
} 