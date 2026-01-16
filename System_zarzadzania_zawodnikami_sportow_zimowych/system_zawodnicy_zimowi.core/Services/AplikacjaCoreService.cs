using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Entities;

namespace system_zawodnicy_zimowi.core.Services
{
    public class AplikacjaCoreService
    {
        private readonly PunktacjaService _punktacja;
        private readonly PrzydzialKlubuService _przydzial;
        private readonly RankingService _ranking;

        public AplikacjaCoreService()
        {
            _punktacja = new PunktacjaService();
            _przydzial = new PrzydzialKlubuService();
            _ranking = new RankingService();
        }



        
        public event Action<Zawodnik, Domain.Enums.Ranga, Domain.Enums.Ranga>? RangaZmieniona
        {
            add { _punktacja.RangaZmieniona += value; }
            remove { _punktacja.RangaZmieniona -= value; }

        }



        public void DodajWynikIZaktualizuj(Zawodnik zawodnik, WynikZawodow wynik, IEnumerable<KlubSportowy>? kluby = null)
        {
            if (zawodnik is null) throw new ArgumentNullException(nameof(zawodnik));
            if (wynik is null) throw new ArgumentNullException(nameof(wynik));


            zawodnik.DodajWynik(wynik);

            _punktacja.Przelicz(zawodnik);


            if (kluby is not null)
                _przydzial.PrzydzielNajlepszyKlub(zawodnik, kluby);
        }





        public void PrzeliczPunktyIZaktualizuj(Zawodnik zawodnik, IEnumerable<KlubSportowy>? kluby = null)
        {
            if (zawodnik is null) throw new ArgumentNullException(nameof(zawodnik));

            _punktacja.Przelicz(zawodnik);


            if (kluby is not null)
                _przydzial.PrzydzielNajlepszyKlub(zawodnik, kluby);
        }




        public KlubSportowy? PrzydzielKlub(Zawodnik zawodnik, IEnumerable<KlubSportowy> kluby)
        {
            return _przydzial.PrzydzielNajlepszyKlub(zawodnik, kluby);
        }

        public IReadOnlyList<Zawodnik> Ranking(IEnumerable<Zawodnik> zawodnicy)
        {
            return _ranking.SortujPoPunktach(zawodnicy);
        }



        public IReadOnlyList<Zawodnik> FiltrujRanking(
            IEnumerable<Zawodnik> zawodnicy,
            Domain.Enums.Dyscyplina? dyscyplina = null,
            Domain.Enums.Ranga? ranga = null,
            int? minPunkty = null,
            int? maxWiek = null,
            Guid? klubId = null)
        {
            return _ranking.Filtruj(zawodnicy, dyscyplina, ranga, minPunkty, maxWiek, klubId);
        }















    }
}
