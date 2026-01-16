using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;


//Przykladowe dane do latwiejszych testow

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

            
            //wygenerowane wyniki zawodow

            z1.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-30), "Puchar Winter A", 12, 3, 450));
            z1.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-10), "Puchar Winter B", 8, 4, 500));

            z2.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-20), "Alpine Cup", 25, 3, 380));
            z2.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-7), "Alpine Cup Final", 14, 4, 420));

            z3.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-18), "Snow Jam", 6, 4, 520));
            z3.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-5), "Snow Jam 2", 3, 5, 600));

            z4.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-12), "Riders Open", 40, 2, 260));
            z4.DodajWynik(new WynikZawodow(DateTime.Now.AddDays(-3), "Riders Open Final", 22, 3, 330));

            return new List<Zawodnik> { z1, z2, z3, z4 };
        }

    }
}