using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Exceptions;

namespace system_zawodnicy_zimowi.core.Domain.Entities
{
    public class RodzajZawodow
    {
     
        private RodzajZawodow() { }

        public Guid Id { get; private set; } = Guid.NewGuid();

        
        private string _nazwa = "";

        
        public string Nazwa
        {
            get => _nazwa;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DomainValidationException("Nazwa zawodów nie może być pusta.");
                var t = value.Trim();
                if (t.Length < 3 || t.Length > 80)
                    throw new DomainValidationException("Nazwa zawodów musi mieć 3–80 znaków.");
                _nazwa = t;
            }
        }

        public int Trudnosc { get; private set; }
        public int PunktyBazowe { get; private set; }

        
        public RodzajZawodow(string nazwa, int trudnosc, int punktyBazowe)
        {
            Nazwa = nazwa;
            SetTrudnosc(trudnosc);
            SetPunktyBazowe(punktyBazowe);
        }

        
        public void SetTrudnosc(int trudnosc)
        {
            if (trudnosc < 1 || trudnosc > 5)
                throw new DomainValidationException("Trudność trasy musi być w zakresie 1–5.");
            Trudnosc = trudnosc;
        }

        public void SetPunktyBazowe(int punkty)
        {
            if (punkty < 0 || punkty > 10000)
                throw new DomainValidationException("Punkty bazowe muszą być w zakresie 0–10000.");
            PunktyBazowe = punkty;
        }

       
        public override string ToString()
        {
            return $"{Nazwa} (Trudność: {Trudnosc}, Pkt: {PunktyBazowe})";
        }
    }
}
