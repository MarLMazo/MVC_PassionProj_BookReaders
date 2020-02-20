using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookReader.Data;
using BookReader.Models;
using System.Data.SqlClient;
using BookReader.Models.ViewModels;
using System.Diagnostics;


namespace BookReader.Controllers
{
    //TO DO: add Server side Validation
    //TO DO: Pagination without using PAgelist framework clue: use offset for mssql query, mysql: LIMIT should do the trick to do Pagination
    //TO DO: Create a Login Feature for the admin
    public class ReaderController : Controller
    {
        private BookReaderContext db = new BookReaderContext();

        // GET: Reader
      
        public ActionResult List(string search, string sortOrder)
        {
            string order;

            switch (sortOrder)
            {
                case "last_name":
                    order = " ORDER BY ReaderLname";
                    break;              
                case "address":
                    order = " ORDER BY ReaderAddress";
                    break;              
                case "phone":
                    order = " ORDER BY ReaderPhone";
                    break;            
                default:
                    order = " ORDER BY ReaderFname";
                    break;

                    //case "last_name_desc":
                    //    order = " ORDER BY ReaderLname DESC";
                    //    break;
                    //case "address_desc":
                    //    order = " ORDER BY ReaderAddress DESC";
                    //    break;
                    //case "phone_desc":
                    //    order = " ORDER BY ReaderPhone DESC";
                    //    break;
                    //case "first_name_desc":
                    //    order = " ORDER BY ReaderPhone DESC";
                    //    break;
            }

            List<Reader> readers;
            if (!string.IsNullOrEmpty(search))
            {
                //readers = db.Readers.ToList();
                //string query = "Select * from Readers where ReaderFname LIKE '%'+ @search + '%' OR ReaderLname LIKE '%'+ @search + '%'";
                //Phonenumber has no % Before search string cause I want to search exactly the first number that the user input 
                string query = "Select * from Readers where ReaderFname LIKE '%'+ @search + '%' OR ReaderLname LIKE '%'+ @search + '%' OR ReaderPhone LIKE @search + '%'";
                readers = db.Readers.SqlQuery(query + order, new SqlParameter("@search", search)).ToList();
                //Debug.WriteLine(query);
                //readers = db.Readers.Where(s => s.ReaderFname.Contains(search)|| s.ReaderLname.Contains(search)).ToList();
            }
            else
            {
                readers = db.Readers.SqlQuery("Select * from Readers"+ order).ToList();
            }

           

            return View(readers);
        }
        // GET: Reader/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Reader selectedreader = db.Readers.SqlQuery("Select * from Readers where ReaderId = @id", new SqlParameter("@id", id)).First();
            
            if (selectedreader == null)
            {
                return HttpNotFound();
            }
            List<Book> readbooks = db.Books.SqlQuery("Select * from Readers INNER JOIN BookReaders ON Readers.ReaderId = BookReaders.ReaderId INNER JOIN Books ON BookReaders.BookId = Books.BookId where Readers.ReaderId = @id ORDER BY BookTitle;", new SqlParameter("@id", id)).ToList();
            //Debug.WriteLine(books);
            List<Book> allbooks = db.Books.SqlQuery("Select * from Books ORDER BY BookTitle").ToList();
            //Subtract values from allbooks - readbooks
            //This will help me have no duplicate on values if I try to do sql query to just select the books
            //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-find-the-set-difference-between-two-lists-linq
            //Microsoft Documentation July 20,2015- How to find the set difference between two lists
            IEnumerable<Book> notreadbooks = allbooks.Except(readbooks);

            DetailsReader detailsreader = new DetailsReader();
            detailsreader.Reader = selectedreader;
            detailsreader.Books = readbooks;
            //since notreadbooks is an IEnumerable, change it to list so you can use it as a List of values
            detailsreader.notreadBooks = notreadbooks.ToList();
            return View(detailsreader);
        }

        // GET: Reader/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //public ActionResult Create(string ReaderFname, string ReaderLname, string ReaderAddress, Int64 ReaderPhone)
        public ActionResult Create(string ReaderFname, string ReaderLname, string ReaderAddress, long ReaderPhone)
        {
            //long is the datatype needed for the phone since it has 10 digit number unline INT which only has 9 digit number
            string query = "insert into Readers (ReaderFname, ReaderLname, ReaderAddress, ReaderPhone) values (@ReaderFname, @ReaderLname, @ReaderAddress, @ReaderPhone)";
            SqlParameter[] sqlparams = new SqlParameter[4];
            sqlparams[0] = new SqlParameter("@ReaderFname", ReaderFname);
            sqlparams[1] = new SqlParameter("@ReaderLname", ReaderLname);
            sqlparams[2] = new SqlParameter("@ReaderAddress", ReaderAddress);
            sqlparams[3] = new SqlParameter("@ReaderPhone", ReaderPhone);

            //Debug.WriteLine(query);
            db.Database.ExecuteSqlCommand(query, sqlparams);
            //Customer book = new Customer();
            //book.Books = db.Books.Where(BookdId = bookid);

            //Customer order = new Customer();
            //order.CustomerFname = CustomerFname;
            //order.Books = db.Books.Where( );
            //db.Customers.Add(order);
            //db.SaveChanges();

            return RedirectToAction("List");
        }

        // GET: Reader/Edit/5
        public ActionResult Update(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Reader selectedreader = db.Readers.SqlQuery("Select * from Readers where ReaderId = @id", new SqlParameter("@id", id)).First();

            if (selectedreader == null)
            {
                return HttpNotFound();
            }
            return View(selectedreader);
        }

     
        [HttpPost]
        //public ActionResult Update(int id, string ReaderFname, string ReaderLname, string ReaderAddress, Int64 ReaderPhone)
        public ActionResult Update(int id, string ReaderFname, string ReaderLname, string ReaderAddress, long ReaderPhone)
        {
            string query = "update Readers set ReaderFname = @ReaderFname, ReaderLname = @ReaderLname, ReaderAddress = @ReaderAddress, ReaderPhone = @ReaderPhone where ReaderId = @id ";
            SqlParameter[] sqlparams = new SqlParameter[5];
            sqlparams[0] = new SqlParameter("@ReaderFname", ReaderFname);
            sqlparams[1] = new SqlParameter("@ReaderLname", ReaderLname);
            sqlparams[2] = new SqlParameter("@ReaderAddress", ReaderAddress);
            sqlparams[3] = new SqlParameter("@ReaderPhone", ReaderPhone);
            sqlparams[4] = new SqlParameter("@id", id);

            //Debug.WriteLine(query);
            db.Database.ExecuteSqlCommand(query, sqlparams);

            return RedirectToAction("List");
        }

        // GET: Reader/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reader selectedreader = db.Readers.SqlQuery("Select * from Readers where ReaderId = @id", new SqlParameter("@id", id)).First();
            if (selectedreader == null)
            {
                return HttpNotFound();
            }
            return View(selectedreader);
        }

        //To execute DeleteConfirmed as HTTPPOST of Action "Delete"
        [HttpPost, ActionName("Delete")]
     
        public ActionResult DeleteConfirmed(int id)
        {
            //query to delete the Reader on Reader Table
            string delreader = "delete from Readers where ReaderId= @id";
            //query to delete the relationship on the bridging table
            string delrelationship = "delete from BookReaders where ReaderId= @id";
            //Debug.WriteLine(delreader);
            //Debug.WriteLine(delrelationship);
            SqlParameter sqlparams = new SqlParameter("@id", id);
            db.Database.ExecuteSqlCommand(delreader, sqlparams);
            db.Database.ExecuteSqlCommand(delrelationship, sqlparams);

            return RedirectToAction("List");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult DeleteBook(int id, ICollection<int> BookDelete)
        {
            //Collecting data by a collection of BookDelete means creating an array to fetch all the data inside BookDelete
            //This will run on each value of BookDelete
            //List<Reader> readers = db.Readers.SqlQuery("Select * from Books INNER JOIN BookReaders ON Books.bookId = BookReaders.bookId INNER JOIN Readers ON BookReaders.ReaderId = Readers.ReaderId where Books.BookId = @id;", new SqlParameter("@id", id)).ToList();
            foreach( var selection in BookDelete)
            {
                string query = "delete from BookReaders where BookReaders.BookId= @BookId AND BookReaders.ReaderId = @id";

                SqlParameter[] sqlparams = new SqlParameter[2];
                sqlparams[0] = new SqlParameter("@BookId", selection);
                sqlparams[1] = new SqlParameter("@id", id);
                db.Database.ExecuteSqlCommand(query, sqlparams);
                //var sample = "Select * from BookReaders INNER JOIN Books ON BookReaders.BookId = Books.BookId WHERE BookReaders.Reader = 1";

                //Debug.WriteLine("BookId = " +id + "ReaderId = "+ ReaderId);
            }
            //Returning to the same page with the same Id
            //return RedirectToAction("Details", id);
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        public ActionResult AddBook(int id, ICollection<int> BookAdd)
        {
            //BookAdd will be an array of values and execute each value using foreach statement
            //This will execute on each book to add each book on the Reader 
            //List<Reader> readers = db.Readers.SqlQuery("Select * from Books INNER JOIN BookReaders ON Books.bookId = BookReaders.bookId INNER JOIN Readers ON BookReaders.ReaderId = Readers.ReaderId where Books.BookId = @id;", new SqlParameter("@id", id)).ToList();
            foreach (var selection in BookAdd)
            {
                string query = "insert into BookReaders (BookId, ReaderId) values (@BookId, @id)";
                SqlParameter[] sqlparams = new SqlParameter[2];
                sqlparams[0] = new SqlParameter("@BookId", selection);
                sqlparams[1] = new SqlParameter("@id", id);
                db.Database.ExecuteSqlCommand(query, sqlparams);
                //var sample = "Select * from BookReaders INNER JOIN Books ON BookReaders.BookId = Books.BookId WHERE BookReaders.Reader = 1";

                //Debug.WriteLine("BookId = " +id + "ReaderId = "+ selectionselection);
                //Return to the same page
            }
            //Returning to the same page with the same Id
            return RedirectToAction("Details", new { id });
        }
    }
}


