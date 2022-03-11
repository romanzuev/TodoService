using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    internal class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TodoItem>()
                .ToTable("Todos")
                .HasKey(e => e.Id);

            builder.Entity<TodoItem>()
                .HasIndex(e => e.Name);
        }
    }
}