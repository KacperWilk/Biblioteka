using BibliotekaLib.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BibliotekaLib
{

    /// Klasa zawierająca metody używane do zarządzania bazą danych aplikacji.
    /// 
    public class DBConnect
    {
        #region General Methods

        /// Metoda wywołująca skrypt CreateDatabase.sql, odpowiadający za tworzenie bazy danych programu.

        public void CreateDatabase()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BibliotekaLib.CreateDatabase.sql");

            StreamReader reader = new StreamReader(stream);

            using (SqlConnection connection = new SqlConnection(Helper.GetDefaultDBConnectionString() + ";Database=master"))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    var script = ParseSQLScript(reader.ReadToEnd());

                    foreach (var query in script)
                    {
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        #endregion

        #region Books Methods

        /// Zwraca listę książek, które znajdują się aktualnie w bazie danych.

        /// <returns>Lista obiektów klasy <c>Book</c></returns>
        public List<Book> GetBooks()
        {
            using (BibliotekaContext context = new BibliotekaContext())
            {
                return context.Books
                    .Include("Genere")
                    .Include("Shelf")
                    .ToList();
            }
        }


        /// Dodaj nową książkę do bazy danych.

        /// <param name="title">Tytuł ksiązki</param>
        /// <param name="purchaseDate">Data zakupu ksiązki</param>
        /// <param name="genere">Gatunek</param>
        /// <param name="shelf">Półka, na której znajduje się książka</param>
        /// <param name="authors">Lista autorów książki</param>
        public void AddBook(string title, DateTime purchaseDate, Genere genere, Shelf shelf, List<Author> authors)
        {
            title = title.Trim();

            if (title == null || title == "" || purchaseDate == null || genere == null || shelf == null || authors == null || authors.Count == 0)
                throw new ArgumentNullException();

            if (purchaseDate > DateTime.Now)
                throw new ArgumentOutOfRangeException();

            Book bookToAdd = new Book()
            {
                Title = title,
                PurchaseDate = purchaseDate,
                GenereId = genere.GenereId,
                ShelfId = shelf.ShelfId
            };

            using (BibliotekaContext context = new BibliotekaContext())
            {
                context.Add(bookToAdd);
                context.SaveChanges();                    
            }

            foreach (Author author in authors)
                AddBookAuthor(bookToAdd, author);
        }


        /// Modyfikuje istniejącą w bazie książkę.

        /// <param name="bookToModify">Książka, która ma zostać zmodyfikowana</param>
        /// <param name="title">Nowy tytuł</param>
        /// <param name="purchaseDate">Nowa data zakupu</param>
        /// <param name="genere">Nowy gatunek</param>
        /// <param name="shelf">Nowa półka</param>
        public void ModifyBook(Book bookToModify, string title, DateTime purchaseDate, Genere genere, Shelf shelf, List<Author> authors)
        {
            title = title.Trim();

            if (title == null || title == "" || purchaseDate == null || genere == null || shelf == null || authors == null || authors.Count == 0)
                throw new ArgumentNullException();

            if (purchaseDate > DateTime.Now)
                throw new ArgumentOutOfRangeException();

            bookToModify.Title = title;
            bookToModify.PurchaseDate = purchaseDate;
            bookToModify.Genere = genere;
            bookToModify.Shelf = shelf;

            using (BibliotekaContext context = new BibliotekaContext())
            {
                context.Update(bookToModify);
                context.SaveChanges();
            }

            List<Author> currentBookAuthors = GetBookAuthors(bookToModify);

            foreach(Author authorToRemove in currentBookAuthors)
            {
                if (!authors.Contains(authorToRemove))
                    RemoveAuthorFromBook(bookToModify, authorToRemove);
            }

            foreach(Author authorToAdd in authors)
            {
                if (!currentBookAuthors.Contains(authorToAdd))
                    AddBookAuthor(bookToModify, authorToAdd);
            }
        }


        /// Zwiększa licznik przeczytań książki o 1.

        /// <param name="bookRead">Przeczytana książka</param>
        public void IncrementBookReadCount(Book bookRead)
        {
            if (bookRead == null)
                throw new ArgumentNullException();

            using (BibliotekaContext context = new BibliotekaContext())
            {
                bookRead.ReadCount++;
                context.Update(bookRead);
                context.SaveChanges();
            }
        }


        /// Resetuje licznik przeczytań książki

        /// <param name="book">Książka, której licznik ma zostać zresetowany</param>
        public void ResetBookReadCount(Book book)
        {
            if (book == null)
                throw new ArgumentNullException();

            using (BibliotekaContext context = new BibliotekaContext())
            {
                book.ReadCount = 0;
                context.Update(book);
                context.SaveChanges();
            }
        }


        /// Usuwa istniejącą w bazie książkę.

        /// <param name="bookToRemove">Książka, która ma zostać usunieta</param>
        public void RemoveBook(Book bookToRemove)
        {
            if (bookToRemove == null)
                throw new ArgumentNullException();

            foreach (Author bookAuthor in GetBookAuthors(bookToRemove))
                RemoveAuthorFromBook(bookToRemove, bookAuthor);

            using (BibliotekaContext context = new BibliotekaContext())
            {
                context.Remove(bookToRemove);
                context.SaveChanges();
            }
        }
        #endregion

        #region Shelfs Methods

        /// Zwraca listę półek, które znajdują się aktualnie w bazie danych.

        /// <returns>Lista obiektów klasy <c>Shelf</c></returns>
        public List<Shelf> GetShelfs()
        {
            using (BibliotekaContext context = new BibliotekaContext())
            {
                return context.Shelfs.ToList();
            }
        }


        /// Dodaje nową półkę do bazy danych.

        /// <param name="name">Nazwa nowej półki</param>
        public void AddShelf(string name)
        {
            name = name.Trim();

            if (name == null || name == "")
                throw new ArgumentNullException();

            using (BibliotekaContext context = new BibliotekaContext())
            {
                context.Add(new Shelf() { ShelfName = name });
                context.SaveChanges();
            }
        }


        /// Usuwa istniejącą w bazie półkę.

        /// <param name="shelfToRemove">Półka, która ma zostać usunięta z bazy</param>
        public void RemoveShelf(Shelf shelfToRemove)
        {
            using (BibliotekaContext context = new BibliotekaContext())
            {
                context.Remove(shelfToRemove);
                context.SaveChanges();
            }
        }


        /// Modyfikuje istniejącą w bazie półkę.

        /// <param name="shelfToModify">Pólka, która ma zostać zmodyfikowana</param>
        /// <param name="name">Nowa nazwa półki</param>
        public void ModifyShelf(Shelf shelfToModify, string name)
        {
            name = name.Trim();

            if (shelfToModify == null || name == null || name == "")
                throw new ArgumentNullException();

            shelfToModify.ShelfName = name;

            using (BibliotekaContext context = new BibliotekaContext())
            {
                context.Update(shelfToModify);
                context.SaveChanges();
            }
        }
        #endregion

        #region Generes Methods

        /// Zwraca listę gatunków, które znajdują się aktualnie w bazie danych.

        /// <returns>Lista obiektów klasy <c>Genere</c></returns>
        public List<Genere> GetGeneres()
        {
            using (BibliotekaContext context = new BibliotekaContext())
            {
                return context.Generes.ToList();
            }
        }


        /// Dodaje nowy gatunek do bazy danych.

        /// <param name="name">Nazwa nowego gatunku</param>
        public void AddGenere(string name)
        {
            name = name.Trim();

            if (name == null || name == "")
                throw new ArgumentNullException();

            using (BibliotekaContext context = new BibliotekaContext())
            {
                context.Add(new Genere() { GenereName = name });
                context.SaveChanges();
            }
        }


        /// Usuwa istniejący w bazie gatunek.

        /// <param name="genereToRemove">Gatunek, który ma zostać usunięty z bazy</param>
        public void RemoveGenere(Genere genereToRemove)
        {
            using (BibliotekaContext context = new BibliotekaContext())
            {
                context.Remove(genereToRemove);
                context.SaveChanges();
            }
        }


        /// Modyfikuje istniejący w bazie gatunek.

        /// <param name="genereToModify">Autor, który ma zostać zmodyfikowany</param>
        /// <param name="name">Nowa nazwa gatunku</param>
        public void ModifyGenere(Genere genereToModify, string name)
        {
            name = name.Trim();

            if (genereToModify == null || name == null || name == "")
                throw new ArgumentNullException();

            genereToModify.GenereName = name;

            using (BibliotekaContext context = new BibliotekaContext())
            {
                context.Update(genereToModify);
                context.SaveChanges();
            }
        }
        #endregion

        #region Authors Methods

        /// Zwraca listę autorów, którzy znajdują się aktualnie w bazie danych.

        /// <returns>Lista obiektów klasy <c>Author</c></returns>
        public List<Author> GetAuthors()
        {
            using (BibliotekaContext context = new BibliotekaContext())
            {
                return context.Authors.ToList();
            }
        }


        /// Dodaje nowego autora do bazy danych.

        /// <param name="firstName">Imię autora</param>
        /// <param name="lastName">Nazwisko autora</param>
        public void AddAuthor(string firstName, string lastName)
        {
            firstName = firstName.Trim();
            lastName = lastName.Trim();

            if (firstName == "" || firstName == null || lastName == "" || lastName == null)
                throw new ArgumentNullException();

            using (BibliotekaContext context = new BibliotekaContext())
            {
                context.Authors.Add(new Author()
                {
                    FirstName = firstName,
                    LastName = lastName
                });
                context.SaveChanges();
            }
        }


        /// Usuwa istniejącego w bazie autora.

        /// <param name="authorToRemove">Autor, który ma zostać usunięty z bazy</param>
        public void RemoveAuthor(Author authorToRemove)
        {
            using (BibliotekaContext context = new BibliotekaContext())
            {
                context.Remove(authorToRemove);
                context.SaveChanges();
            }
        }


        /// Modyfikuje dane instniejącego w bazie autora.

        /// <param name="authorToModify">Autor, który ma zostać zmodyfikowany</param>
        /// <param name="firstName">Nowe imię autora</param>
        /// <param name="lastName">Nowe nazwisko autora</param>
        public void ModifyAuthor(Author authorToModify, string firstName, string lastName)
        {
            firstName = firstName.Trim();
            lastName = lastName.Trim();

            if (authorToModify == null || firstName == "" || firstName == null || lastName == "" || lastName == null)
                throw new ArgumentNullException();

            authorToModify.FirstName = firstName;
            authorToModify.LastName = lastName;

            using(BibliotekaContext context = new BibliotekaContext())
            {
                context.Update(authorToModify);
                context.SaveChanges();
            }
        }
        #endregion

        #region BookAuthors Methods

        /// Zwraca listę autorów danej książki.
        
        /// <param name="book">Książka, której autorzy mają zostać zwróceni</param>
        /// <returns>Lista obiektów klasy <c>Author</c></returns>
        public List<Author> GetBookAuthors(Book book)
        {
            using(BibliotekaContext context = new BibliotekaContext())
            {
                return context.BookAuthors
                    .Where(x => x.BookId == book.BookId)
                    .Include("Author")
                    .Select(x => x.Author).ToList();
            }
        }


        /// Dodaje istniejącego w bazie autora do istniejącej bazie ksiązki.

        /// <param name="book">Ksiązka, do której ma zostać dodany autor</param>
        /// <param name="author">Autor, który ma zostać przypisany do danej książki</param>
        public void AddBookAuthor(Book book, Author author) 
        {
            if (book == null || author == null)
                throw new ArgumentNullException();

            using(BibliotekaContext context = new BibliotekaContext())
            {
                context.Add(new BookAuthor()
                {
                    BookId = book.BookId,
                    AuthorId = author.AuthorId
                });
                context.SaveChanges();
            }
        }

        /// Usuwa autora z danej książki.
        /// 
        /// <param name="book">Książka, z której ma zostać usunięty dany autor</param>
        /// <param name="author">Autor, który ma zostać usunięty z danej książki</param>
        public void RemoveAuthorFromBook(Book book, Author author)
        {
            if (book == null || author == null)
                throw new ArgumentNullException();

            using (BibliotekaContext context = new BibliotekaContext())
            {
                context.Remove(new BookAuthor()
                {
                    BookId = book.BookId,
                    AuthorId = author.AuthorId
                });
                context.SaveChanges();
            }
        }
        #endregion

        #region Other Methods
        private IEnumerable<string> ParseSQLScript(string script)
        {
            var result = Regex.Split(script, @"^GO[\r\n]?$", RegexOptions.Multiline);

            return result.Where(x => !string.IsNullOrWhiteSpace(x));
        }
        #endregion
    }
}