using CommentStorage.Api.Models;
using CommentStorage.Api.Persistence.CommentStorageDb.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace CommentStorage.Api.Persistence.CommentStorageDb;

public class CommentStorageDbContext : DbContext
{
    public CommentStorageDbContext(DbContextOptions<CommentStorageDbContext> options) : base(options)
    {
        
    }

    public DbSet<Comment> Comments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CommentEntityConfigurations());
    }
}