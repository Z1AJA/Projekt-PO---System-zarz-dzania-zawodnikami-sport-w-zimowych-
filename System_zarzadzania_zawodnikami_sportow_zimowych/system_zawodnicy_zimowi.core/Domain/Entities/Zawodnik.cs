using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Enums;
using system_zawodnicy_zimowi.core.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;


namespace system_zawodnicy_zimowi.core.Domain.Entities
{
    public abstract class Zawodnik : IEquatable<Zawodnik>, IComparable<Zawodnik>, ICloneable
    {
        protected Zawodnik() { }
        private string _imie = "";
        private string _nazwisko = "";

        public Guid Id { get; internal set; } = Guid.NewGuid();
        public string Imie { get => _imie; protected set
            {
                ValidateName(value, nameof(Imie));
                _imie = value.Trim();
            }
        }
        public string Nazwisko { get => _nazwisko;
            protected set
            {
                ValidateName(value, nameof(Nazwisko));
                _nazwisko = value.Trim();
            }
        }

        public int Wiek { get; protected set; }
        public Dyscyplina Dyscyplina { get; protected set; }
        public int Punkty { get; protected set; }
        public Ranga Ranga { get; protected set; } = Ranga.Junior;


        protected Zawodnik(string imie, string nazwisko, int wiek, Dyscyplina dyscyplina)
        {
            Imie = imie;
            Nazwisko = nazwisko;
            SetWiek(wiek);
            Dyscyplina = dyscyplina;
        }

        public void SetWiek(int wiek)
        {
            if (wiek < 6 || wiek > 70)
                throw new DomainValidationException("Wiek musi być w zakresie 6–70.");
            Wiek = wiek;
        }

        public void SetPunktyIRange(int punkty, Ranga ranga)
        {
            if (punkty < 0) throw new DomainValidationException("Punkty nie mogą być ujemne.");
            Punkty = punkty;
            Ranga = ranga;
        }

        public int CompareTo(Zawodnik? other)
        {
            if (other is null) return -1;

            int cmp = other.Punkty.CompareTo(Punkty); // malejąco
            if (cmp != 0) return cmp;

            cmp = string.Compare(Nazwisko, other.Nazwisko, StringComparison.OrdinalIgnoreCase);
            if (cmp != 0) return cmp;

            return string.Compare(Imie, other.Imie, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(Zawodnik? other)
        {
            if (other is null) return false;
            return Id == other.Id;
        }

        public override bool Equals(object? obj) => Equals(obj as Zawodnik);

        public override int GetHashCode() => Id.GetHashCode();

        public abstract object Clone();

        protected static void ValidateName(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainValidationException($"{fieldName} nie może być puste.");

            string trimmed = value.Trim();
            if (trimmed.Length < 2 || trimmed.Length > 40)
                throw new DomainValidationException($"{fieldName} musi mieć 2–40 znaków.");
        }

        private readonly List<WynikZawodow> _wyniki = new();

        public IReadOnlyList<WynikZawodow> Wyniki => _wyniki.AsReadOnly();

        public void DodajWynik(WynikZawodow wynik)
        {
            if (wynik is null) throw new DomainValidationException("Wynik nie może być null.");
            _wyniki.Add(wynik);
        }

        public bool UsunWynik(Guid wynikId)
        {
            var w = _wyniki.FirstOrDefault(x => x.Id == wynikId);
            if(w is null) return false;
            _wyniki.Remove(w);
            return true;
        }

        public IReadOnlyList<WynikZawodow> PobierzWynikiChronologicznie()
        {
            return _wyniki.OrderBy(x => x.Data).ToList().AsReadOnly();
        }


        public Guid? KlubId { get; private set; }
        public string? KlubNazwa { get; private set; }


        public void PrzypiszKlub(Guid klubId, string klubNazwa)
        {
            if (klubId == Guid.Empty)
                throw new DomainValidationException("KlubId nie może być pusty.");

            if (string.IsNullOrWhiteSpace(klubNazwa))
                throw new DomainValidationException("Nazwa klubu nie może być pusta.");

            KlubId = klubId;
            KlubNazwa = klubNazwa.Trim();
        }

        public void WypiszZKlubu()
        {
            KlubId = null;
            KlubNazwa = null;
        }
















    }
}
