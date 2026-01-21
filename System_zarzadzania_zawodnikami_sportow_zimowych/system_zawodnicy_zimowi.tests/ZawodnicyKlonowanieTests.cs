using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;
namespace system_zawodnicy_zimowi.tests
{
    public class ZawodnicyKlonowanieTests
    {
        [Fact]
        public void KlonowanieNarciarz()
        {
            var narciarz = new NarciarzAlpejski("Kamil", "Nowak", 30);
            narciarz.SetPunktyIRange(500, Ranga.Amator);
            var kopia = (NarciarzAlpejski)narciarz.Clone();
            Xunit.Assert.NotSame(narciarz, kopia);
            Xunit.Assert.Equal("Kamil", kopia.Imie);
            Xunit.Assert.Equal(500, kopia.Punkty);
        }
        [Fact]
        public void KlonowanieSnowboardzista()
        {
            var snowboardzista = new Snowboardzista("Anna", "Nowak", 30);
            snowboardzista.SetPunktyIRange(500, Ranga.Amator);
            var kopia = (Snowboardzista)snowboardzista.Clone();
            Xunit.Assert.NotSame(snowboardzista, kopia);
            Xunit.Assert.Equal("Anna", kopia.Imie);
            Xunit.Assert.Equal(500, kopia.Punkty);
        }
    }
}
