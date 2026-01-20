using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;
using system_zawodnicy_zimowi.core.Domain.Exceptions;
using system_zawodnicy_zimowi.core.Services; // Dodano namespace serwisu

namespace system_zawodnicy_zimowi
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Zawodnik> Zawodnicy { get; set; } = new ObservableCollection<Zawodnik>();

        // 1. Instancja serwisu
        private readonly PunktacjaService _punktacjaService = new PunktacjaService();

        public MainWindow()
        {
            InitializeComponent();
            ListaDatagrid.ItemsSource = Zawodnicy;

            // 2. Subskrypcja zdarzenia zmiany rangi
            _punktacjaService.RangaZmieniona += ObslugaZmianyRangi;

            ZaladujDaneTestowe();
        }

        // Metoda wywoływana automatycznie przez serwis, gdy ranga się zmieni
        private void ObslugaZmianyRangi(Zawodnik z, Ranga stara, Ranga nowa)
        {
            MessageBox.Show(
                $"GRATULACJE!\nZawodnik {z.Imie} {z.Nazwisko} awansował!\n\nStara ranga: {stara}\nNOWA RANGA: {nowa}",
                "Awans Zawodnika",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ZaladujDaneTestowe()
        {
            try
            {
                var z1 = new NarciarzAlpejski("Andrzej", "Bachleda", 28);
                // Ustawiamy ręcznie punkty, żeby wyglądało ładnie na starcie, 
                // ale w praktyce powinniśmy dodawać wyniki.
                z1.SetPunktyIRange(2500, Ranga.Junior);

                var z2 = new Snowboardzista("Paulina", "Ligocka", 24);

                Zawodnicy.Add(z1);
                Zawodnicy.Add(z2);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd danych startowych: " + ex.Message);
            }
        }

        // --- DODAWANIE WYNIKU I PRZELICZANIE (NOWOŚĆ) ---
        private void BtnDodajWynik_Click(object sender, RoutedEventArgs e)
        {
            if (ListaDatagrid.SelectedItem is not Zawodnik zawodnik)
            {
                MessageBox.Show("Wybierz zawodnika z listy po lewej stronie, aby dodać mu wynik.");
                return;
            }

            try
            {
                // Pobieranie danych z formularza
                string nazwa = TxtNazwaZawodow.Text;
                DateTime data = DateDataZawodow.SelectedDate ?? DateTime.Now;

                if (!int.TryParse(TxtMiejsce.Text, out int miejsce)) throw new Exception("Podaj poprawne miejsce (liczba).");
                if (!int.TryParse(TxtTrudnosc.Text, out int trudnosc)) throw new Exception("Podaj poprawną trudność (liczba).");
                if (!int.TryParse(TxtPunktyBazowe.Text, out int punktyBazowe)) throw new Exception("Podaj poprawne punkty bazowe (liczba).");

                // Tworzenie obiektu WynikZawodow (korzystamy z Twojej klasy)
                var wynik = new WynikZawodow(data, nazwa, miejsce, trudnosc, punktyBazowe);

                // Dodanie wyniku do listy wyników zawodnika
                zawodnik.DodajWynik(wynik);

                // KLUCZOWY MOMENT: Wywołanie serwisu do przeliczenia punktów
                _punktacjaService.Przelicz(zawodnik);

                // Odświeżenie widoku
                OdswiezListe();

                MessageBox.Show($"Dodano wynik i przeliczono punkty.\nAktualne punkty: {zawodnik.Punkty}");

                // Wyczyszczenie pól
                TxtNazwaZawodow.Clear(); TxtMiejsce.Clear(); TxtTrudnosc.Clear(); TxtPunktyBazowe.Clear();
            }
            catch (DomainValidationException ex)
            {
                MessageBox.Show($"Błąd walidacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- AKTUALIZACJA SZCZEGÓŁÓW (Wspólna metoda) ---
        private void ListaDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AktualizujPanelSzczegolow();
        }

        private void AktualizujPanelSzczegolow()
        {
            if (ListaDatagrid.SelectedItem is Zawodnik wybrany)
            {
                // Panel górny (nazwisko nad formularzem wyników)
                TxtWybranyDoWyniku.Text = $"{wybrany.Imie} {wybrany.Nazwisko}";

                // Panel dolny (stopka)
                TxtImieStopka.Text = $"{wybrany.Imie} {wybrany.Nazwisko}";
                TxtWiekStopka.Text = $"{wybrany.Wiek} lat";
                TxtRangaStopka.Text = wybrany.Ranga.ToString().ToUpper();
                TxtDyscyplinaStopka.Text = wybrany.Dyscyplina.ToString();
                TxtPunktyPasek.Text = $"{wybrany.Punkty} / 20000 pkt"; // 20k to limit Pro w Twoim serwisie

                if (!string.IsNullOrEmpty(wybrany.KlubNazwa))
                    TxtNazwaKlubu.Text = wybrany.KlubNazwa;
                else
                    TxtNazwaKlubu.Clear();

                AnimujPasekPostepu(wybrany.Punkty);
            }
            else
            {
                TxtWybranyDoWyniku.Text = "-- wybierz z listy --";
                TxtImieStopka.Text = "Wybierz zawodnika";
                TxtWiekStopka.Text = "-- lat";
                TxtRangaStopka.Text = "---";
                TxtDyscyplinaStopka.Text = "---";
                TxtPunktyPasek.Text = "0 pkt";
                PasekPostepu.Value = 0;
                TxtNazwaKlubu.Clear();
            }
        }

        private void AnimujPasekPostepu(int punktyDocelowe)
        {
            // Limit wizualny do 20000 (Ranga Pro)
            double targetValue = punktyDocelowe > 20000 ? 20000 : punktyDocelowe;

            DoubleAnimation animation = new DoubleAnimation
            {
                From = PasekPostepu.Value,
                To = targetValue,
                Duration = new Duration(TimeSpan.FromSeconds(0.8)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            PasekPostepu.BeginAnimation(ProgressBar.ValueProperty, animation);
        }

        // --- POZOSTAŁE FUNKCJE (DODAWANIE ZAWODNIKA, KLUBY) ---
        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string imie = TxtImie.Text;
                string nazwisko = TxtNazwisko.Text;
                if (!int.TryParse(TxtWiek.Text, out int wiek)) { MessageBox.Show("Błędny wiek"); return; }

                if (CmbTyp.SelectedItem is not ComboBoxItem typItem) return;
                string typ = typItem.Content?.ToString() ?? "";

                Zawodnik nowy = typ.Contains("Narciarz")
                    ? new NarciarzAlpejski(imie, nazwisko, wiek)
                    : new Snowboardzista(imie, nazwisko, wiek);

                Zawodnicy.Add(nowy);
                TxtImie.Clear(); TxtNazwisko.Clear(); TxtWiek.Clear();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnPrzypiszKlub_Click(object sender, RoutedEventArgs e)
        {
            if (ListaDatagrid.SelectedItem is Zawodnik z)
            {
                try { z.PrzypiszKlub(Guid.NewGuid(), TxtNazwaKlubu.Text); OdswiezListe(); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void BtnWypiszKlub_Click(object sender, RoutedEventArgs e)
        {
            if (ListaDatagrid.SelectedItem is Zawodnik z)
            {
                z.WypiszZKlubu();
                OdswiezListe();
            }
        }

        private void OdswiezListe()
        {
            ListaDatagrid.Items.Refresh();
            AktualizujPanelSzczegolow();
        }
    }
}