using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookReader.Data;
using BookReader.Models.ViewModels;
using BookReader.Models;
using System.Diagnostics;

namespace BookReader.Controllers
{
    public class HomeController : Controller
        
    {
        private BookReaderContext db = new BookReaderContext();
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}