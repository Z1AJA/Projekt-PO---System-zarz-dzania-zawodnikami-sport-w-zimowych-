using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;

namespace system_zawodnicy_zimowi.core.Services
{
    public class PunktacjaService
    {

        public event Action<Zawodnik, Ranga, Ranga>? RangaZmieniona;





        public void Przelicz(Zawodnik zawodnik)
        {
            if (zawodnik is null) throw new ArgumentNullException(nameof(zawodnik));

            int punkty = ObliczPunkty(zawodnik);
            var nowaRanga = WyznaczRange(punkty);

            var stara = zawodnik.Ranga;
            zawodnik.SetPunktyIRange(punkty, nowaRanga);

            if (stara != nowaRanga)
            {
                RangaZmieniona?.Invoke(zawodnik, stara, nowaRanga);
            }
        }




        public int ObliczPunkty(Zawodnik zawodnik)
        {
            // 10 ostatnich wyników (punktyBazowe + bonus) * trudnosc
            // bonus za miejsce: max(0, 120 - miejsce)
            var last = zawodnik.Wyniki.OrderByDescending(w => w.Data).Take(10).ToList();

            double dyscyplinaFactor = zawodnik.Dyscyplina switch
            {
                Dyscyplina.NarciarstwoAlpejskie => 1.05, // bo narty sa lepsze
                Dyscyplina.Snowboard => 1.00,
                _ => 1.00
            };

            int sum = 0;

            foreach (var w in last)
            {
                int bonus = Math.Max(0, 120 - w.Miejsce);
                int pkt = (w.PunktyBazowe + bonus) * w.TrudnoscTrasy;
                sum += pkt;
            }

            return (int)Math.Round(sum * dyscyplinaFactor, MidpointRounding.AwayFromZero); // zaokraglenie na korzysc zawodnika
        }



        public Ranga WyznaczRange(int punkty)
        {
            if (punkty < 3000) return Ranga.Junior;
            if (punkty < 9000) return Ranga.Amator; // its over 9000
            if (punkty < 20000) return Ranga.SemiPro;
            return Ranga.Pro;
        }



     }
















}

