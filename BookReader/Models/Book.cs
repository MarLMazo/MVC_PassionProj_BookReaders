using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BookReader.Models
{
    public class Book
    {
        [Key]

        public int BookId { get; set; }


        public string BookTitle { get; set; }

        public string BookDescrp { get; set; }

        public string BookAuthor { get; set; }

        public DateTime BookPublish { get; set; }
        

        public ICollection<Reader> Readers { get; set; }
    }
}