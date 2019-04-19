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
    public class GenreController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        public GenreController()
        {
            if (!ElasticsearchHelper.EsClient().IndexExists("genres").Exists)
            {
                ElasticsearchHelper.CreateGenreIndex();
            }
        }
        // GET: Genre
        public ActionResult Index()
        {


            var liste = db.Genre.Include(a => a.Movies).ToList();

            try
            {


                var response = ElasticsearchHelper.EsClient().Search<Genre>(s => s
                 .Index("genres")
                 .Type("genre")
                 .From(0)
                 .Size(1000)
                 .Query(a => a.MatchAll())
                 
                 );

                


                ViewBag.Genre = response.Documents;



            }
            catch (Exception ex)
            {

                throw;
            }


            return View(ViewBag.Genre);
        }




        public ActionResult DeleteIndex()
        {
            ElasticsearchHelper.DeleteIndex("genres");
            string mesaj = "<script language='javascript' type='text/javascript'>alert('Silme İşlemi Başarıyla Gerçekleşmiştir!');window.location.href = '/home/index/';</script>";
            return Content(mesaj);
        }



        // GET: Genre/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Genre genre = db.Genre.Find(id);
            if (genre == null)
            {
                return HttpNotFound();
            }
            return View(genre);
        }

        // GET: Genre/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Genre/Create
        // Aşırı gönderim saldırılarından korunmak için, lütfen bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için https://go.microsoft.com/fwlink/?LinkId=317598 sayfasına bakın.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                db.Genre.Add(genre);
                db.SaveChanges();
                ElasticsearchHelper.EsClient().Index<Genre>(genre,
                  id => id.Index("genres")
              .Type(TypeName.From<Genre>())
                                     .Id(genre.Id)
                                     .Refresh(Elasticsearch.Net.Refresh.True));
                return RedirectToAction("Index");
            }

            return View(genre);
        }

        // GET: Genre/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Genre genre = db.Genre.Find(id);
            if (genre == null)
            {
                return HttpNotFound();
            }
            return View(genre);
        }

        // POST: Genre/Edit/5
        // Aşırı gönderim saldırılarından korunmak için, lütfen bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için https://go.microsoft.com/fwlink/?LinkId=317598 sayfasına bakın.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                db.Entry(genre).State = EntityState.Modified;
                db.SaveChanges();

                var response = ElasticsearchHelper.EsClient().Update<Genre, Genre>(genre.Id, d => d
                     .Index("genres")
                     .Type("genre")
                     .Refresh(Elasticsearch.Net.Refresh.True)
                     .Doc(new Genre
                     {
                         Id = genre.Id,
                         Name = genre.Name,
                         Description = genre.Description
                     }));
                return RedirectToAction("edit");
            }
            return View(genre);
        }

        // GET: Genre/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Genre genre = db.Genre.Find(id);
            if (genre == null)
            {
                return HttpNotFound();
            }
            return View(genre);
        }

        // POST: Genre/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Genre genre = db.Genre.Find(id);
            db.Genre.Remove(genre);
            db.SaveChanges();
            var response = ElasticsearchHelper.EsClient().Delete<Genre>(id, d => d
               .Index("genres")
               .Type("genre")
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
