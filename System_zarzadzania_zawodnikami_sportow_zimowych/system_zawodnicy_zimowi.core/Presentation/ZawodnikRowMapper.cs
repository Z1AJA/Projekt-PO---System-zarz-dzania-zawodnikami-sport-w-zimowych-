using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using system_zawodnicy_zimowi.core.Domain.Entities;




namespace system_zawodnicy_zimowi.core.Presentation
{
    public static class ZawodnikRowMapper
    {

        public static ZawodnikRowDto ToRow(Zawodnik z) => new()
        {
            Id = z.Id,
            Imie = z.Imie,
            Nazwisko = z.Nazwisko,
            Wiek = z.Wiek,
            Dyscyplina = z.Dyscyplina,
            Punkty = z.Punkty,
            Ranga = z.Ranga,
            KlubId = z.KlubId,
            KlubNazwa = z.KlubNazwa
        };



        public static List<ZawodnikRowDto> ToRows(IEnumerable<Zawodnik> zawodnicy) => zawodnicy.Select(ToRow).ToList();
    }
}
