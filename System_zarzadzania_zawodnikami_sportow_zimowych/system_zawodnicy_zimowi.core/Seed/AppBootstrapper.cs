using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Interfaces;
using system_zawodnicy_zimowi.core.Infrastructure;
using system_zawodnicy_zimowi.core.Services;

namespace system_zawodnicy_zimowi.core.Seed
{
    public class AppBootstrapper
    {
        public IZawodnicyRepository ZawodnicyRepo { get; }
        public IKlubyRepository KlubyRepo { get; }
        public AplikacjaCoreService Core { get; }

        public AppBootstrapper()
        {
            ZawodnicyRepo = new InMemoryZawodnicyRepository();
            KlubyRepo = new InMemoryKlubyRepository();
            Core = new AplikacjaCoreService();
        }

        public void LoadSeed()
        {
            var kluby = SeedData.PrzykladoweKluby();
            foreach (var k in kluby) KlubyRepo.Add(k);

            var zawodnicy = SeedData.PrzykladowiZawodnicy();
            foreach (var z in zawodnicy)
            {
                Core.PrzeliczPunktyIZaktualizuj(z, kluby);
                ZawodnicyRepo.Add(z);
            }
        }
    }

}
