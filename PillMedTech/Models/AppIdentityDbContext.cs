using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PillMedTech.Models
{
    /* 
     1. Datalagring 
        1.1. Olika databaser som hanterar olika typer av data (ex. inloggningsuppgifter, loggar, datahanteringen etc).
     */

    public class AppIdentityDbContext : IdentityDbContext<IdentityUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }
    }
}
