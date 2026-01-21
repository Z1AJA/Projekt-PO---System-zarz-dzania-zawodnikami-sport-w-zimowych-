using System;
using System.Collections.Generic;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;

namespace system_zawodnicy_zimowi.core.Seed
{
    public static class SeedData
    {
        public static List<KlubSportowy> PrzykladoweKluby()
        {
            return new List<KlubSportowy>
            {
                new KlubSportowy(
                    nazwa: "Młodzicy IcePeak",
                    minimalnePunkty: 0,
                    maksWiek: 18,
                    dyscypliny: new[] { Dyscyplina.NarciarstwoAlpejskie, Dyscyplina.Snowboard },
                    limitMiejsc: 50),

                new KlubSportowy(
                    nazwa: "Alpine Pro Team",
                    minimalnePunkty: 8000,
                    maksWiek: null,
                    dyscypliny: new[] { Dyscyplina.NarciarstwoAlpejskie },
                    limitMiejsc: 40),

                new KlubSportowy(
                    nazwa: "Snow Riders Elite",
                    minimalnePunkty: 9000,
                    maksWiek: null,
                    dyscypliny: new[] { Dyscyplina.Snowboard },
                    limitMiejsc: 35),

                new KlubSportowy(
                    nazwa: "Team Semi-Pro",
                    minimalnePunkty: 3000,
                    maksWiek: 26,
                    dyscypliny: new[] { Dyscyplina.NarciarstwoAlpejskie, Dyscyplina.Snowboard },
                    limitMiejsc: 60),
            };
        }

        public static List<Zawodnik> PrzykladowiZawodnicy()
        {
            var z1 = new NarciarzAlpejski("Jan", "Kowalski", 17);
            var z2 = new NarciarzAlpejski("Anna", "Nowak", 23);
            var z3 = new Snowboardzista("Piotr", "Zieliński", 20);
            var z4 = new Snowboardzista("Kaja", "Wójcik", 27);

            //1.TWORZYMY RODZAJE ZAWODÓW 
            var rWinterA = new RodzajZawodow("Puchar Winter A", 3, 450);
            var rWinterB = new RodzajZawodow("Puchar Winter B", 4, 500);

            var rAlpine = new RodzajZawodow("Alpine Cup", 3, 380);
            var rAlpineFinal = new RodzajZawodow("Alpine Cup Final", 4, 420);

            var rSnowJam = new RodzajZawodow("Snow Jam", 4, 520);
            var rSnowJam2 = new RodzajZawodow("Snow Jam 2", 5, 600);

            var rRiders = new RodzajZawodow("Riders Open", 2, 260);
            var rRidersFinal = new RodzajZawodow("Riders Open Final", 3, 330);

            //2. DODAJEMY WYNIKI

            // Jan Kowalski
            z1.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-30), 12, rWinterA));
            z1.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-10), 8, rWinterB));

            // Anna Nowak
            z2.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-20), 25, rAlpine));
            z2.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-7), 14, rAlpineFinal));

            // Piotr Zieliński
            z3.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-18), 6, rSnowJam));
            z3.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-5), 3, rSnowJam2));

            // Kaja Wójcik
            z4.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-12), 40, rRiders));
            z4.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-3), 22, rRidersFinal));

            return new List<Zawodnik> { z1, z2, z3, z4 };
        }

        //Metoda pomocnicza
        public static List<RodzajZawodow> PrzykladoweRodzaje()
        {
            return new List<RodzajZawodow>
             {
                 new RodzajZawodow("Puchar Winter A", 3, 450),
                 new RodzajZawodow("Puchar Winter B", 4, 500),
                 new RodzajZawodow("Alpine Cup", 3, 380),
                 new RodzajZawodow("Snow Jam", 4, 520)
             };
        }
    }
}