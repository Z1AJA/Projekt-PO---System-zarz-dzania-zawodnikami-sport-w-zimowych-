using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Enums;
using system_zawodnicy_zimowi.core.Domain.Exceptions;

namespace system_zawodnicy_zimowi.core.Domain.Entities
{
    public class KlubSportowy
    {
        private string _nazwa = "";

        public Guid Id { get; internal set; } = Guid.NewGuid();
        public string Nazwa { get => _nazwa;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DomainValidationException("Nazwa klubu nie może być pusta.");
                var t = value.Trim();
                if (t.Length < 3 || t.Length > 80)
                    throw new DomainValidationException("Nazwa klubu musi mieć 3–80 znaków.");
                _nazwa = t;
            }
        
        }

        public int MinimalnePunkty { get; private set; }
        
        // jak dasz null nie bedzie limitu
        public int? MaksWiek { get; private set; }

        // Lista dyscyplin przyjmowanych przez klub
        public List<Dyscyplina> Dyscypliny { get; private set; } = new();

        public int? LimitMiejsc { get; private set; }

        public KlubSportowy(string nazwa, int minimalnePunkty, int? maksWiek, IEnumerable<Dyscyplina> dyscypliny, int? limitMiejsc = null)
        {
            Nazwa = nazwa;
            SetMinimalnePunkty(minimalnePunkty);
            SetMaksWiek(maksWiek);
            SetLimitMiejsc(limitMiejsc);
            SetDyscypliny(dyscypliny);
        }

        public void SetMinimalnePunkty(int punkty)
        {
            if (punkty < 0 || punkty > 1000000)
                throw new DomainValidationException("Minimalne punkty muszą być w zakresie 0–1 000 000");
            MinimalnePunkty = punkty;
        }

        public void SetMaksWiek(int? maksWiek)
        {
            if (maksWiek is null) { MaksWiek = null; return; }
            if (maksWiek < 6 || maksWiek > 70)
                throw new DomainValidationException("Maksymalny wiek musi być w zakresie 6–70 albo null.");
            MaksWiek = maksWiek;
        }

        public void SetLimitMiejsc(int? limit)
        {
            if (limit is null) { LimitMiejsc = null; return; }
            if (limit < 1 || limit > 10000)
                throw new DomainValidationException("Limit miejsc musi być w zakresie 1–10 000 albo null.");
            LimitMiejsc = limit;
        }

        public void SetDyscypliny(IEnumerable<Dyscyplina> dyscypliny)
        {
            if (dyscypliny is null) throw new DomainValidationException("Dyscypliny nie mogą być null.");

            var list = new List<Dyscyplina>(dyscypliny);
            if (list.Count == 0) throw new DomainValidationException("Klub musi przyjmować co najmniej jedną dyscyplinę.");

            Dyscypliny = list;
        }

        public bool PasujeDo(Zawodnik zawodnik)
        {
            if (zawodnik is null) 
                return false;

            if (zawodnik.Punkty < MinimalnePunkty) 
                return false;
            if (MaksWiek is not null && zawodnik.Wiek > MaksWiek.Value) 
                return false;
            if (!Dyscypliny.Contains(zawodnik.Dyscyplina)) 
                return false;

            return true;
        }


    }
}
