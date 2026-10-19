using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

class Program
{
    private static readonly string BackupFilePath = "blogs_backup.json";

    static async Task Main(string[] args)
    {
        // 1. Ensure a local backup file exists with sample data
        await SeedBackupFileAsync();

        Console.WriteLine("Initializing EF Core with an invalid connection string...");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=tcp:nonexistent.database.windows.net,1433;Database=TestDb;User ID=test;Password=test;")
            .AddInterceptors(new DatabaseFallbackInterceptor())
            .Options;

        using var context = new AppDbContext(options);

        try
        {
            Console.WriteLine("Attempting to query database...");
            var blogs = await context.Blogs.ToListAsync();
            
            foreach (var b in blogs)
            {
                Console.WriteLine($"Blog: {b.Title}");
            }
        }
        catch (Exception)
        {
            Console.WriteLine("\n[APP] Database unreachable. Falling back to local JSON file...");
            
            // 2. Load blogs from local file when database fails
            var localBlogs = await LoadBlogsFromFileAsync();

            foreach (var blog in localBlogs)
            {
                Console.WriteLine($" -> [Local File] ID: {blog.Id}, Title: {blog.Title}");
            }
        }
    }

    private static async Task SeedBackupFileAsync()
    {
        if (!File.Exists(BackupFilePath))
        {
            var sampleBlogs = new List<Blog>
            {
                new Blog { Id = 1, Title = "Offline Backup Blog: EF Core Interceptors" },
                new Blog { Id = 2, Title = "Offline Backup Blog: Local File Fallback" }
            };
            var json = JsonSerializer.Serialize(sampleBlogs, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(BackupFilePath, json);
        }
    }

    private static async Task<List<Blog>> LoadBlogsFromFileAsync()
    {
        if (!File.Exists(BackupFilePath)) return new List<Blog>();

        var json = await File.ReadAllTextAsync(BackupFilePath);
        return JsonSerializer.Deserialize<List<Blog>>(json) ?? new List<Blog>();
    }
}

// 1. Interceptor catches the failure and logs it asynchronously
public class DatabaseFallbackInterceptor : IDbCommandInterceptor
{
    public async Task CommandFailedAsync(
        DbCommand command, 
        CommandErrorEventData eventData, 
        CancellationToken cancellationToken = default)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[INTERCEPTOR] CommandFailedAsync triggered: {eventData.Exception.Message}");
        Console.ResetColor();

        await Task.CompletedTask;
    }

    public void CommandFailed(DbCommand command, CommandErrorEventData eventData) { }
}

// 2. Entity and DbContext definition
public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<Blog> Blogs => Set<Blog>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}