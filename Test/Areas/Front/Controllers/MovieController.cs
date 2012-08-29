using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Models;

namespace Test.Areas.Front.Controllers
{ 
    public class MovieController : Controller
    {
        private MovieDBContext db = new MovieDBContext();

        //
        // GET: /Front/Movie/

        public ViewResult Index()
        {
            return View(db.Movies.ToList());
        }

        //
        // GET: /Front/Movie/Details/5

        public ViewResult Details(int id)
        {
            Movie movie = db.Movies.Find(id);
            return View(movie);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}