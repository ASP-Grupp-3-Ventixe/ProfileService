using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Data.Context;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Locate the base path 
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Presentation");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("local.settings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetSection("Values:SqlConnection").Value;
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlServer(connectionString);
        
        return new AppDbContext(optionsBuilder.Options);
    }
}