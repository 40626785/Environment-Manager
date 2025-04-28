using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;

namespace EnvironmentManager.Services
{
    public class UserLogService : IUserLogService
    {
        private readonly UserLogDbContext _logContext;
        private readonly ISessionService _sessionService;

        public UserLogService(UserLogDbContext logContext, ISessionService sessionService)
        {
            _logContext = logContext;
            _sessionService = sessionService;
        }

        /// <summary>
        /// Logs a user creation event
        /// </summary>
        public async Task LogUserCreatedAsync(User user)
        {
            try
            {
                var log = new UserLog
                {
                    Username = user.Username,
                    ActionType = "CREATE",
                    ChangedFields = "All",
                    OldValues = null,
                    NewValues = JsonSerializer.Serialize(new { 
                        user.Username, 
                        Role = user.Role.ToString(), 
                        HasPassword = !string.IsNullOrEmpty(user.Password)
                    }),
                    PerformedBy = _sessionService.AuthenticatedUser?.Username ?? "System",
                    Timestamp = DateTime.Now
                };

                _logContext.UserLogs.Add(log);
                await _logContext.SaveChangesAsync();
                Debug.WriteLine($"User creation logged: {user.Username}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error logging user creation: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a user update event
        /// </summary>
        public async Task LogUserUpdatedAsync(User oldUser, User newUser)
        {
            try
            {
                Debug.WriteLine($"LogUserUpdatedAsync: ENTRY POINT - Comparing old user to new user");
                Debug.WriteLine($"Old user object: {oldUser?.GetType().FullName ?? "null"}, Old user reference: {(oldUser == null ? "null" : "not null")}");
                Debug.WriteLine($"New user object: {newUser?.GetType().FullName ?? "null"}, New user reference: {(newUser == null ? "null" : "not null")}");
                
                if (oldUser == null || newUser == null)
                {
                    Debug.WriteLine("ERROR: Either oldUser or newUser is null, cannot log update");
                    return;
                }
                
                Debug.WriteLine($"Old user: {oldUser.Username}, Role: {oldUser.Role} (Value: {(int)oldUser.Role})");
                Debug.WriteLine($"New user: {newUser.Username}, Role: {newUser.Role} (Value: {(int)newUser.Role})");
                
                var changedFields = new List<string>();
                
                // Debug more details about the roles
                Debug.WriteLine($"Role comparison: oldUser.Role (enum type): {oldUser.Role.GetType().FullName}");
                Debug.WriteLine($"Role comparison: newUser.Role (enum type): {newUser.Role.GetType().FullName}");
                Debug.WriteLine($"Role comparison: oldUser.Role value: {(int)oldUser.Role}, newUser.Role value: {(int)newUser.Role}");
                Debug.WriteLine($"Role comparison result: {(int)oldUser.Role != (int)newUser.Role}");
                
                if ((int)oldUser.Role != (int)newUser.Role)
                {
                    Debug.WriteLine($"Role changed from {oldUser.Role} ({(int)oldUser.Role}) to {newUser.Role} ({(int)newUser.Role})");
                    changedFields.Add("Role");
                }
                else
                {
                    Debug.WriteLine("Role did not change");
                }
                
                // Check password change
                bool passwordChanged = !string.IsNullOrEmpty(newUser.Password) && oldUser.Password != newUser.Password;
                Debug.WriteLine($"Password check: oldPassword null/empty: {string.IsNullOrEmpty(oldUser.Password)}, newPassword null/empty: {string.IsNullOrEmpty(newUser.Password)}");
                Debug.WriteLine($"Password changed: {passwordChanged}");
                
                if (passwordChanged)
                {
                    Debug.WriteLine("Password changed");
                    changedFields.Add("Password");
                }
                else
                {
                    Debug.WriteLine("Password did not change or was not provided");
                }
                
                if (changedFields.Count == 0)
                {
                    Debug.WriteLine("No fields changed, skipping log");
                    return;
                }
                
                // Create the log entry
                Debug.WriteLine($"Creating log entry with changed fields: {string.Join(", ", changedFields)}");
                var log = new UserLog
                {
                    Username = newUser.Username,
                    ActionType = "UPDATE",
                    ChangedFields = string.Join(",", changedFields),
                    OldValues = JsonSerializer.Serialize(new { 
                        Role = oldUser.Role.ToString(),
                        PasswordChanged = changedFields.Contains("Password")
                    }),
                    NewValues = JsonSerializer.Serialize(new { 
                        Role = newUser.Role.ToString(),
                        PasswordChanged = changedFields.Contains("Password")
                    }),
                    PerformedBy = _sessionService.AuthenticatedUser?.Username ?? "System",
                    Timestamp = DateTime.Now
                };

                _logContext.UserLogs.Add(log);
                Debug.WriteLine("Log entry added to context, saving changes...");
                var result = await _logContext.SaveChangesAsync();
                Debug.WriteLine($"SaveChangesAsync result: {result} records affected");
                Debug.WriteLine($"User update logged: {newUser.Username}, Changed fields: {log.ChangedFields}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error logging user update: {ex.Message}");
                Debug.WriteLine($"Exception type: {ex.GetType().FullName}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        /// <summary>
        /// Logs a user deletion event
        /// </summary>
        public async Task LogUserDeletedAsync(User user)
        {
            try
            {
                var log = new UserLog
                {
                    Username = user.Username,
                    ActionType = "DELETE",
                    ChangedFields = "All",
                    OldValues = JsonSerializer.Serialize(new { 
                        user.Username, 
                        Role = user.Role.ToString()
                    }),
                    NewValues = null,
                    PerformedBy = _sessionService.AuthenticatedUser?.Username ?? "System",
                    Timestamp = DateTime.Now
                };

                _logContext.UserLogs.Add(log);
                await _logContext.SaveChangesAsync();
                Debug.WriteLine($"User deletion logged: {user.Username}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error logging user deletion: {ex.Message}");
            }
        }
    }
} 