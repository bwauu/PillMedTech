using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PillMedTech.Models
{
    public class LoggDbContext : DbContext
    {   
  
        public LoggDbContext(DbContextOptions<LoggDbContext> options) : base(options) { }
        public DbSet<Logger> Loggers { get; set; }
    }
}
