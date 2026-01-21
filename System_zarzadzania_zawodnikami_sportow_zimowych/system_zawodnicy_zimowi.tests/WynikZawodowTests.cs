using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Exceptions;


namespace system_zawodnicy_zimowi.tests
{
    public class WynikZawodowTests
    {
        [Fact]
        public void Konstruktor_Snapshot()
        {
            var data = DateTime.Now.AddDays(-1);
            var szablon = new RodzajZawodow("Puchar Swiata", 5, 1000);
            var wynik = new WynikZawodow(data, 1, szablon);
            szablon.Nazwa = "Nazwa Zmieniona";
            szablon.PunktyBazowe = 0;
            Xunit.Assert.Equal("Puchar Swiata", wynik.NazwaHistoryczna);
            Xunit.Assert.Equal(1000, wynik.PunktyBazoweHistoryczne);
        }
        [Fact]
        public void Konstruktor_Rodzaj()
        {
            var ex = Xunit.Assert.Throws<DomainValidationException>(() => new WynikZawodow(DateTime.Now, 1, null));
            Xunit.Assert.Equal("Nie wybrano rodzaju zawodów.", ex.Message);
        }
        [Fact]
        public void DataPrzyszlosc()
        {
            var szablon = new RodzajZawodow("Test", 1, 100);
            var dataPrzyszlosc = DateTime.Now.AddDays(1);
            var ex = Xunit.Assert.Throws<DomainValidationException>(() => new WynikZawodow(dataPrzyszlosc, 1, szablon));
            Xunit.Assert.Equal("Data zawodów nie może być w przyszłości.", ex.Message);

        }
        [Theory]
        [InlineData(0)]
        [InlineData(301)]
        public void MiejsceZakres(int bledne)
        {
            var szablon = new RodzajZawodow("Test", 1, 100);
            var ex = Xunit.Assert.Throws<DomainValidationException>(() => new WynikZawodow(DateTime.Now, bledne, szablon));
            Xunit.Assert.Equal("Miejsce musi być w zakresie 1–300.", ex.Message);
        }
    }
}
