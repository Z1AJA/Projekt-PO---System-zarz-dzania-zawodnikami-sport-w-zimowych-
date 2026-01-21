using Microsoft.EntityFrameworkCore;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;
using system_zawodnicy_zimowi.Data; 

namespace system_zawodnicy_zimowi.Data
{
    public class ManagerDanych
    {
        public void DodajWynikDoBazy(Guid zawodnikId, WynikZawodow nowyWynik)
        {
            using (var context = new AppDbContext())
            {
                var zawodnik = context.Zawodnicy
                                      .Include(z => z.Wyniki)
                                      .FirstOrDefault(z => z.Id == zawodnikId);

                if (zawodnik != null)
                {
                 
                    zawodnik.DodajWynik(nowyWynik);
                    context.SaveChanges();
                }
            }
        }
       
        public void InicjalizujBaze()
        {
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();

                if (!context.Zawodnicy.Any())
                {
                    var k1 = new KlubSportowy("Zimowe Orły", 100, 30, new[] { Dyscyplina.NarciarstwoAlpejskie });

                    var z1 = new NarciarzAlpejski("Adam", "Małysz", 45);
                    z1.SetPunktyIRange(500, Ranga.Junior);
                    z1.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-10), "Puchar Tatr", 1, 5, 100));
                    z1.PrzypiszKlub(k1.Id, k1.Nazwa);

                    var z2 = new Snowboardzista("Shaun", "White", 35);
                    z2.SetPunktyIRange(450, Ranga.Pro);

                    context.Kluby.Add(k1);
                    context.Zawodnicy.AddRange(z1, z2);

                    context.SaveChanges(); // Zapis do SQL (COMMIT)
                }
            }
        }

        // --- Poniżej wymagane zapytania LINQ (pkt 17) ---

        public List<Zawodnik> PobierzWszystkich()
        {
            using (var context = new AppDbContext())
            {
                // Include ładuje też wyniki (JOIN w SQL)
                return context.Zawodnicy.Include(z => z.Wyniki).ToList();
            }
        }

        // Przykład LINQ: Filtrowanie i Sortowanie
        public List<Zawodnik> ZnajdzNajlepszych(Dyscyplina dyscyplina)
        {
            using (var context = new AppDbContext())
            {
                return context.Zawodnicy
                    .Where(z => z.Dyscyplina == dyscyplina && z.Punkty > 100)
                    .OrderByDescending(z => z.Punkty)
                    .ToList();
            }
        }

        // Przykład LINQ: Wyszukiwanie po nazwisku
        public Zawodnik? ZnajdzPoNazwisku(string nazwisko)
        {
            using (var context = new AppDbContext())
            {
                return context.Zawodnicy
                    .FirstOrDefault(z => z.Nazwisko == nazwisko);
            }
        }
        public double ObliczSredniWiekWKlubie(string nazwaKlubu)
        {
            using (var context = new AppDbContext())
            {
                // To jest bardziej zaawansowane LINQ
                var srednia = context.Zawodnicy
                    .Where(z => z.KlubNazwa == nazwaKlubu)
                    .Average(z => z.Wiek);

                return srednia;
            }
        }
    }
}