using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore; // Biblioteka do bazy danych
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using system_zawodnicy_zimowi.core.Domain.Entities; // Import Twoich klas (Zawodnik, Klub...)
using system_zawodnicy_zimowi.core.Domain.Enums;    // Import Twoich enumów (Dyscyplina...)


namespace system_zawodnicy_zimowi.Data
{
    // Ta klasa odpowiada za połączenie z bazą. Musi dziedziczyć po DbContext.
    public class AppDbContext : DbContext
    {
        // Tu mówimy bazie: "Chcę mieć takie tabele"
        public DbSet<Zawodnik> Zawodnicy { get; set; }
        public DbSet<KlubSportowy> Kluby { get; set; }
        public DbSet<WynikZawodow> Wyniki { get; set; }

        // Konfiguracja połączenia (ConnectionString)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Używamy lokalnej bazy SQL Server wbudowanej w Visual Studio
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ZawodnicyZimowiDB;Trusted_Connection=True;");
        }

        // Konfiguracja szczegółów (jak zapisać trudne pola)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. DZIEDZICZENIE (Narciarz/Snowboardzista w jednej tabeli)
            modelBuilder.Entity<Zawodnik>()
                .HasDiscriminator<string>("TypZawodnika")
                .HasValue<Snowboardzista>("Snowboardzista")
                .HasValue<NarciarzAlpejski>("Narciarz");

            // 2. PRYWATNA LISTA WYNIKÓW
            modelBuilder.Entity<Zawodnik>()
                .Metadata
                .FindNavigation(nameof(Zawodnik.Wyniki))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // 3. KONFIGURACJA LISTY DYSCYPLIN (Dla Klubu)

            // A. Konwerter (zapisuje listę jako tekst "1,2,3")
            var dyscyplinyConverter = new ValueConverter<List<Dyscyplina>, string>(
                v => string.Join(",", v.Select(e => (int)e)),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(e => (Dyscyplina)int.Parse(e)).ToList());

            // B. Komparator (NOWOŚĆ - mówi bazie jak porównywać dwie listy)
            var dyscyplinyComparer = new ValueComparer<List<Dyscyplina>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            modelBuilder.Entity<KlubSportowy>()
                .Property(k => k.Dyscypliny)
                .HasConversion(dyscyplinyConverter)
                .Metadata.SetValueComparer(dyscyplinyComparer); // <-- Tu przypinamy komparator
        }
    }
}