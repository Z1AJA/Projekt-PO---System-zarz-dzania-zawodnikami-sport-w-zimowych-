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
       
        private WynikZawodow() { }

        public Guid Id { get; internal set; } = Guid.NewGuid();

        public DateTime Data { get; private set; }
        public int Miejsce { get; private set; }

        public Guid RodzajZawodowId { get; private set; }
        public virtual RodzajZawodow RodzajZawodow { get; private set; }

    
        public string NazwaHistoryczna { get; private set; }
        public int TrudnoscHistoryczna { get; private set; }
        public int PunktyBazoweHistoryczne { get; private set; }

        public WynikZawodow(DateTime data, int miejsce, RodzajZawodow rodzaj)
        {
            if (rodzaj == null)
                throw new DomainValidationException("Nie wybrano rodzaju zawodów.");

            SetData(data);
            SetMiejsce(miejsce);

           
            RodzajZawodow = rodzaj;
            RodzajZawodowId = rodzaj.Id;

            
            NazwaHistoryczna = rodzaj.Nazwa;
            TrudnoscHistoryczna = rodzaj.Trudnosc;
            PunktyBazoweHistoryczne = rodzaj.PunktyBazowe;
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

        

        public string NazwaZawodow => NazwaHistoryczna;
        public int TrudnoscTrasy => TrudnoscHistoryczna;
        public int PunktyBazowe => PunktyBazoweHistoryczne;
    }







}

