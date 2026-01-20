using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;
using system_zawodnicy_zimowi.core.Services;
using system_zawodnicy_zimowi.Data;

namespace system_zawodnicy_zimowi
{
    public partial class MainWindow : Window
    {
        // Serwis punktacji (logika biznesowa)
        private readonly PunktacjaService _punktacjaService = new PunktacjaService();

        // Kolekcje podpięte pod UI
        public ObservableCollection<Zawodnik> Zawodnicy { get; set; } = new ObservableCollection<Zawodnik>();
        public ObservableCollection<KlubSportowy> Kluby { get; set; } = new ObservableCollection<KlubSportowy>();
        public ObservableCollection<WynikZawodow> BazaZawodow { get; set; } = new ObservableCollection<WynikZawodow>();

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // Inicjalizacja bazy przy starcie
                using (var context = new AppDbContext())
                {
                    context.Database.EnsureCreated();
                }

                // Załaduj wszystko
                OdswiezWszystko();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd startu: " + ex.Message);
            }

            // Przypisanie do GUI
            ListaDatagrid.ItemsSource = Zawodnicy;
            GridKluby.ItemsSource = Kluby;
            CmbKlubyWybior.ItemsSource = Kluby;
            GridZawody.ItemsSource = BazaZawodow;
            CmbZawodyWybor.ItemsSource = BazaZawodow;

            _punktacjaService.RangaZmieniona += (z, s, n) =>
                MessageBox.Show($"AWANS!\n{z.Imie} {z.Nazwisko}: {s} -> {n}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Ta metoda pobiera świeże dane z bazy i wrzuca do GUI
        private void OdswiezWszystko()
        {
            using (var context = new AppDbContext())
            {
                // 1. Zawodnicy
                var listaZ = context.Zawodnicy.Include(z => z.Wyniki).ToList();
                Zawodnicy.Clear();
                foreach (var z in listaZ) Zawodnicy.Add(z);

                // 2. Kluby
                var listaK = context.Kluby.ToList();
                Kluby.Clear();
                foreach (var k in listaK) Kluby.Add(k);

                // 3. Szablony
                var wszystkieWyniki = context.Wyniki.ToList();
                // Pobieramy ID wyników przypisanych do graczy
                var idsWynikowGraczy = listaZ.SelectMany(z => z.Wyniki).Select(w => w.Id).ToHashSet();
                // Szablony to te, które NIE są u graczy
                var szablony = wszystkieWyniki.Where(w => !idsWynikowGraczy.Contains(w.Id)).ToList();

                BazaZawodow.Clear();
                foreach (var s in szablony) BazaZawodow.Add(s);
            }

            ListaDatagrid.Items.Refresh();
        }

        // --- ZAWODNICY ---

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(TxtWiek.Text, out int wiek)) { MessageBox.Show("Zły wiek"); return; }
                if (CmbTyp.SelectedItem is not ComboBoxItem item) return;

                string typ = item.Content.ToString() ?? "";

                Zawodnik z = typ.Contains("Narciarz")
                    ? new NarciarzAlpejski(TxtImie.Text, TxtNazwisko.Text, wiek)
                    : new Snowboardzista(TxtImie.Text, TxtNazwisko.Text, wiek);

                using (var context = new AppDbContext())
                {
                    context.Zawodnicy.Add(z);
                    context.SaveChanges();
                }

                OdswiezWszystko();
                TxtImie.Clear(); TxtNazwisko.Clear(); TxtWiek.Clear();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
        }

        private void BtnPrzypiszKlub_Click(object sender, RoutedEventArgs e)
        {
            var uiZawodnik = ListaDatagrid.SelectedItem as Zawodnik;
            var uiKlub = CmbKlubyWybior.SelectedItem as KlubSportowy;

            if (uiZawodnik == null || uiKlub == null) { MessageBox.Show("Wybierz parę."); return; }

            try
            {
                using (var context = new AppDbContext())
                {
                    // Pobieramy świeże wersje z bazy po ID
                    var dbZawodnik = context.Zawodnicy.FirstOrDefault(x => x.Id == uiZawodnik.Id);
                    var dbKlub = context.Kluby.FirstOrDefault(x => x.Id == uiKlub.Id);

                    if (dbZawodnik == null || dbKlub == null) return;

                    if (!dbKlub.PasujeDo(dbZawodnik))
                    {
                        MessageBox.Show($"Klub wymaga min. {dbKlub.MinimalnePunkty} pkt.");
                        return;
                    }

                    dbZawodnik.PrzypiszKlub(dbKlub.Id, dbKlub.Nazwa);
                    context.SaveChanges();
                }
                OdswiezWszystko();
                MessageBox.Show("Przypisano!");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnWypiszKlub_Click(object sender, RoutedEventArgs e)
        {
            var uiZawodnik = ListaDatagrid.SelectedItem as Zawodnik;
            if (uiZawodnik == null) return;

            using (var context = new AppDbContext())
            {
                var dbZawodnik = context.Zawodnicy.FirstOrDefault(x => x.Id == uiZawodnik.Id);
                if (dbZawodnik != null)
                {
                    dbZawodnik.WypiszZKlubu();
                    context.SaveChanges();
                }
            }
            OdswiezWszystko();
            MessageBox.Show("Wypisano.");
        }

        // --- WYNIKI (TU BYŁ GŁÓWNY PROBLEM) ---

        private void BtnDodajWynik_Click(object sender, RoutedEventArgs e)
        {
            // 1. Sprawdzenie danych wejściowych z GUI
            var uiZawodnik = ListaDatagrid.SelectedItem as Zawodnik;
            if (uiZawodnik == null)
            {
                MessageBox.Show("Najpierw zaznacz zawodnika na liście po lewej.");
                return;
            }

            if (CmbZawodyWybor.SelectedItem is not WynikZawodow szablon)
            {
                MessageBox.Show("Wybierz zawody z listy rozwijanej.");
                return;
            }

            if (!int.TryParse(TxtMiejsce.Text, out int miejsce))
            {
                MessageBox.Show("Podaj miejsce (musi być liczbą).");
                return;
            }

            DateTime data = DateDataZawodow.SelectedDate ?? DateTime.Now;

            try
            {
                // 2. Otwarcie "świeżego" połączenia do bazy
                using (var context = new AppDbContext())
                {
                    // Pobieramy zawodnika ŚWIEŻO z bazy, aby mieć pewność, że istnieje
                    // Używamy "Include", żeby pobrać też jego listę wyników
                    var dbZawodnik = context.Zawodnicy
                                            .Include(z => z.Wyniki)
                                            .FirstOrDefault(z => z.Id == uiZawodnik.Id);

                    if (dbZawodnik == null)
                    {
                        MessageBox.Show("Błąd krytyczny: Nie znaleziono tego zawodnika w bazie danych. Spróbuj odświeżyć aplikację.");
                        return;
                    }

                    // 3. Tworzymy nowy wynik
                    var nowyWynik = new WynikZawodow(
                        data,
                        szablon.NazwaZawodow,
                        miejsce,
                        szablon.TrudnoscTrasy,
                        szablon.PunktyBazowe
                    );

                    // 4. Dodajemy wynik i przeliczamy punkty na obiekcie BAZODANOWYM
                    dbZawodnik.DodajWynik(nowyWynik);
                    _punktacjaService.Przelicz(dbZawodnik);

                    // 5. Wymuszamy na Entity Frameworku zauważenie zmian
                    context.Entry(dbZawodnik).State = EntityState.Modified;

                    // 6. Zapis
                    context.SaveChanges();
                }

                // 7. Sukces - odświeżamy widok
                OdswiezWszystko();
                MessageBox.Show("Wynik został pomyślnie dodany!");
            }
            catch (Exception ex)
            {
                // Wyświetlamy pełny błąd, żeby wiedzieć co poszło nie tak
                MessageBox.Show($"Wystąpił błąd zapisu:\n{ex.Message}\n\n{ex.InnerException?.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbZawodyWybor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbZawodyWybor.SelectedItem is WynikZawodow szablon)
            {
                TxtNazwaZawodow.Text = szablon.NazwaZawodow;
                TxtTrudnosc.Text = szablon.TrudnoscTrasy.ToString();
                TxtPunktyBazowe.Text = szablon.PunktyBazowe.ToString();
            }
        }

        // --- KLUBY ---
        private void BtnUtworzKlub_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nazwa = TxtKlubNazwa.Text;
                int minPkt = int.TryParse(TxtKlubMinPkt.Text, out int mp) ? mp : 0;
                int? limit = int.TryParse(TxtKlubLimit.Text, out int l) ? l : null;
                int? maxWiek = int.TryParse(TxtKlubMaxWiek.Text, out int mw) ? mw : null;
                var dyscypliny = Enum.GetValues(typeof(Dyscyplina)).Cast<Dyscyplina>();

                using (var context = new AppDbContext())
                {
                    var k = new KlubSportowy(nazwa, minPkt, maxWiek, dyscypliny, limit);
                    context.Kluby.Add(k);
                    context.SaveChanges();
                }
                OdswiezWszystko();
                TxtKlubNazwa.Clear();
                MessageBox.Show("Klub dodany.");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // --- SZABLONY ---
        private void BtnUtworzZawody_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nazwa = TxtDefZawodyNazwa.Text;
                int trudnosc = int.TryParse(TxtDefZawodyTrudnosc.Text, out int t) ? t : 1;
                int pkt = int.TryParse(TxtDefZawodyPkt.Text, out int p) ? p : 0;

                using (var context = new AppDbContext())
                {
                    var szablon = new WynikZawodow(DateTime.Now, nazwa, 1, trudnosc, pkt);
                    context.Wyniki.Add(szablon);
                    context.SaveChanges();
                }
                OdswiezWszystko();
                TxtDefZawodyNazwa.Clear();
                MessageBox.Show("Szablon dodany.");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // --- WIDOK ---
        private void ListaDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OdswiezPanelDolny(ListaDatagrid.SelectedItem as Zawodnik);
        }

        private void OdswiezPanelDolny(Zawodnik? z)
        {
            if (z != null)
            {
                TxtWybranyInfo.Text = $"{z.Imie} {z.Nazwisko}";
                TxtRangaStopka.Text = z.Ranga.ToString().ToUpper();
                TxtPunktyPasek.Text = $"{z.Punkty} / 20000";

                double target = z.Punkty > 20000 ? 20000 : z.Punkty;
                DoubleAnimation anim = new DoubleAnimation(target, TimeSpan.FromSeconds(0.5));
                PasekPostepu.BeginAnimation(ProgressBar.ValueProperty, anim);
            }
            else
            {
                TxtWybranyInfo.Text = "Brak wyboru";
                TxtRangaStopka.Text = "---";
                PasekPostepu.Value = 0;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}