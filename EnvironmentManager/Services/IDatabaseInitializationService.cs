namespace EnvironmentManager.Services
{
    public interface IDatabaseInitializationService
    {
        /// <summary>
        /// Verifies database connections without performing extensive testing
        /// </summary>
        Task VerifyDatabaseConnectionsAsync();

        /// <summary>
        /// Loads test data if needed - should only be called in development environments
        /// </summary>
        Task LoadTestDataIfNeededAsync();
    }
}
