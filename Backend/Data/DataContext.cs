using Microsoft.EntityFrameworkCore;

namespace Ewidencjomat.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }
    // public DbSet<{Entity}> {Entities} { get; set; }
    
    public DbSet<User> Users { get; set; }
}