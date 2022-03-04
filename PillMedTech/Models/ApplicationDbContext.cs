using Microsoft.EntityFrameworkCore;

namespace PillMedTech.Models
{
    public class ApplicationDbContext : DbContext
    {

        /* 
         1. Datalagring 
            1.1. Olika databaser som hanterar olika typer av data (ex. inloggningsuppgifter, loggar, datahanteringen etc).
         */

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<SickErrand> SickErrands { get; set; }
        public DbSet<Children> Childrens { get; set; }
    }
}
