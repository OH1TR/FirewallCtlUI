using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FirewallCtlUI.DB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.Npcap;

namespace FirewallCtlUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!File.Exists("FirewallCtl.db"))
            {
                using (var db = new FWContext())
                {
                    RelationalDatabaseCreator databaseCreator =
                        (RelationalDatabaseCreator)db.Database.GetService<IDatabaseCreator>();
                    databaseCreator.CreateTables();
                }
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
