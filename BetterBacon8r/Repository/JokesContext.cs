using Microsoft.EntityFrameworkCore;

public class JokesDbContext : DbContext {
    public JokesDbContext(DbContextOptions<JokesDbContext> options) : base(options) { }

    public DbSet<Jokes> Jokes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // Example of model configuration:
        modelBuilder.Entity<Jokes>()
            .Property(j => j.Id)
            .ValueGeneratedOnAdd();
    }
}