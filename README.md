# Create Asp.Net Web Project Using Elasticsearch and SqlServer
Extending ASP.NET MVC Movie Store with elasticsearch


# How to make ASP.NET MVC Movie app using Elasticsearch and Entityframework

You can download and install the setup file for Elasticsearch here http://ruilopes.com/elasticsearch-setup/

After Setup.exe is installed, you can open the run command line with "Windows + R" in the Services section of Windows and type "services.msc" command.

After that, you can start the Elasticsearch Service with the following url and test with http://localhost:9200/
The screen image will be this way.

    {
        "name" : "aV0rXdK",
        "cluster_name" : "elasticsearch",
        "cluster_uuid" : "MVjT7pG4TNqsq-ImqgR0kA",
        "version" : {
        "number" : "6.3.2",
        "build_flavor" : "oss",
        "build_type" : "zip",
        "build_hash" : "053779d",
        "build_date" : "2018-07-20T05:20:23.451332Z",
        "build_snapshot" : false,
        "lucene_version" : "7.3.1",
        "minimum_wire_compatibility_version" : "5.6.0",
        "minimum_index_compatibility_version" : "5.0.0"
        },
        "tagline" : "You Know, for Search"
        }
Don't forget to edit migration settings and connectionstring settings in web config!

Now that I work with 2 tables, I have integrated two databases with 2 methods and integrated them into a button. but you do not need to delete the databases for you to open a new one.

The names of the databases will be movies and genres. You must first change their names from the ElasticsearchHelper class and then from the controllers.

# What's in the ElasticsearchHelper class?

1-There is a connection string in the EsClient method. The method you will always need when creating a new database.

    public static ElasticClient EsClient()
        {
            node = new Uri("http://localhost:9200/");
            connSettings = new ConnectionSettings(node);
            client = new ElasticClient(connSettings);
            return client;
        }
    
2- This method deletes the database.

     public static IDeleteIndexResponse DeleteIndex(string indexName)
        {
            return EsClient().DeleteIndex(indexName);
        }

3-Database creation methods with CreateMovieIndex and CreateGenreIndex

    public static void CreateMovieIndex()
        {
            string indexName = "movies";
            string aliasName = "my_movie";
            var indexSettings = new IndexSettings();
            indexSettings.NumberOfReplicas = 1;
            indexSettings.NumberOfShards = 3;
            var createIndexDescriptor = new CreateIndexDescriptor(indexName)
            .Mappings(ms => ms.Map<Movie>
                (m => m.AutoMap()))
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
            .Mappings(ms => ms.Map<Genre>
                (m => m.AutoMap()))
                .InitializeUsing(new IndexState() { Settings = indexSettings })
                .Aliases(a => a.Alias(aliasName));
                ElasticsearchHelper.EsClient().CreateIndex(createIndexDescriptor);
         }
  
  
4-Movie and Genre controllers contain the standard methods of entityframework.

5-Get all data

     var response = ElasticsearchHelper.EsClient().Search(movie)(s => s
        .Index("movies")
        .Type("movie")
        .From(0).Size(1000)
        .Source(a => a.IncludeAll())
        .Query(a => a.MatchAll())
        )
    
6-Edit a data

     var response = ElasticsearchHelper.EsClient().Update(Movie, Movie)
            (movie.Id, d => d
            .Index("movies")
            .Type("movie")
            .Doc(new Movie
            {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Description,
            GenreId = movie.GenreId,
            CreatedDate = DateTime.Now
            }));
        
7-Insert a data

    ElasticsearchHelper.EsClient().Index(Movie)
            (movie,
            id => id.Index("movies")
            .Type(TypeName.From(Movie)())
                .Id(movie.Id)
                .Refresh(Elasticsearch.Net.Refresh.True));
            
            
8-Delete a data

    var response = ElasticsearchHelper.EsClient().Delete(Movie)
            (id, d => d
            .Index("movies")
            .Type("movie"));
        

# Asp.Net MVC Projesinde Sql Server ve Elasticsearch Veritabanları Kullanılması
ASP.NET MVC Movie Store'u elasticsearch ile genişletme


# Elasticsearch ve entityframework kullanarak ASP.NET MVC Movie uygulaması yapma

Elasticsearch ile ilgili setup dosyasını buradan indirip kurabilirsiniz http://ruilopes.com/elasticsearch-setup/

Setup.exe kurulduktan sonra, Windows'un servisler kısmına, "Windows+R" ile run command satırını açıp, "services.msc" komutunu yazıp ulaşabilirsiniz.

Bundan sonra aşağıdaki url ile Elasticsearch Servisini startlayıp ve http://localhost:9200/ ile test edebilirsiniz.

Ekran görüntüsü bu şekilde olacaktır.

            {
            "name" : "aV0rXdK",
            "cluster_name" : "elasticsearch",
            "cluster_uuid" : "MVjT7pG4TNqsq-ImqgR0kA",
            "version" : {
            "number" : "6.3.2",
            "build_flavor" : "oss",
            "build_type" : "zip",
            "build_hash" : "053779d",
            "build_date" : "2018-07-20T05:20:23.451332Z",
            "build_snapshot" : false,
            "lucene_version" : "7.3.1",
            "minimum_wire_compatibility_version" : "5.6.0",
            "minimum_index_compatibility_version" : "5.0.0"
            },
            "tagline" : "You Know, for Search"
            }
            
Gerekli migration ve web configdeki connectionstring ayarlarını düzeltmeyi unutmayınız!

Şuan 2 tablo ile çalıştığım için 2 adet metod ile oluşmuş veritabanlarını sildirmeyi birer buttona entegre ettim. Ama siz sıfırdan açacağınız için veritabanlarını silmenize gerek yok.

Oluşacak veritabanlarının isimleri movies ve genres olacak. Bunların isimlerini öncelikle ElasticsearchHelper classından daha sonra da controllerlardan değiştirmelisiniz.

# ElasticsearchHelper classının içinde neler var?
1-EsClient metodunun içinde connection string var. Yeni bir veritabanı oluştururken her zaman ihtiyaç duyacağınız metodumuz.

      public static ElasticClient EsClient()
        {
            node = new Uri("http://localhost:9200/");
            connSettings = new ConnectionSettings(node);
            client = new ElasticClient(connSettings);
            return client;
        }

2-Veritabanı silme metodu

      public static IDeleteIndexResponse DeleteIndex(string indexName)
        {
            return EsClient().DeleteIndex(indexName);
        }
    
3-CreateMovieIndex ve CreateGenreIndex ile veritabanı oluşturma metodları

      public static void CreateMovieIndex()
        {
            string indexName = "movies";
            string aliasName = "my_movie";
            var indexSettings = new IndexSettings();
            indexSettings.NumberOfReplicas = 1;
            indexSettings.NumberOfShards = 3;
            var createIndexDescriptor = new CreateIndexDescriptor(indexName)
            .Mappings(ms => ms.Map<Movie>
                (m => m.AutoMap()))
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
            .Mappings(ms => ms.Map<Genre>
                (m => m.AutoMap()))
                .InitializeUsing(new IndexState() { Settings = indexSettings })
                .Aliases(a => a.Alias(aliasName));
                ElasticsearchHelper.EsClient().CreateIndex(createIndexDescriptor);
         }
  
 4-Movie ve Genre controllerları içinde entityframeworkün standart metodlarını barındırıyor.
 
 5-Veritabanındaki verinin listesini getirme
 
      var response = ElasticsearchHelper.EsClient().Search(movie)(s => s
        .Index("movies")
        .Type("movie")
        .From(0).Size(1000)
        .Source(a => a.IncludeAll())
        .Query(a => a.MatchAll())
        )

  6-Veritabanındaki veriyi edit etme
  
      var response = ElasticsearchHelper.EsClient().Update(Movie, Movie)
            (movie.Id, d => d
            .Index("movies")
            .Type("movie")
            .Doc(new Movie
            {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Description,
            GenreId = movie.GenreId,
            CreatedDate = DateTime.Now
            }));
        
   7-Veritabanına veri insert etme
   
       ElasticsearchHelper.EsClient().Index(Movie)
            (movie,
            id => id.Index("movies")
            .Type(TypeName.From(Movie)())
                .Id(movie.Id)
                .Refresh(Elasticsearch.Net.Refresh.True));
            
   8-Veritabanından veri silme
   
       var response = ElasticsearchHelper.EsClient().Delete(Movie)
            (id, d => d
            .Index("movies")
            .Type("movie"));
