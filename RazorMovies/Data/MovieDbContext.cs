using Microsoft.EntityFrameworkCore;
using RazorMovies.Models;

namespace RazorMovies.Data
{
    public class MovieDbContext: DbContext
    {
        public MovieDbContext(
            DbContextOptions<MovieDbContext> options): base(options)
        {
            
        }
        public DbSet<Movie> Movie { get; set; }
    }
}