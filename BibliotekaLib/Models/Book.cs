using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace BibliotekaLib.Models
{
    public partial class Book
    {
        DBConnect dB = new DBConnect();

        public Book()
        {
            BookAuthors = new HashSet<BookAuthor>();
        }

        public int BookId { get; set; }
        public string Title { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int GenereId { get; set; }
        public int ReadCount { get; set; }
        public int ShelfId { get; set; }

        public virtual Genere Genere { get; set; }
        public virtual Shelf Shelf { get; set; }
        public virtual ICollection<BookAuthor> BookAuthors { get; set; }

        public string AuthorsToString => string.Join(", ", dB.GetBookAuthors(this).Select(x => x.FirstName + " " + x.LastName));
    }
}
