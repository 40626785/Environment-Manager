using System;
using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces;

//Enables Dependency Inversion Principle, allowing management of sessions without directly depending on session class
public interface ISessionService
{
   User? AuthenticatedUser { get; }
   DateTime? Expiry { get; }
    
   void NewSession(User user);
}
