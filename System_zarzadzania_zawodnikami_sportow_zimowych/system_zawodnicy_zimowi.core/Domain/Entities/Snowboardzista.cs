using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Enums;

namespace system_zawodnicy_zimowi.core.Domain.Entities
{
    public class Snowboardzista : Zawodnik
    {
        public Snowboardzista(string imie, string nazwisko, int wiek) : base(imie, nazwisko, wiek, Dyscyplina.Snowboard) { }

        public override object Clone()
        {
            var copy = new Snowboardzista(Imie, Nazwisko, Wiek);
            copy.SetPunktyIRange(Punkty, Ranga);
            return copy;
        }
    }
}