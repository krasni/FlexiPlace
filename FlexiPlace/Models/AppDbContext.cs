using FlexiPlace.Models.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Status> Status { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Zahtjev> Zahtjev { get; set; }
        public DbSet<NeradniDanHanfa> NeradniDanHanfa { get; set; }
        public DbSet<Parametar> Parametar { get; set; }
        public DbSet<NeradnoMjesto> NeradnoMjesto { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // [Borota Igor]: Regster store procedure custom object.  
            modelBuilder.Query<SpGetUserDataBP2>();
            modelBuilder.Query<SpGetDjelatnici>();
            modelBuilder.Query<SpGetOrganizacijskeJedinice>();
            modelBuilder.Query<SPGetRadnaMjesta>();

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<Status>().HasData(
                new Status
                {
                    Id = 1,
                    Naziv = "Za odobravanje"
                },

                new Status
                {
                    Id = 2,
                    Naziv = "Odobren"
                },

                new Status
                {
                    Id = 3,
                    Naziv = "Odbijen"
                },

                new Status
                {
                    Id = 4,
                    Naziv = "Neobrađen"
                },

                new Status
                {
                    Id = 5,
                    Naziv = "Obrisan"
                },

                new Status
                {
                    Id = 6,
                    Naziv = "Otkazan"
                }
            );

            modelBuilder.Entity<Parametar>().HasData(
                new Parametar
                {
                    Id  = 1,
                    DozvoljeniBrojDanaMjesec = 10,
                    DozvoljeniBrojaDanaOdobrenje = 10
                }
            );
        }

        public List<SpGetUserDataBP2> GetUserDataBP2(string employeeADName)
        {
            // ovdje se popunjavaju defaulti koje uzimam iz store koja uzima podatke izbp2 !!!!
            List<SpGetUserDataBP2> userDetails = new List<SpGetUserDataBP2>();
            SqlParameter usernameParam = new SqlParameter("@EmployeeADName", employeeADName);
            string sqlQuery = "EXEC [dbo].[spGetUserDataBP2] " +
                                    "@EmployeeADName";
            userDetails = this.Query<SpGetUserDataBP2>().FromSql(sqlQuery, usernameParam).ToList();
            return userDetails;
        }

        public List<SpGetDjelatnici> SpGetDjelatnici()
        {
            List<SpGetDjelatnici> djelatnici = new List<SpGetDjelatnici>();
            string sqlQuery = "EXEC [dbo].[spGetDjelatnici]";
            djelatnici = this.Query<SpGetDjelatnici>().FromSql(sqlQuery).ToList();
            return djelatnici;
        }

        public List<SpGetOrganizacijskeJedinice> SpGetOrganizacijskeJedinice()
        {
            List<SpGetOrganizacijskeJedinice> organizacijskeJedinice = new List<SpGetOrganizacijskeJedinice>();
            string sqlQuery = "EXEC [dbo].[spGetOrganizacijskeJedinice]";
            organizacijskeJedinice = this.Query<SpGetOrganizacijskeJedinice>().FromSql(sqlQuery).ToList();
            return organizacijskeJedinice;
        }

        public List<SPGetRadnaMjesta> SpGetRadnaMjesta()
        {
            List<SPGetRadnaMjesta> radnaMjesta = new List<SPGetRadnaMjesta>();
            string sqlQuery = "EXEC [dbo].[spGetRadnaMjesta]";
            radnaMjesta = this.Query<SPGetRadnaMjesta>().FromSql(sqlQuery).ToList();
            return radnaMjesta;
        }
    }
}
