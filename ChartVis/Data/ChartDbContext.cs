using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class ChartDbContext : DbContext
{
    public ChartDbContext(DbContextOptions<ChartDbContext> options) : base(options) { }
    public DbSet<Sales> Sales { get; set; }
}

public class Sales
{
    public int Id { get; set; }
    public string? Category { get; set; }
    public int Year { get; set; }
    public decimal SalesAmount { get; set; }
}
