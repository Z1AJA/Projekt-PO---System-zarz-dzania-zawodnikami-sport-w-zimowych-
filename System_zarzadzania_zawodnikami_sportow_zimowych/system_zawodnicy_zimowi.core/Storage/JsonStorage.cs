using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;
using system_zawodnicy_zimowi.core.Storage.Dto;

namespace system_zawodnicy_zimowi.core.Storage
{
    public class JsonStorage
    {

        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true
        };

        public void Zapisz(string path, IEnumerable<Zawodnik> zawodnicy, IEnumerable<KlubSportowy> kluby)
        {
            var state = new AppStateDto
            {
                Zawodnicy = zawodnicy.Select(ToDto).ToList(),
                Kluby = kluby.Select(ToDto).ToList()
            };

            var json = JsonSerializer.Serialize(state, Options);
            File.WriteAllText(path, json);
        }

        public AppStateDto WczytajDto(string path)
        {
            var json = File.ReadAllText(path);
            var state = JsonSerializer.Deserialize<AppStateDto>(json, Options);
            return state ?? new AppStateDto();
        }


        // GUI/DATA mozecie użyć DTO albo  z odtworzenia obiektów core:
        public (List<Zawodnik> zawodnicy, List<KlubSportowy> kluby) Wczytaj(string path)
        {
            var dto = WczytajDto(path);

            var kluby = dto.Kluby.Select(FromDto).ToList();
            var zawodnicy = dto.Zawodnicy.Select(FromDto).ToList();

            return (zawodnicy, kluby);
        }

        private static KlubDto ToDto(KlubSportowy k) => new()
        {
            Id = k.Id,
            Nazwa = k.Nazwa,
            MinimalnePunkty = k.MinimalnePunkty,
            MaksWiek = k.MaksWiek,
            LimitMiejsc = k.LimitMiejsc,
            Dyscypliny = new List<Dyscyplina>(k.Dyscypliny)
        };


        private static ZawodnikDto ToDto(Zawodnik z) => new()
        {
            Id = z.Id,
            Imie = z.Imie,
            Nazwisko = z.Nazwisko,
            Wiek = z.Wiek,
            Dyscyplina = z.Dyscyplina,
            Punkty = z.Punkty,
            Ranga = z.Ranga,
            KlubId = z.KlubId,
            KlubNazwa = z.KlubNazwa,
            Wyniki = z.Wyniki.Select(w => new WynikDto
            {
                Id = w.Id,
                Data = w.Data,
                NazwaZawodow = w.NazwaZawodow,
                Miejsce = w.Miejsce,
                TrudnoscTrasy = w.TrudnoscTrasy,
                PunktyBazowe = w.PunktyBazowe
            }).ToList()
        };


        private static KlubSportowy FromDto(KlubDto k)
        {
            var klub = new KlubSportowy(k.Nazwa, k.MinimalnePunkty, k.MaksWiek, k.Dyscypliny, k.LimitMiejsc);
            klub.Id = k.Id;
            return klub;
        }


        private static Zawodnik FromDto(ZawodnikDto z)
        {
            Zawodnik zawodnik = z.Dyscyplina switch
            {
                Dyscyplina.NarciarstwoAlpejskie => new NarciarzAlpejski(z.Imie, z.Nazwisko, z.Wiek),
                Dyscyplina.Snowboard => new Snowboardzista(z.Imie, z.Nazwisko, z.Wiek),
                _ => new Snowboardzista(z.Imie, z.Nazwisko, z.Wiek)
            };

            zawodnik.Id = z.Id;

            // Punkty i ranga
            zawodnik.SetPunktyIRange(z.Punkty, z.Ranga);

            // klub
            if (z.KlubId is not null && !string.IsNullOrWhiteSpace(z.KlubNazwa))
                zawodnik.PrzypiszKlub(z.KlubId.Value, z.KlubNazwa!);

            // wwyniki
            foreach (var w in z.Wyniki)
            {
                var wynik = new WynikZawodow(w.Data, w.NazwaZawodow, w.Miejsce, w.TrudnoscTrasy, w.PunktyBazowe);
                wynik.Id = w.Id;
                zawodnik.DodajWynik(wynik);
            }

            return zawodnik;
        }
































    }
}
