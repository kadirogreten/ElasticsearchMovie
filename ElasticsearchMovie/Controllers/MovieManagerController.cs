using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElasticsearchMovie.ElasticHelper;
using ElasticsearchMovie.Models;
using IdentitySample.Models;
using Nest;

namespace ElasticsearchMovie.Controllers
{
    public class MovieManagerController : Controller
    {

        ApplicationDbContext db;
        private static ISearchResponse<Movie> movies;
        private static ISearchResponse<Genre> genres;



        public MovieManagerController()
        {
            db = new ApplicationDbContext();
        }
        // GET: MovieManager
        public ActionResult Index()
        {
            //CreateNewIndex();
            //GetElasticMovies();
            //GetElasticGenre();
            ElasticsearchHelper.client.DeleteIndex("movies");
            ElasticsearchHelper.client.DeleteIndex("genres");
            return View();
        }

        private void GetElasticGenre()
        {
            var indexSettings = new IndexSettings();
            indexSettings.NumberOfReplicas = 1;
            indexSettings.NumberOfShards = 3;
            var createIndexDescriptor = new CreateIndexDescriptor("genres")
                                            .Mappings(ms => ms.Map<Genre>(m => m.AutoMap()))
                                            .InitializeUsing(new IndexState() { Settings = indexSettings })
                                            .Aliases(a => a.Alias("my_genre"));


            ElasticsearchHelper.EsClient().CreateIndex(createIndexDescriptor);
            try
            {
                var response = ElasticsearchHelper.EsClient().Search<Genre>(s => s
                                       .Index("genres")
                                       .Type("genre")
                                       .From(0)
                                       .Size(2000)
                                       .MatchAll());

                genres = response;
            }
            catch (Exception ex)
            {

                throw;
            }



            
           

            ViewBag.GenreData = genres.Documents;
        }

        private void GetElasticMovies()
        {

            var indexSettings = new IndexSettings();
            indexSettings.NumberOfReplicas = 1;
            indexSettings.NumberOfShards = 3;
            var createIndexDescriptor = new CreateIndexDescriptor("movies")
                                            .Mappings(ms => ms.Map<Movie>(m => m.AutoMap()))
                                            .InitializeUsing(new IndexState() { Settings = indexSettings })
                                            .Aliases(a => a.Alias("movies"));

            ElasticsearchHelper.EsClient().CreateIndex(createIndexDescriptor);
            try
            {


                var response = ElasticsearchHelper.EsClient().Search<Movie>(s => s
                 .Index("movies")
                 .Type("movie")
                 .From(0)
                 .Size(1000)
                 .Query(q => q.MatchAll()));

                movies = response;


            }
            catch (Exception ex)
            {

                throw;
            }

            ViewBag.MovieData = movies.Documents;

        }

    }


}
