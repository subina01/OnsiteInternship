
using Microsoft.EntityFrameworkCore;
using sample_redistestapp;
using sample_redistestapp.Data;
using sample_redistestapp.Entity;
using sample_redistestapp.RedisHelper;
using System.Text.Json;

Console.WriteLine("Starting application...");

// Initialize DbContext
using var context = new AppDbContext();

// Apply migrations (create DB if not exists)
context.Database.Migrate();

// ------------------- Seed Users from JSON -------------------
var jsonFilePath = "users.json"; // JSON file path in project root

if (System.IO.File.Exists(jsonFilePath))
{
    Console.WriteLine("Seeding users from JSON...");

    // Read and deserialize JSON
    var json = System.IO.File.ReadAllText(jsonFilePath);
    var users = JsonSerializer.Deserialize<List<User>>(json);

    if (users != null && users.Count > 0)
    {
        int batchSize = 5000; // Insert in batches
        for (int i = 0; i < users.Count; i += batchSize)
        {
            var batch = users.Skip(i).Take(batchSize);
            context.Users.AddRange(batch);
            context.SaveChanges();
            Console.WriteLine($"Inserted {Math.Min(i + batchSize, users.Count)} / {users.Count} users...");
        }

        Console.WriteLine("All users inserted successfully!");
    }
}
else
{
    Console.WriteLine($"JSON file not found at path: {jsonFilePath}");
}

// ------------------- Optional: Display first 10 users -------------------
var firstUsers = context.Users.Take(10).ToList();
Console.WriteLine("First 10 users in the database:");
foreach (var user in firstUsers)
{
    Console.WriteLine($"User: {user.Name}, Email: {user.Email}");
}

// ------------------- Redis Integration -------------------
var redis = new RedisHelper("localhost:6379"); 
redis.SetValue("greeting", "Hello from Redis!");
var value = redis.GetValue("greeting");
Console.WriteLine($"Redis value: {value}");
