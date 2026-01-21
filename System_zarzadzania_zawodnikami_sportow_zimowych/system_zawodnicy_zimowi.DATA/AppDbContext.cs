using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using system_zawodnicy_zimowi.core.Domain.Entities; 
using system_zawodnicy_zimowi.core.Domain.Enums;    


namespace system_zawodnicy_zimowi.Data
{
   
    public class AppDbContext : DbContext
    {
        
        public DbSet<Zawodnik> Zawodnicy { get; set; }
        public DbSet<KlubSportowy> Kluby { get; set; }
        public DbSet<WynikZawodow> Wyniki { get; set; }

       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ZawodnicyZimowiDB_v2;Trusted_Connection=True;");
        }

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WynikZawodow>()
        .HasKey(w => w.Id); 

            modelBuilder.Entity<WynikZawodow>()
                .Property(w => w.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Zawodnik>()
                .HasDiscriminator<string>("TypZawodnika")
                .HasValue<Snowboardzista>("Snowboardzista")
                .HasValue<NarciarzAlpejski>("Narciarz");

            
            modelBuilder.Entity<Zawodnik>()
                .Metadata
                .FindNavigation(nameof(Zawodnik.Wyniki))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            

            
            var dyscyplinyConverter = new ValueConverter<List<Dyscyplina>, string>(
                v => string.Join(",", v.Select(e => (int)e)),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(e => (Dyscyplina)int.Parse(e)).ToList());

           
            var dyscyplinyComparer = new ValueComparer<List<Dyscyplina>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            modelBuilder.Entity<KlubSportowy>()
                .Property(k => k.Dyscypliny)
                .HasConversion(dyscyplinyConverter)
                .Metadata.SetValueComparer(dyscyplinyComparer);
        }
    }
}