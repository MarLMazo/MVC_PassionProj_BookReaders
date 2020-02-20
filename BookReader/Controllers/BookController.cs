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
using PagedList.Mvc;
using PagedList;
using System.Diagnostics;

namespace BookReader.Controllers
{
    //TO DO: add Server side Validation
    //TO DO: Pagination without using PAgelist framework clue: use offset for mssql query, mysql: LIMIT should do the trick to do Pagination
    //TO DO: Create a Login Feature for the admin
    public class BookController : Controller
    {
        private BookReaderContext db = new BookReaderContext();

        // GET: Book
        public ActionResult List(string search, string sortOrder)
        {
            //ViewBag.CurrentSort = sortOrder;
            //List<Book> books = db.Books.SqlQuery("Select * from books").ToList();
            //int pageSize = 3;
            //int pageNumber = (page ?? 1);
            //return View(books.ToPagedList(pageNumber, pageSize));
            //return View(books);
            //Debug.WriteLine(query);
            //switch (search)
            //{
            //    case "January":
            //        search = "01";
            //        break;
            //    case "Jan":
            //        search = "01";
            //        break;        
            //    default:
            //        search = search;
            //        break;
            //}

            //Debug.WriteLine(search);
            //Using the first 3 letters of the month will still work in the datetime in SQL
            string order;

            switch (sortOrder)
            {
                
                case "publish":
                    order = " ORDER BY BookPublish DESC";
                    break;
                case "author":
                    order = " ORDER BY BookAuthor";
                    break;
                default:
                    order = " ORDER BY BookTitle";
                    break;
            }

            List<Book> books;
            if (!string.IsNullOrEmpty(search))
            {
                //readers = db.Readers.ToList();
                string query = "Select * from books where BookTitle LIKE '%'+ @search + '%' OR BookAuthor LIKE '%'+ @search + '%' OR BookPublish LIKE '%'+ @search + '%'";
                //string query = "Select * from books where BookTitle LIKE '%'+ @search + '%' OR BookAuthor LIKE '%'+ @search + '%'";
                books = db.Books.SqlQuery( query + order, new SqlParameter("@search", search)).ToList();
                //Debug.WriteLine(query);
                //Debug.WriteLine(books);
                //readers = db.Readers.Where(s => s.ReaderFname.Contains(search)|| s.ReaderLname.Contains(search)).ToList();
            }
            else
            {
                books = db.Books.SqlQuery("Select * from books" + order).ToList();
            }

            return View(books);

        }
        

        // GET: Book/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            Book selectedbook = db.Books.SqlQuery("Select * from books where BookId = @id", new SqlParameter("@id", id)).First();           
           
            if (selectedbook == null)
            {
                return HttpNotFound();
            }
            List<Reader> readers = db.Readers.SqlQuery("Select Readers.* from Books INNER JOIN BookReaders ON Books.bookId = BookReaders.bookId INNER JOIN Readers ON BookReaders.ReaderId = Readers.ReaderId where Books.BookId = @id ORDER BY ReaderFname;", new SqlParameter("@id", id)).ToList();
            //List<Reader> notreaders = db.Readers.SqlQuery("Select Readers.* from Books RIGHT JOIN BookReaders ON Books.bookId = BookReaders.bookId RIGHT JOIN Readers ON BookReaders.ReaderId = Readers.ReaderId WHERE Books.BookId IS NULL OR Books.BookId != @id;", new SqlParameter("@id", id)).ToList();
            List<Reader> allreaders = db.Readers.SqlQuery("Select * from Readers ORDER BY ReaderFname").ToList();
            //Subtract values from allreaders - readers
            //This will help me have no duplicate on values if I try to do sql query to just select all non-readers
            //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-find-the-set-difference-between-two-lists-linq
            //Microsoft Documentation July 20,2015- How to find the set difference between two lists
            IEnumerable<Reader> notreaders = allreaders.Except(readers);
            DetailsBook detailsbook = new DetailsBook();
         
            detailsbook.Book = selectedbook;
            detailsbook.Readers = readers;
            //notreaders is an IEnumerable type so in order to use it as a list, change it to Tolist();
            detailsbook.NotReaders = notreaders.ToList();
            //Debug.WriteLine(detailsbook);
            //Debug.WriteLine("Not readers= " +notreaders);
            //Debug.WriteLine("Readers= "readers);
            return View(detailsbook);


        }

        // GET: Book/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Book/Create
        [HttpPost]       
        public ActionResult Create(string BookTitle, string BookDescrp, string BookAuthor, DateTime? BookPublish)
        {
            
            string query = "insert into Books (BookTitle, BookDescrp, BookAuthor, BookPublish) values (@BookTitle,@BookDescrp,@BookAuthor,@BookPublish)";
            SqlParameter[] sqlparams = new SqlParameter[4];
            sqlparams[0] = new SqlParameter("@BookTitle", BookTitle);
            sqlparams[1] = new SqlParameter("@BookDescrp", BookDescrp);
            sqlparams[2] = new SqlParameter("@BookAuthor", BookAuthor);
            sqlparams[3] = new SqlParameter("@BookPublish", BookPublish);

            Debug.WriteLine(query);
            db.Database.ExecuteSqlCommand(query, sqlparams);

            return RedirectToAction("List");
        }

        // GET: Book/Edit/5
        public ActionResult Update(int? id)
        {
            // if id is null return Bad Request
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //id has value so create the selected books
            Book selectedbooks = db.Books.SqlQuery("Select * from books where BookId = @id", new SqlParameter("@id", id)).First();
            //if selected book is null or no value return Page Not found
            if (selectedbooks == null)
            {
                return HttpNotFound();
            }
            
            return View(selectedbooks);

        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public ActionResult Update(int id, string BookTitle, string BookDescrp, string BookAuthor, DateTime BookPublish)
        {

            string query = "update Books set BookTitle = @BookTitle, BookDescrp = @BookDescrp, BookAuthor = @BookAuthor, BookPublish = @BookPublish where BookId = @id ";
            SqlParameter[] sqlparams = new SqlParameter[5];
            sqlparams[0] = new SqlParameter("@BookTitle", BookTitle);
            sqlparams[1] = new SqlParameter("@BookDescrp", BookDescrp);
            sqlparams[2] = new SqlParameter("@BookAuthor", BookAuthor);
            sqlparams[3] = new SqlParameter("@BookPublish", BookPublish);
            sqlparams[4] = new SqlParameter("@id", id);

            //Debug.WriteLine(query);
            db.Database.ExecuteSqlCommand(query, sqlparams);

            return RedirectToAction("List");
        }

  
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Book selectedbooks = db.Books.SqlQuery("Select * from books where BookID = @id", new SqlParameter("@id", id)).First();
            
            if (selectedbooks == null)
            {
                return HttpNotFound();
            }
            return View(selectedbooks);
        }

        // POST: Book/Delete/5
        //In order to execute the post method using the "Delete" Action, the naming can be use to override the DeleteConfirmed Action
        [HttpPost, ActionName("Delete")]
        
        public ActionResult DeleteConfirmed(int id)
        {
            ////query to delete the Book in the books table
            string delbooks = "delete from books where BookId= @id";
            //query to delete the relationship on the bridging table
            string delrelationship = "delete from BookReaders where BookId = @id";
            //Debug.WriteLine(delbooks);
            //Debug.WriteLine(delrelationship);
            SqlParameter sqlparams = new SqlParameter("@id", id);
            //Run the sql command
            db.Database.ExecuteSqlCommand(delbooks, sqlparams);
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
        public ActionResult DeleteReader(int id, ICollection<int> ReaderDelete)
        {
            //ReaderDelete will be an array of values and execute each value using foreach statement
            //This will execute on each reader to delete each reader on the Book 
            foreach (var selection in ReaderDelete)
            {
                //Debug.WriteLine(selection);
                string query = "delete from BookReaders where BookReaders.ReaderId= @ReaderId AND BookReaders.BookId = @id";

                SqlParameter[] sqlparams = new SqlParameter[2];
                sqlparams[0] = new SqlParameter("@ReaderId", selection);
                sqlparams[1] = new SqlParameter("@id", id);
                db.Database.ExecuteSqlCommand(query, sqlparams);
            }
            //Redirect to the same page with the same id with same action
            return RedirectToAction("Details", new { id });

            //return View();
            //List<Reader> readers = db.Readers.SqlQuery("Select * from Books INNER JOIN BookReaders ON Books.bookId = BookReaders.bookId INNER JOIN Readers ON BookReaders.ReaderId = Readers.ReaderId where Books.BookId = @id;", new SqlParameter("@id", id)).ToList();          
            //var sample = "Select * from BookReaders INNER JOIN Books ON BookReaders.BookId = Books.BookId WHERE BookReaders.Reader = 1";
            //Debug.WriteLine("BookId = " +id + "ReaderId = "+ ReaderId);
            //return RedirectToAction("Details", new { id });
        }


        [HttpPost]
        public ActionResult AddReader(int id, ICollection<int> ReaderAdd)
        {
            //ReaderAdd will be an array of values and execute each value using foreach statement
            //This will execute on each reader to add each reader on the Book 
            //List<Reader> readers = db.Readers.SqlQuery("Select * from Books INNER JOIN BookReaders ON Books.bookId = BookReaders.bookId INNER JOIN Readers ON BookReaders.ReaderId = Readers.ReaderId where Books.BookId = @id;", new SqlParameter("@id", id)).ToList();
            foreach (var selection in ReaderAdd)
             {
                string query = "insert into BookReaders (BookId, ReaderId) values (@id, @ReaderId)";
                SqlParameter[] sqlparams = new SqlParameter[2];
                sqlparams[0] = new SqlParameter("@ReaderId", selection);
                sqlparams[1] = new SqlParameter("@id", id);
                db.Database.ExecuteSqlCommand(query, sqlparams);
             }

            //var sample = "Select * from BookReaders INNER JOIN Books ON BookReaders.BookId = Books.BookId WHERE BookReaders.Reader = 1";
            //Debug.WriteLine("BookId = " +id + "ReaderId = "+ ReaderId);
            //Redirect to the same page with the same id with same action
            return RedirectToAction("Details", new { id });
        }
      
    }
}
