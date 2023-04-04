using CovidChart.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CovidChart.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Covid> Covids { get; set; }
}