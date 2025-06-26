using Microsoft.EntityFrameworkCore;

public class JokesDbContext : DbContext {
    public JokesDbContext(DbContextOptions<JokesDbContext> options) : base(options) { }

    public DbSet<Jokes> Jokes { get; set; }
}