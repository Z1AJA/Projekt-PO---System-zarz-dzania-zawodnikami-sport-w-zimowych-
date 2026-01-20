using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
// PONIŻEJ POPRAWIONE USINGI:
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;
using system_zawodnicy_zimowi.core.Services;
using system_zawodnicy_zimowi.Data;

namespace system_zawodnicy_zimowi
{
    public partial class MainWindow : Window
    {
        // Kontekst bazy danych
        private readonly AppDbContext _context = new AppDbContext();
        
        // Serwis punktacji
        private readonly PunktacjaService _punktacjaService = new PunktacjaService();

        // Kolekcje
        public ObservableCollection<Zawodnik> Zawodnicy { get; set; } = new ObservableCollection<Zawodnik>();
        public ObservableCollection<KlubSportowy> Kluby { get; set; } = new ObservableCollection<KlubSportowy>();
        public ObservableCollection<WynikZawodow> BazaZawodow { get; set; } = new ObservableCollection<WynikZawodow>();

        public MainWindow()
        {
            InitializeComponent();

            try 
            {
                // 1. Inicjalizacja bazy
                _context.Database.EnsureCreated();
                
                // 2. Ładowanie danych
                ZaladujDaneZBazy();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd bazy danych: " + ex.Message + "\n\nSpróbuj usunąć foldery bin i obj z katalogu projektu.");
            }

            // 3. Przypisanie do GUI
            ListaDatagrid.ItemsSource = Zawodnicy;
            GridKluby.ItemsSource = Kluby;
            CmbKlubyWybior.ItemsSource = Kluby;
            GridZawody.ItemsSource = BazaZawodow;
            CmbZawodyWybor.ItemsSource = BazaZawodow;

            // Obsługa zdarzenia zmiany rangi
            _punktacjaService.RangaZmieniona += (zawodnik, staraRanga, nowaRanga) => 
            {
                MessageBox.Show(
                    $"GRATULACJE!\nZawodnik {zawodnik.Imie} {zawodnik.Nazwisko} awansował!\n" +
                    $"{staraRanga} -> {nowaRanga}", 
                    "Awans", MessageBoxButton.OK, MessageBoxImage.Information);
            };
        }

        private void ZaladujDaneZBazy()
        {
            // Ładowanie z Include (wyniki zawodnika)
            _context.Zawodnicy.Include(z => z.Wyniki).Load();
            Zawodnicy = _context.Zawodnicy.Local.ToObservableCollection();

            _context.Kluby.Load();
            Kluby = _context.Kluby.Local.ToObservableCollection();

            _context.Wyniki.Load();
            
            // Filtrowanie szablonów (wyniki bez przypisanego zawodnika)
            var wszystkieWyniki = _context.Wyniki.Local.ToList();
            var idsWynikowGraczy = Zawodnicy.SelectMany(z => z.Wyniki).Select(w => w.Id).ToHashSet();
            
            var szablony = wszystkieWyniki.Where(w => !idsWynikowGraczy.Contains(w.Id)).ToList();
            BazaZawodow = new ObservableCollection<WynikZawodow>(szablony);
        }

        // --- ZAWODNICY ---
        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(TxtWiek.Text, out int wiek))
                {
                    MessageBox.Show("Podaj poprawny wiek.");
                    return;
                }

                if (CmbTyp.SelectedItem is not ComboBoxItem selectedItem) return;
                string typ = selectedItem.Content?.ToString() ?? "";

                Zawodnik nowyZawodnik = typ.Contains("Narciarz") 
                    ? new NarciarzAlpejski(TxtImie.Text, TxtNazwisko.Text, wiek) 
                    : new Snowboardzista(TxtImie.Text, TxtNazwisko.Text, wiek);

                Zawodnicy.Add(nowyZawodnik);
                _context.SaveChanges();

                TxtImie.Clear(); TxtNazwisko.Clear(); TxtWiek.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu: " + ex.Message);
            }
        }

        private void BtnPrzypiszKlub_Click(object sender, RoutedEventArgs e)
        {
            var zawodnik = ListaDatagrid.SelectedItem as Zawodnik;
            var klub = CmbKlubyWybior.SelectedItem as KlubSportowy;

            if (zawodnik == null || klub == null)
            {
                MessageBox.Show("Wybierz zawodnika i klub.");
                return;
            }

            try
            {
                if (!klub.PasujeDo(zawodnik))
                {
                    MessageBox.Show($"Klub odrzucił zawodnika (wymagane pkt: {klub.MinimalnePunkty})");
                    return;
                }

                zawodnik.PrzypiszKlub(klub.Id, klub.Nazwa);
                _context.SaveChanges();
                OdswiezWidok(zawodnik);
                MessageBox.Show($"Przypisano do: {klub.Nazwa}");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnWypiszKlub_Click(object sender, RoutedEventArgs e)
        {
            if (ListaDatagrid.SelectedItem is Zawodnik z)
            {
                z.WypiszZKlubu();
                _context.SaveChanges();
                OdswiezWidok(z);
                MessageBox.Show("Wypisano z klubu.");
            }
            else MessageBox.Show("Wybierz zawodnika.");
        }

        // --- WYNIKI ---
        private void BtnDodajWynik_Click(object sender, RoutedEventArgs e)
        {
            var zawodnik = ListaDatagrid.SelectedItem as Zawodnik;
            if (zawodnik == null) { MessageBox.Show("Wybierz zawodnika."); return; }

            try
            {
                if (CmbZawodyWybor.SelectedItem is not WynikZawodow szablon) 
                {
                    MessageBox.Show("Wybierz zawody z listy.");
                    return;
                }

                if (!int.TryParse(TxtMiejsce.Text, out int miejsce)) 
                {
                    MessageBox.Show("Podaj miejsce.");
                    return;
                }

                DateTime data = DateDataZawodow.SelectedDate ?? DateTime.Now;

                var nowyWynik = new WynikZawodow(data, szablon.NazwaZawodow, miejsce, szablon.TrudnoscTrasy, szablon.PunktyBazowe);

                zawodnik.DodajWynik(nowyWynik);
                _punktacjaService.Przelicz(zawodnik);
                
                _context.SaveChanges();
                
                OdswiezWidok(zawodnik);
                MessageBox.Show("Wynik dodany!");
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
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
                
                var klub = new KlubSportowy(nazwa, minPkt, maxWiek, dyscypliny, limit);
                
                Kluby.Add(klub);
                _context.SaveChanges();
                MessageBox.Show("Klub utworzony!");
                TxtKlubNazwa.Clear();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
        }

        // --- SZABLONY ---
        private void BtnUtworzZawody_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nazwa = TxtDefZawodyNazwa.Text;
                int trudnosc = int.TryParse(TxtDefZawodyTrudnosc.Text, out int t) ? t : 1;
                int pkt = int.TryParse(TxtDefZawodyPkt.Text, out int p) ? p : 0;

                var szablon = new WynikZawodow(DateTime.Now, nazwa, 1, trudnosc, pkt);

                _context.Wyniki.Add(szablon);
                _context.SaveChanges();
                BazaZawodow.Add(szablon);

                MessageBox.Show("Szablon dodany!");
                TxtDefZawodyNazwa.Clear();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
        }

        // --- WIDOK ---
        private void ListaDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OdswiezWidok(ListaDatagrid.SelectedItem as Zawodnik);
        }

        private void OdswiezWidok(Zawodnik? z)
        {
            ListaDatagrid.Items.Refresh();

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
                TxtPunktyPasek.Text = "0 / 20000";
                PasekPostepu.Value = 0;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}