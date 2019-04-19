using ElasticsearchMovie.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElasticsearchMovie.ElasticHelper
{
    public class ElasticsearchHelper
    {

        public static Uri node;
        public static ConnectionSettings connSettings;
        public static ElasticClient client;

  

            

        public static ElasticClient EsClient()
        {


            node = new Uri("http://localhost:9200/");
            connSettings = new ConnectionSettings(node);

            client = new ElasticClient(connSettings);

            return client;
        }


        public static IDeleteIndexResponse DeleteIndex(string indexName)
        {
           return EsClient().DeleteIndex(indexName);
        }


        public static void CreateMovieIndex()
        {
            string indexName = "movies";
            string aliasName = "my_movie";

            var indexSettings = new IndexSettings();
            indexSettings.NumberOfReplicas = 1;
            indexSettings.NumberOfShards = 3;
            var createIndexDescriptor = new CreateIndexDescriptor(indexName)
                                            .Mappings(ms => ms.Map<Movie>(m => m.AutoMap()))
                                            .InitializeUsing(new IndexState() { Settings = indexSettings })
                                            .Aliases(a => a.Alias(aliasName));

            ElasticsearchHelper.EsClient().CreateIndex(createIndexDescriptor);
        }

        public static void CreateGenreIndex()
        {
            string indexName = "genres";
            string aliasName = "my_genre";

            var indexSettings = new IndexSettings();
            indexSettings.NumberOfReplicas = 1;
            indexSettings.NumberOfShards = 3;
            var createIndexDescriptor = new CreateIndexDescriptor(indexName)
                                            .Mappings(ms => ms.Map<Genre>(m => m.AutoMap()))
                                            .InitializeUsing(new IndexState() { Settings = indexSettings })
                                            .Aliases(a => a.Alias(aliasName));

            ElasticsearchHelper.EsClient().CreateIndex(createIndexDescriptor);
        }


    }
}