# Guideline 

    dotnet new webapp -o RazorMovies
    dotnet dev-certs https --trust
    
Add the `Movie` entity 

Then run the below commands

    dotnet tool install --global dotnet-ef
    dotnet tool install --global dotnet-aspnet-codegenerator
    dotnet add package Microsoft.EntityFrameworkCore.SQLite
    dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
    dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
    dotnet add package Microsoft.EntityFrameworkCore.Design
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    dotnet add package Microsoft.Extensions.Logging.Debug
    
Add `MovieDbContext` database context class. 

Add database connection string at the `RazorMovies.csproj`

    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft": "Warning",
          "Microsoft.Hosting.Lifetime": "Information"
        }
      },
      "AllowedHosts": "*",
      "ConnectionStrings": {
        "DevMovieDbContext": "Data Source=Movies.db"
      }
    }
    
Register the database context at `Startup.cs`

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddDbContext<MovieDbContext>(options =>
            options.UseSqlite(Configuration.GetConnectionString("MovieDbContext")));
    }
    
Scaffold the movie model (remember to build the project before running the below command)

    dotnet-aspnet-codegenerator razorpage -m Movie -dc MovieDbContext -udl -outDir Pages/Movies --referenceScriptLibraries

SQLite for development, SQL Server for production 

    public class Startup
    {
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
    Environment = env;
    Configuration = configuration;
    }
    
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
    
        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.IsDevelopment())
            {
                services.AddDbContext<RazorPagesMovieContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("RazorPagesMovieContext")));
            }
            else
            {
                services.AddDbContext<RazorPagesMovieContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("MovieContext")));
            }
    
            services.AddRazorPages();
        }
        ...
    }

Create the initial database schema 

    dotnet ef migrations add InitialCreate
    dotnet ef database update

You can delete the database and then run the `dotnet ef database update` for re-create the database

Create the application `SeedData` class in Models folder 

Change the `Program.cs`

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using RazorMovies.Models;
    
    namespace RazorMovies
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var host = CreateHostBuilder(args).Build();
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        SeedData.Initialize(services);
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred seeding the DB.");
                    }
                }
                host.Run();
            }
        
            public static IHostBuilder CreateHostBuilder(string[] args) =>
                    Host.CreateDefaultBuilder(args)
                        .ConfigureWebHostDefaults(webBuilder =>
                        {
                            webBuilder.UseStartup<Startup>();
                        });
            }
    }

Install the Db Browser for SQLite for data examine. On MacOS, you can use HomeBrew `brew cask install db-browser-for-sqlite`

When changing the model you need to add migration, but for the current limitation of SQLite driver, we can only delete all migrations then create migration again to update the database. 

    dotnet ef database drop
    dotnet ef migrations add InitialCreate
    dotnet ef database update