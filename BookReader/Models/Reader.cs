using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReader.Models
{
    public class Reader
    {
        [Key]
        public int ReaderId { get; set; }

        public string ReaderFname { get; set; }

        public string ReaderLname { get; set; }

        public string ReaderAddress { get; set; }

        //public Int64 ReaderPhone { get; set; }
        public long ReaderPhone { get; set; }


        public ICollection<Book> Books { get; set; }
    }
}