using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;

namespace system_zawodnicy_zimowi.core.Services
{
    public class ZawodnikFactory
    {
        public Zawodnik Utworz(Dyscyplina dyscyplina, string imie, string nazwisko, int wiek)
        {
            return dyscyplina switch //super sa te skrucone switche
            {
                Dyscyplina.NarciarstwoAlpejskie => new NarciarzAlpejski(imie, nazwisko, wiek),
                Dyscyplina.Snowboard => new Snowboardzista(imie, nazwisko, wiek),
                _ => throw new ArgumentOutOfRangeException(nameof(dyscyplina), "Nieobsługiwana dyscyplina.")
            };
        }
    }
}
