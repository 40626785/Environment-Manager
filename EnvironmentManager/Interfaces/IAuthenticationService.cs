using System;
using EnvironmentManager.Models;
using EnvironmentManager.Services;

namespace EnvironmentManager.Interfaces;

//Enables Dependency Inversion Principle, allowing management of authentication without directly depending on session class
public interface IAuthenticationService
{
    bool Authenticated { get; }
    User AuthenticatedUser { get; }
    
    void Authenticate(string username, string password);
}
