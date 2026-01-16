using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Enums;

namespace system_zawodnicy_zimowi.core.Presentation
{
    public class ZawodnikRowDto
    {
        public Guid Id { get; set; }
        public string Imie { get; set; } = "";
        public string Nazwisko { get; set; } = "";
        public int Wiek { get; set; }
        public Dyscyplina Dyscyplina { get; set; }


        public int Punkty { get; set; }
        public Ranga Ranga { get; set; }



        public Guid? KlubId { get; set; }
        public string? KlubNazwa { get; set; }
    }
}
