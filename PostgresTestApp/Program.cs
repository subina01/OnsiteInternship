using System;
using Npgsql;

class Program
{
    static void Main()
    {
        // Connection string
        var connectionString = "Host=localhost;Port=5432;Database=MyAppDb;Username=myuser;Password=mypassword";

        try
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL successfully!");

            // Sample query
            using var cmd = new NpgsqlCommand("SELECT version();", conn);
            var version = cmd.ExecuteScalar();
            Console.WriteLine($"PostgreSQL version: {version}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to PostgreSQL: {ex.Message}");
        }
    }
}

