using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Enums;
using system_zawodnicy_zimowi.core.Domain.Exceptions;

namespace system_zawodnicy_zimowi.core.Domain.Entities
{
    public class WynikZawodow
    {
        private string _nazwaZawodow = "";

        public Guid Id { get; private set; } = Guid.NewGuid();

        public DateTime Data { get; private set; }
        public string NazwaZawodow
        {
            get => _nazwaZawodow; 
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DomainValidationException("Nazwa zawodów nie może być pusta.");
                var t = value.Trim();
                if (t.Length < 3 || t.Length > 80)
                    throw new DomainValidationException("Nazwa zawodów musi mieć 3–80 znaków.");
                _nazwaZawodow = t;
            }
        }

        public int Miejsce { get; private set; }

        public int TrudnoscTrasy { get; private set; }

        public int PunktyBazowe { get; private set; }

        public WynikZawodow(DateTime data, string nazwaZawodow, int miejsce, int trudnoscTrasy, int punktyBazowe)
        {
            SetData(data);
            NazwaZawodow = nazwaZawodow;
            SetMiejsce(miejsce);
            SetTrudnosc(trudnoscTrasy);
            SetPunktyBazowe(punktyBazowe);
        }

        public void SetData(DateTime data)
        {
            if (data > DateTime.Now)
                throw new DomainValidationException("Data zawodów nie może być w przyszłości.");
            Data = data;
        }

        public void SetMiejsce(int miejsce)
        {
            if (miejsce < 1 || miejsce > 300)
                throw new DomainValidationException("Miejsce musi być w zakresie 1–300.");
            Miejsce = miejsce;
        }

        public void SetTrudnosc(int trudnosc)
        {
            if (trudnosc < 1 || trudnosc > 5)
                throw new DomainValidationException("Trudność trasy musi być w zakresie 1–5.");
            TrudnoscTrasy = trudnosc;
        }

        public void SetPunktyBazowe(int punktyBazowe)
        {
            if (punktyBazowe < 0 || punktyBazowe > 10000)
                throw new DomainValidationException("Punkty bazowe muszą być w zakresie 0–10000.");
            PunktyBazowe = punktyBazowe;
        }








    }
}
