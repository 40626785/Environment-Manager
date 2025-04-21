using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Exceptions;
using System.Diagnostics;

namespace EnvironmentManager.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private IUserDataStore _context;
        private ISessionService _session;
        private bool _authenticated;
        private User _authenticatedUser;

        public bool Authenticated => _authenticated;
        public User AuthenticatedUser => _authenticatedUser;

        public AuthenticationService(IUserDataStore context, ISessionService session)
        {
            _context = context;
            _session = session;
        }
        
        public void Authenticate(string username, string password){
            User user = _context.GetUser(username); //attempts to retrieve user
            try
            {
                if(user.Password == password)
                {
                    _authenticated = true;
                    _authenticatedUser = user;
                    ConfigureSession();
                }
                else
                {
                    _authenticated = false;
                    throw new LoginException();
                }
            }
            catch(NullReferenceException) //handling of a null value for user.Password (user not in database)
            {
                _authenticated = false;
                throw new LoginException();
            }
        }
        
        private void ConfigureSession() 
        {
            _session.NewSession(AuthenticatedUser);
        }
    }
}
