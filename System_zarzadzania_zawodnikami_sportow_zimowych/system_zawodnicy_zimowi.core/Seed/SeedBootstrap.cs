using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Services;

namespace system_zawodnicy_zimowi.core.Seed
{
    public static class SeedBootstrap
    {
        public static (List<Zawodnik> zawodnicy, List<KlubSportowy> kluby) BuildReadyData()
        {
            var kluby = SeedData.PrzykladoweKluby();
            var zawodnicy = SeedData.PrzykladowiZawodnicy();

            var core = new AplikacjaCoreService();
            foreach (var z in zawodnicy)
            {
                core.PrzeliczPunktyIZaktualizuj(z, kluby);
            }

            return (zawodnicy, kluby);
        }
    }
}
