using FlexiPlaceWinServiceTopShelf.Models;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexiPlaceWinServiceTopShelf.Modles
{
    class FlexiPlaceDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var cs = ConfigurationManager.ConnectionStrings["HangFireConnectionString"]
                                    .ConnectionString;
            optionsBuilder.UseSqlServer(cs);
        }

        public DbSet<Status> Status { get; set; }
        public DbSet<Zahtjev> Zahtjev { get; set; }
        public DbSet<Parametar> Parametar { get; set; }
    }
}
