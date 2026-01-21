using System;
using System.Collections.Generic;
using System.Linq;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Enums;
using system_zawodnicy_zimowi.core.Domain.Exceptions;
using system_zawodnicy_zimowi.Data;

namespace system_zawodnicy_zimowi.consoleAPP
{
    class app
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- SYSTEM ZAWODNIKÓW ZIMOWYCH ---");

            // 1. Tworzymy managera
            ManagerDanych manager = new ManagerDanych();

            // 2. Wypełniamy bazę (to wywoła Twoją metodę InicjalizujBaze)
            Console.WriteLine("Trwa łączenie z bazą i dodawanie danych...");
            try
            {
                manager.InicjalizujBaze();
                Console.WriteLine("SUKCES! Baza została zaktualizowana.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("BŁĄD BAZY: " + ex.Message);
            }

            // 3. Testujemy pobieranie danych (żebyś widział, że działa)
            var zawodnicy = manager.PobierzWszystkich();
            Console.WriteLine($"\nLiczba zawodników w bazie: {zawodnicy.Count}");

            foreach (var z in zawodnicy)
            {
                Console.WriteLine($"- {z.Imie} {z.Nazwisko} (Wiek: {z.Wiek})");
            }

            Console.WriteLine("\nKoniec programu. Wciśnij Enter.");
            Console.ReadLine();

        }
    }
}