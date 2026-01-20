
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MiniBiblioteka
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public int Pages { get; set; }
        public string Category { get; set; }

        public Book(int id, string title, string author, int year, int pages, string category)
        {
            Id = id;
            Title = title;
            Author = author;
            Year = year;
            Pages = pages;
            Category = category;
        }

        public override string ToString()
        {
            return $"{Id};{Title};{Author};{Year};{Pages};{Category}";
        }

        public static Book FromString(string line)
        {
            string[] data = line.Split(';');
            if (data.Length != 6) return null;
            return new Book(
                int.Parse(data[0]),
                data[1],
                data[2],
                int.Parse(data[3]),
                int.Parse(data[4]),
                data[5]
            );
        }
    }

    public class Library
    {
        private List<Book> books = new List<Book>();
        private readonly string filePath = "baza.txt";

        public Library()
        {
            LoadFromFile();
        }

        public void LoadFromFile()
        {
            books.Clear();
            if (!File.Exists(filePath))
                return;

            foreach (var line in File.ReadAllLines(filePath))
            {
                var book = Book.FromString(line);
                if (book != null)
                    books.Add(book);
            }
        }

        public void SaveToFile()
        {
            File.WriteAllLines(filePath, books.Select(b => b.ToString()));
        }

        public void AddBook(Book book)
        {
            books.Add(book);
            SaveToFile();
            Console.WriteLine("Książka została dodana!\n");
        }

        public void ShowAllBooks()
        {
            if (books.Count == 0)
            {
                Console.WriteLine("Brak książek w bibliotece.\n");
                return;
            }

            Console.WriteLine("\nLISTA KSIĄŻEK:");
            Console.WriteLine("------------------------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-5} {"TYTUŁ",-25} {"AUTOR",-20} {"ROK",-6} {"STR",-5} {"KATEGORIA",-15}");
            Console.WriteLine("------------------------------------------------------------------------------------");

            foreach (var b in books)
            {
                Console.WriteLine($"{b.Id,-5} {b.Title,-25} {b.Author,-20} {b.Year,-6} {b.Pages,-5} {b.Category,-15}");
            }
            Console.WriteLine("------------------------------------------------------------------------------------\n");
        }

        public void SearchBooks(string query)
        {
            query = query?.Trim() ?? "";

            var results = books.Where(b =>
                b.Title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                b.Author.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                b.Category.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                b.Year.ToString().IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
            ).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("Nie znaleziono książek.\n");
                return;
            }

            Console.WriteLine("\nWYNIKI WYSZUKIWANIA:");
            Console.WriteLine("------------------------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-5} {"TYTUŁ",-25} {"AUTOR",-20} {"ROK",-6} {"STR",-5} {"KATEGORIA",-15}");
            Console.WriteLine("------------------------------------------------------------------------------------");

            foreach (var b in results)
            {
                Console.WriteLine($"{b.Id,-5} {b.Title,-25} {b.Author,-20} {b.Year,-6} {b.Pages,-5} {b.Category,-15}");
            }

            Console.WriteLine("------------------------------------------------------------------------------------\n");
        }


        public void SortBooks(string criterion)
        {
            List<Book> sortedBooks;

            switch (criterion.ToLower())
            {
                case "tytuł":
                    sortedBooks = books.OrderBy(b => b.Title).ToList();
                    break;
                case "autor":
                    sortedBooks = books.OrderBy(b => b.Author).ToList();
                    break;
                case "rok":
                    sortedBooks = books.OrderBy(b => b.Year).ToList();
                    break;
                default:
                    Console.WriteLine("Nieznane kryterium sortowania!");
                    return;
            }

            Console.WriteLine($"Posortowano książki wg: {criterion}\n");

            Console.WriteLine("------------------------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-5} {"TYTUŁ",-25} {"AUTOR",-20} {"ROK",-6} {"STR",-5} {"KATEGORIA",-15}");
            Console.WriteLine("------------------------------------------------------------------------------------");

            foreach (var b in sortedBooks)
            {
                Console.WriteLine($"{b.Id,-5} {b.Title,-25} {b.Author,-20} {b.Year,-6} {b.Pages,-5} {b.Category,-15}");
            }

            Console.WriteLine("------------------------------------------------------------------------------------\n");
        }

        public void DeleteBook(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                Console.WriteLine("Nie znaleziono książki o podanym ID.\n");
                return;
            }
            books.Remove(book);
            SaveToFile();
            Console.WriteLine("Książka została usunięta.\n");
        }

        public void EditBook(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                Console.WriteLine("Nie znaleziono książki o podanym ID.\n");
                return;
            }

            Console.WriteLine("Wprowadź nowe dane (pozostaw puste, aby nie zmieniać):");

            Console.Write($"Tytuł ({book.Title}): ");
            string title = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(title)) book.Title = title;

            Console.Write($"Autor ({book.Author}): ");
            string author = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(author)) book.Author = author;

            Console.Write($"Rok ({book.Year}): ");
            string yearStr = Console.ReadLine();
            if (int.TryParse(yearStr, out int year)) book.Year = year;

            Console.Write($"Liczba stron ({book.Pages}): ");
            string pagesStr = Console.ReadLine();
            if (int.TryParse(pagesStr, out int pages)) book.Pages = pages;

            Console.Write($"Kategoria ({book.Category}): ");
            string category = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(category)) book.Category = category;

            SaveToFile();
            Console.WriteLine("Książka została zaktualizowana.\n");
        }

        public int GetNextId()
        {
            return books.Count == 0 ? 1 : books.Max(b => b.Id) + 1;
        }
    }
    class Program
    {
        static void Main()
        {
            Library library = new Library();
            bool running = true;

            while (running)
            {
                Console.WriteLine("=== MINI BIBLIOTEKA ===");
                Console.WriteLine("1. Wyświetl wszystkie książki");
                Console.WriteLine("2. Dodaj książkę");
                Console.WriteLine("3. Wyszukaj książkę");
                Console.WriteLine("4. Sortuj książki");
                Console.WriteLine("5. Edytuj książkę");
                Console.WriteLine("6. Usuń książkę");
                Console.WriteLine("0. Wyjście");
                Console.Write("Wybierz opcję: ");
                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        library.ShowAllBooks();
                        break;

                    case "2":
                        int id = library.GetNextId();

                        Console.Write("Tytuł: ");
                        string title = Console.ReadLine();

                        Console.Write("Autor: ");
                        string author = Console.ReadLine();

                        Console.Write("Rok wydania: ");
                        int.TryParse(Console.ReadLine(), out int year);

                        Console.Write("Liczba stron: ");
                        int.TryParse(Console.ReadLine(), out int pages);

                        Console.Write("Kategoria: ");
                        string category = Console.ReadLine();

                        library.AddBook(new Book(id, title, author, year, pages, category));
                        break;

                    case "3":
                        Console.Write("Wpisz tytuł lub rok: ");
                        string query = Console.ReadLine();
                        library.SearchBooks(query);
                        break;

                    case "4":
                        Console.Write("Sortuj wg (tytuł/autor/rok): ");
                        string criterion = Console.ReadLine();
                        library.SortBooks(criterion);
                        break;

                    case "5":
                        Console.Write("Podaj ID książki do edycji: ");
                        if (int.TryParse(Console.ReadLine(), out int editId))
                            library.EditBook(editId);
                        break;

                    case "6":
                        Console.Write("Podaj ID książki do usunięcia: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                            library.DeleteBook(deleteId);
                        break;

                    case "0":
                        running = false;
                        Console.WriteLine("Do zobaczenia!");
                        break;

                    default:
                        Console.WriteLine("Nieprawidłowa opcja!\n");
                        break;
                }
            }
        }
    }
}
