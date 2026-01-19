using System;
using System.Collections.Generic;
using System.Linq;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;
using system_zawodnicy_zimowi.core.Domain.Exceptions;

namespace system_zawodnicy_zimowi.ConsoleAPP
{
    class Program
    {
        static void Main(string[] args)
        {
            var kluby = KreatorKlubow();


            var zawodnicy = KreatorZawodnikow();

            // 3. SYMULACJA DODAWANIA WYNIKÓW
            Console.WriteLine("Dodawanie Zawodow");
            var z = zawodnicy[0];
            z.DodajWynik(new WynikZawodow(DateTime.Now, "Finał Świata", 1, 5, 1000));
            z.DodajWynik(new WynikZawodow(DateTime.Now.AddMonths(-2), "Zawody Lokalne", 10, 2, 100));

            Console.WriteLine($"Wyniki zawodnika {z.Imie} (od najstarszych):");
            foreach (var w in z.PobierzWynikiChronologicznie())
                Console.WriteLine($" - {w.Data.ToShortDateString()}: {w.NazwaZawodow} Miejsce: {w.Miejsce}, Punkty: {w.PunktyBazowe}");

            // Sprawdzanie Klubu
            Console.WriteLine("\n Sprawdzanie Klubu");
            foreach (var k in kluby)
            {
                Console.WriteLine($"\nKlub: {k.Nazwa} (Min Pkt: {k.MinimalnePunkty}, Max Wiek: {k.MaksWiek ?? 0})");
                foreach (var zawodnik in zawodnicy)
                {
                    bool pasuje = k.PasujeDo(zawodnik);
                    Console.WriteLine($" > {zawodnik.Imie} {zawodnik.Nazwisko} ({zawodnik.Dyscyplina}, {zawodnik.Punkty} pkt, {zawodnik.Wiek} l.): {(pasuje ? "ZAAKCEPTOWANY" : "ODRZUCONY")}");

                    if (pasuje && zawodnik.KlubId == null)
                        zawodnik.PrzypiszKlub(k.Id, k.Nazwa); //
                }
            }

            // Sortowanie
            Console.WriteLine("\n Sortowanie");
            zawodnicy.Sort(); // Wywołuje CompareTo z klasy Zawodnik
            foreach (var j in zawodnicy)
            {
                Console.WriteLine($"{j.Punkty} pkt | {j.Nazwisko} {j.Imie} | Klub: {j.KlubNazwa ?? "Brak"}");


            }

            Console.WriteLine("\n Equatable");
            var porownanie1 = zawodnicy[0].Equals(zawodnicy[1]);
            Console.WriteLine($"Czy {zawodnicy[0].Imie} == {zawodnicy[1].Imie}? {porownanie1}");
            var tenSam = zawodnicy[0];
            Console.WriteLine($"Czy zawodnik jest równy samemu sobie? {zawodnicy[0].Equals(tenSam)}");


            // Klonowanie
            var oryginal = zawodnicy[0];
            var klon = (Zawodnik)oryginal.Clone(); // Wywołuje Clone z klasy pochodnej
            Console.WriteLine($"Oryginał: {oryginal.Imie} {oryginal.Nazwisko}, ID: {oryginal.Id}");
            Console.WriteLine($"Klon:      {klon.Imie} {klon.Nazwisko}, ID: {klon.Id}");
            Console.WriteLine($"Czy to ten sam obiekt w pamięci? {ReferenceEquals(oryginal, klon)}");
            Console.WriteLine($"Czy Equals (IEquatable) po klonowaniu jest False? {oryginal.Equals(klon)} (bo Id jest nowe)");

            // Walidajca
            Console.WriteLine("\n--- Testy odporności na błędy (Walidacja) ---");
            TestujBlad("Zbyt młody zawodnik", () => new NarciarzAlpejski("Mały", "Gucio", 3));
            TestujBlad("Puste nazwisko", () => new Snowboardzista("Jan", "", 20));
            TestujBlad("Ujemne punkty", () => zawodnicy[0].SetPunktyIRange(-10, Ranga.Amator));
            TestujBlad("Klub bez dyscyplin", () => new KlubSportowy("Pusty", 0, null, new List<Dyscyplina>()));


            Console.ReadKey();
        }

        static void TestujBlad(string opis, Action akcja)
        {
            try
            {
                akcja();
                Console.WriteLine($"[FAIL] {opis} - nie rzucono wyjątku!");
            }
            catch (DomainValidationException ex)
            {
                Console.WriteLine($"[OK] {opis} - Błąd: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] {opis} - Inny błąd: {ex.GetType().Name}: {ex.Message}");
            }
        }
        static List<KlubSportowy> KreatorKlubow()
        {
            var lista = new List<KlubSportowy>();
            while (true)
            {

                try
                {
                    Console.Write("Nazwa klubu: ");
                    string nazwa = Console.ReadLine();

                    Console.Write("Wymagane punkty (np. 1000): ");
                    int punkty = int.Parse(Console.ReadLine() ?? "0");

                    Console.Write("Maksymalny wiek (Wciśnij ENTER jeśli brak limitu): ");
                    string wiekStr = Console.ReadLine();
                    int? maxWiek = string.IsNullOrWhiteSpace(wiekStr) ? null : int.Parse(wiekStr);


                    Console.WriteLine("Dyscypliny (wpisz numery po przecinku: 1-Narciarstwo, 2-Snowboard): ");
                    string dyscyplinyInput = Console.ReadLine();
                    var dyscypliny = new List<Dyscyplina>();

                    if (dyscyplinyInput.Contains("1")) dyscypliny.Add(Dyscyplina.NarciarstwoAlpejskie);
                    if (dyscyplinyInput.Contains("2")) dyscypliny.Add(Dyscyplina.Snowboard);


                    var klub = new KlubSportowy(nazwa, punkty, maxWiek, dyscypliny);
                    lista.Add(klub);
                    Console.WriteLine("--> Dodano klub!");
                }
                catch (DomainValidationException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"BŁĄD WALIDACJI: {ex.Message}"); //
                    Console.ResetColor();
                    continue;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Błąd: Wpisz poprawną liczbę!");
                    continue;
                }

                Console.Write("Dodać kolejny klub? (t/n): ");
                if (Console.ReadLine().ToLower() != "t") break;
            }
            return lista;
        }

        static List<Zawodnik> KreatorZawodnikow()
        {
            var lista = new List<Zawodnik>();
            while (true)
            {
                Console.WriteLine("\n--- KREATOR ZAWODNIKA ---");
                try
                {
                    Console.WriteLine("Wybierz typ: 1 - Narciarz, 2 - Snowboardzista");
                    string typ = Console.ReadLine();

                    Console.Write("Imię: ");
                    string imie = Console.ReadLine();

                    Console.Write("Nazwisko: ");
                    string nazwisko = Console.ReadLine();

                    Console.Write("Wiek: ");
                    int wiek = int.Parse(Console.ReadLine() ?? "0");


                    Zawodnik nowy;
                    if (typ == "1")
                        nowy = new NarciarzAlpejski(imie, nazwisko, wiek);
                    else
                        nowy = new Snowboardzista(imie, nazwisko, wiek);


                    Console.Write("Aktualne punkty (np. 5000): ");
                    int pkt = int.Parse(Console.ReadLine() ?? "0");


                    Ranga ranga = Ranga.Amator;
                    if (pkt > 8000) ranga = Ranga.Pro;
                    else if (pkt > 4000) ranga = Ranga.SemiPro;
                    else if (pkt < 200) ranga = Ranga.Junior;

                    nowy.SetPunktyIRange(pkt, ranga);

                    lista.Add(nowy);
                    Console.WriteLine("--> Dodano zawodnika!");
                }
                catch (DomainValidationException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"BŁĄD DANYCH: {ex.Message}");
                    Console.ResetColor();
                    continue;
                }
                catch (Exception)
                {
                    Console.WriteLine("Błąd wprowadzania danych. Spróbuj ponownie.");
                    continue;
                }

                Console.Write("Dodać kolejnego zawodnika? (t/n): ");
                if (Console.ReadLine().ToLower() != "t") break;
            }
            return lista;
        }
    }
}