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
        
        private readonly PunktacjaService _punktacjaService = new PunktacjaService();

        public ObservableCollection<Zawodnik> Zawodnicy { get; set; } = new ObservableCollection<Zawodnik>();
        public ObservableCollection<KlubSportowy> Kluby { get; set; } = new ObservableCollection<KlubSportowy>();
        public ObservableCollection<RodzajZawodow> BazaZawodow { get; set; } = new ObservableCollection<RodzajZawodow>();

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // Inicjalizacja bazy przy starcie
                using (var context = new AppDbContext())
                {
                    //context.Database.EnsureDeleted();
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
                var rodzaje = context.RodzajeZawodow.ToList();

                BazaZawodow.Clear();
                foreach (var r in rodzaje) BazaZawodow.Add(r);
              
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
            var uiZawodnik = ListaDatagrid.SelectedItem as Zawodnik;
            if (uiZawodnik == null)
            {
                MessageBox.Show("Najpierw zaznacz zawodnika na liście po lewej.");
                return;
            }

            // ZMIANA: Sprawdzamy czy wybrano RodzajZawodow
            if (CmbZawodyWybor.SelectedItem is not RodzajZawodow wybranyRodzaj)
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
                using (var context = new AppDbContext())
                {
                    var dbZawodnik = context.Zawodnicy
                                            .Include(z => z.Wyniki)
                                            .FirstOrDefault(z => z.Id == uiZawodnik.Id);

                    if (dbZawodnik == null) return;

                    // ZMIANA: Pobieramy ten sam rodzaj zawodów z bazy (po ID)
                    var dbRodzaj = context.RodzajeZawodow.FirstOrDefault(r => r.Id == wybranyRodzaj.Id);

                    if (dbRodzaj == null) return;

                    // ZMIANA: Tworzymy wynik używając nowego konstruktora (przekazujemy cały obiekt rodzaju)
                    var nowyWynik = new WynikZawodow(data, miejsce, dbRodzaj);

                    dbZawodnik.DodajWynik(nowyWynik);
                    _punktacjaService.Przelicz(dbZawodnik);

                    context.Entry(nowyWynik).State = EntityState.Added;
                    context.SaveChanges();
                }

                OdswiezWszystko();
                MessageBox.Show("Wynik został pomyślnie dodany!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}\nInner: {ex.InnerException?.Message}");
            }
        }

        private void CmbZawodyWybor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (CmbZawodyWybor.SelectedItem is RodzajZawodow szablon)
            {
                // Uwaga: w klasie RodzajZawodow pole nazywa się 'Nazwa', a nie 'NazwaZawodow'
                TxtNazwaZawodow.Text = szablon.Nazwa;
                TxtTrudnosc.Text = szablon.Trudnosc.ToString();
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

        
        private void BtnUtworzZawody_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nazwa = TxtDefZawodyNazwa.Text;
                int trudnosc = int.TryParse(TxtDefZawodyTrudnosc.Text, out int t) ? t : 1;
                int pkt = int.TryParse(TxtDefZawodyPkt.Text, out int p) ? p : 0;

                using (var context = new AppDbContext())
                {
                    // ZMIANA: Tworzymy obiekt RodzajZawodow
                    var nowyRodzaj = new RodzajZawodow(nazwa, trudnosc, pkt);

                    // Dodajemy do nowej tabeli
                    context.RodzajeZawodow.Add(nowyRodzaj);
                    context.SaveChanges();
                }
                OdswiezWszystko();
                TxtDefZawodyNazwa.Clear();
                MessageBox.Show("Nowy rodzaj zawodów dodany do bazy.");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

       
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

        private void CmbTyp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TxtNazwisko_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void GridZawody_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}