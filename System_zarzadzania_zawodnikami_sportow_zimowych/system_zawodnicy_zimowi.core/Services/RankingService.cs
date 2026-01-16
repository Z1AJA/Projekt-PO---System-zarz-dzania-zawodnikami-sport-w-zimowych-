using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;

namespace system_zawodnicy_zimowi.core.Services
{
    public class RankingService
    {

        public IReadOnlyList<Zawodnik> SortujPoPunktach(IEnumerable<Zawodnik> zawodnicy)
        {
            if (zawodnicy is null) throw new ArgumentNullException(nameof(zawodnicy));

            return zawodnicy.OrderByDescending(z => z.Punkty).ThenBy(z => z.Nazwisko).ThenBy(z => z.Imie).ToList().AsReadOnly();
        }




        public IReadOnlyList<Zawodnik> Filtruj(
            IEnumerable<Zawodnik> zawodnicy,
            Dyscyplina? dyscyplina = null,
            Ranga? ranga = null,
            int? minPunkty = null,
            int? maxWiek = null,
            Guid? klubId = null)
        {
            if (zawodnicy is null) throw new ArgumentNullException(nameof(zawodnicy));

            var q = zawodnicy.AsQueryable();

            if (dyscyplina is not null) q = q.Where(z => z.Dyscyplina == dyscyplina.Value);
            if (ranga is not null) q = q.Where(z => z.Ranga == ranga.Value);
            if (minPunkty is not null) q = q.Where(z => z.Punkty >= minPunkty.Value);
            if (maxWiek is not null) q = q.Where(z => z.Wiek <= maxWiek.Value);
            if (klubId is not null) q = q.Where(z => z.KlubId == klubId.Value);


            return q.ToList().AsReadOnly();
        }















    }
}
