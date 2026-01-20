using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;
using system_zawodnicy_zimowi.core.Domain.Exceptions;

namespace system_zawodnicy_zimowi
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Zawodnik> Zawodnicy { get; set; } = new ObservableCollection<Zawodnik>();

        public MainWindow()
        {
            InitializeComponent();
            ListaDatagrid.ItemsSource = Zawodnicy;

            ZaladujDaneTestowe();
        }

        private void ZaladujDaneTestowe()
        {
            try
            {
                // POPRAWKA: Używamy Ranga.Junior wszędzie, ponieważ Senior/Mistrz 
                // prawdopodobnie nie istnieją w Twoim pliku Enum.
                var z1 = new NarciarzAlpejski("Andrzej", "Bachleda", 28);
                z1.SetPunktyIRange(850, Ranga.Junior);
                z1.PrzypiszKlub(Guid.NewGuid(), "Tatry Ski Team");

                var z2 = new Snowboardzista("Paulina", "Ligocka", 24);
                z2.SetPunktyIRange(420, Ranga.Junior);

                var z3 = new NarciarzAlpejski("Kamil", "Stoch", 35);
                z3.SetPunktyIRange(980, Ranga.Junior);
                z3.PrzypiszKlub(Guid.NewGuid(), "WKS Zakopane");

                Zawodnicy.Add(z1);
                Zawodnicy.Add(z2);
                Zawodnicy.Add(z3);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd ładowania danych: " + ex.Message);
            }
        }

        // --- OBSŁUGA SELEKCJI I ANIMACJA PASKA ---
        private void ListaDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AktualizujPanelSzczegolow();
        }

        // Wydzielona metoda do aktualizacji panelu (rozwiązuje problem null przy odświeżaniu)
        private void AktualizujPanelSzczegolow()
        {
            if (ListaDatagrid.SelectedItem is Zawodnik wybrany)
            {
                // Aktualizacja tekstów
                TxtWybranyInfo.Text = $"{wybrany.Imie} {wybrany.Nazwisko}";
                TxtImieStopka.Text = $"{wybrany.Imie} {wybrany.Nazwisko}";
                TxtDyscyplinaStopka.Text = wybrany.Dyscyplina.ToString();
                TxtPunktyPasek.Text = $"{wybrany.Punkty} / 1000 pkt";

                // Wypełnienie pola klubu nazwą
                if (!string.IsNullOrEmpty(wybrany.KlubNazwa))
                    TxtNazwaKlubu.Text = wybrany.KlubNazwa;
                else
                    TxtNazwaKlubu.Clear();

                // Animacja Progress Bara
                AnimujPasekPostepu(wybrany.Punkty);
            }
            else
            {
                // Resetowanie widoku gdy nic nie jest wybrane
                TxtWybranyInfo.Text = "-- brak --";
                TxtImieStopka.Text = "Wybierz zawodnika";
                TxtDyscyplinaStopka.Text = "---";
                TxtPunktyPasek.Text = "0 pkt";
                PasekPostepu.Value = 0;
                TxtNazwaKlubu.Clear();
            }
        }

        private void AnimujPasekPostepu(int punktyDocelowe)
        {
            double targetValue = punktyDocelowe > 1000 ? 1000 : punktyDocelowe;

            DoubleAnimation animation = new DoubleAnimation
            {
                From = PasekPostepu.Value,
                To = targetValue,
                Duration = new Duration(TimeSpan.FromSeconds(0.6)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            PasekPostepu.BeginAnimation(ProgressBar.ValueProperty, animation);
        }

        // --- DODAWANIE ZAWODNIKA ---
        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string imie = TxtImie.Text;
                string nazwisko = TxtNazwisko.Text;

                if (!int.TryParse(TxtWiek.Text, out int wiek))
                {
                    MessageBox.Show("Podaj poprawny wiek (liczba).");
                    return;
                }

                // Bezpieczne sprawdzanie ComboBoxa (eliminuje ostrzeżenie o null)
                if (CmbTyp.SelectedItem is not ComboBoxItem typItem)
                {
                    MessageBox.Show("Wybierz typ zawodnika.");
                    return;
                }

                string typ = typItem.Content?.ToString() ?? "";

                Zawodnik nowy;
                if (typ.Contains("Narciarz"))
                    nowy = new NarciarzAlpejski(imie, nazwisko, wiek);
                else
                    nowy = new Snowboardzista(imie, nazwisko, wiek);

                // Ustawiamy domyślne punkty i rangę Junior (bo tylko Junior istnieje w Twoim Enumie)
                var rand = new Random();
                nowy.SetPunktyIRange(rand.Next(100, 600), Ranga.Junior);

                Zawodnicy.Add(nowy);
                WyczyscFormularz();
            }
            catch (DomainValidationException ex)
            {
                MessageBox.Show($"Błąd walidacji: {ex.Message}", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- OBSŁUGA KLUBU ---
        private void BtnPrzypiszKlub_Click(object sender, RoutedEventArgs e)
        {
            if (ListaDatagrid.SelectedItem is Zawodnik z)
            {
                try
                {
                    string nazwaKlubu = TxtNazwaKlubu.Text;
                    // Generujemy nowe ID dla klubu
                    z.PrzypiszKlub(Guid.NewGuid(), nazwaKlubu);

                    OdswiezListe();
                    MessageBox.Show("Przypisano do klubu!");
                }
                catch (DomainValidationException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Wybierz zawodnika z listy.");
            }
        }

        private void BtnWypiszKlub_Click(object sender, RoutedEventArgs e)
        {
            if (ListaDatagrid.SelectedItem is Zawodnik z)
            {
                z.WypiszZKlubu();
                OdswiezListe();
                MessageBox.Show("Wypisano z klubu.");
            }
            else
            {
                MessageBox.Show("Wybierz zawodnika z listy.");
            }
        }

        // Pomocnicze
        private void WyczyscFormularz()
        {
            TxtImie.Clear(); TxtNazwisko.Clear(); TxtWiek.Clear();
        }

        private void OdswiezListe()
        {
            // Odświeżenie widoku tabeli
            ListaDatagrid.Items.Refresh();

            // POPRAWKA: Zamiast wywoływać zdarzenie z "null", wywołujemy bezpieczną metodę
            AktualizujPanelSzczegolow();
        }
    }
}