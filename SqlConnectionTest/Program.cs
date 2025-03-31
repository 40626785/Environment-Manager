using System;
using Microsoft.Data.SqlClient;

namespace SqlConnectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=localhost,1433;Database=notesdb;User Id=notesapp;Password=N0tesApp$;TrustServerCertificate=True;Encrypt=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection successful!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection failed: {ex.Message}");
                }
            }
        }
    }
}
