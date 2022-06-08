using System;
using System.Collections.Generic;

#nullable disable

namespace BibliotekaLib.Models
{
    public partial class Genere
    {
        public Genere()
        {
            Books = new HashSet<Book>();
        }

        public int GenereId { get; set; }
        public string GenereName { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
