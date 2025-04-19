using System;
using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces;

//Enables Dependency Inversion Principle, allowing ViewModels to retrieve a user without depending on concrete DbContext implementation
public interface IUserDataStore
{
    User GetUser(string username);
}
