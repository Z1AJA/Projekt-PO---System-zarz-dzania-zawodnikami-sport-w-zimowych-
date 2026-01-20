using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Enums;

namespace system_zawodnicy_zimowi.core.Domain.Entities
{
    public class NarciarzAlpejski : Zawodnik
    {
        public NarciarzAlpejski(string imie, string nazwisko, int wiek) : base(imie, nazwisko, wiek, Dyscyplina.NarciarstwoAlpejskie) { }

        public override object Clone()
        {
            var copy = new NarciarzAlpejski(Imie, Nazwisko, Wiek);

            copy.SetPunktyIRange(Punkty, Ranga);
            return copy;
        }
    }
}
