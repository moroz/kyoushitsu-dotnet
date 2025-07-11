// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using Npgsql;

Console.WriteLine("Hello, world!");

public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime PublishedOn { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
}

public class Author
{
    public int AuthorId { get; set; }
    public string Name { get; set; }
    public string WebUrl { get; set; }
}

public class AppDbContext : DbContext
{
    private const string ConnectionString =
        @"Host=localhost; Database=my_first_ef_core_app_dev; Username=postgres; Password=postgres";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(ConnectionString);
    }

    public DbSet<Book> Books { get; set; }
}