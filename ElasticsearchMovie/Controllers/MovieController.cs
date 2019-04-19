using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElasticsearchMovie.ElasticHelper;
using ElasticsearchMovie.Models;
using IdentitySample.Models;
using Nest;

namespace ElasticsearchMovie.Controllers
{
    public class MovieController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
   



        public MovieController()
        {
            if (!ElasticsearchHelper.EsClient().IndexExists("movies").Exists)
            {
                ElasticsearchHelper.CreateMovieIndex();
            }
        }

        // GET: Movie
        public ActionResult Index(string q)
        {

            if (!String.IsNullOrEmpty(q))
            {
                try
                {


                    var response = ElasticsearchHelper.EsClient().Search<Movie>(s => s
                     .Index("movies")
                     .Type("movie")
                     .From(0)
                     .Size(1000)
                     .Query(a => a.QueryString(b => b.Query(q.ToString())))
                     );

                    var datasend = (from hits in response.Hits
                                    select hits.Source).ToList();

                    ViewBag.Data = datasend;

                    return View(ViewBag.Data);


                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            else
            {

                try
                {


                    var response = ElasticsearchHelper.EsClient().Search<Movie>(s => s
                     .Index("movies")
                     .Type("movie")
                     .From(0)
                     .Size(1000)
                     .Source(a => a.IncludeAll())
                     .Query(a => a.MatchAll())
                     );




                    ViewBag.Data = response.Documents;

                }
                catch (Exception ex)
                {

                    throw;
                }


                return View(ViewBag.Data);
            }




        }
        
        public ActionResult DeleteIndex()
        {
            ElasticsearchHelper.DeleteIndex("movies");
            string mesaj = "<script language='javascript' type='text/javascript'>alert('Silme İşlemi Başarıyla Gerçekleşmiştir!');window.location.href = '/home/index/';</script>";
            return Content(mesaj);
        }

        // GET: Movie/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movie.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: Movie/Create
        public ActionResult Create()
        {
            ViewBag.GenreID = new SelectList(db.Genre, "Id", "Name");
            return View();
        }

        // POST: Movie/Create
        // Aşırı gönderim saldırılarından korunmak için, lütfen bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için https://go.microsoft.com/fwlink/?LinkId=317598 sayfasına bakın.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,CreatedDate,GenreId")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                movie.CreatedDate = DateTime.Now;
                db.Movie.Add(movie);
                db.SaveChanges();

                ElasticsearchHelper.EsClient().Index<Movie>(movie,
                   id => id.Index("movies")
               .Type(TypeName.From<Movie>())
                                      .Id(movie.Id)
                                      .Refresh(Elasticsearch.Net.Refresh.True));

                return RedirectToAction("Index");
            }
            ViewBag.GenreID = new SelectList(db.Genre, "ID", "Name", movie.GenreId);
            return View(movie);
        }

        // GET: Movie/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movie.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            ViewBag.GenreID = new SelectList(db.Genre, "ID", "Name", movie.GenreId);
            return View(movie);
        }

        // POST: Movie/Edit/5
        // Aşırı gönderim saldırılarından korunmak için, lütfen bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için https://go.microsoft.com/fwlink/?LinkId=317598 sayfasına bakın.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,CreatedDate,GenreId")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                var response = ElasticsearchHelper.EsClient().Update<Movie, Movie>(movie.Id, d => d
                      .Index("movies")
                      .Type("movie")
                      .Refresh(Elasticsearch.Net.Refresh.True)
                      .Doc(new Movie
                      {
                          Id = movie.Id,
                          Title = movie.Title,
                          Description = movie.Description,
                          GenreId = movie.GenreId,
                          CreatedDate = DateTime.Now

                      }));
                return RedirectToAction("Index");
            }
            ViewBag.GenreID = new SelectList(db.Genre, "ID", "Name", movie.GenreId);
            return View(movie);
        }

        // GET: Movie/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movie.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movie.Find(id);
            db.Movie.Remove(movie);
            db.SaveChanges();

            var response = ElasticsearchHelper.EsClient().Delete<Movie>(id, d => d
                .Index("movies")
                .Type("movie")
                .Refresh(Elasticsearch.Net.Refresh.True));

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
