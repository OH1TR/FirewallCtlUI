using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirewallCtlUI.DB
{
    public class FWContext : DbContext
    {
        public DbSet<Parameter> Parameters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=FirewallCtl.db");
        }
    }
}