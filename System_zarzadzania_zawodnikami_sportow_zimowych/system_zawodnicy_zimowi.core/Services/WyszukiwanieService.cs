using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using system_zawodnicy_zimowi.core.Domain.Entities;

namespace system_zawodnicy_zimowi.core.Services
{
    public class WyszukiwanieService
    {
        public IReadOnlyList<Zawodnik> SzukajPoImieniuLubNazwisku(IEnumerable<Zawodnik> zawodnicy, string? fraza)
        {
            if (zawodnicy is null) throw new ArgumentNullException(nameof(zawodnicy));



            if (string.IsNullOrWhiteSpace(fraza))
                return zawodnicy.ToList().AsReadOnly();

            var f = fraza.Trim().ToLowerInvariant();







            return zawodnicy
                .Where(z =>
                    z.Imie.ToLowerInvariant().Contains(f) ||
                    z.Nazwisko.ToLowerInvariant().Contains(f))
                .ToList()
                .AsReadOnly();
        }
    }
}
