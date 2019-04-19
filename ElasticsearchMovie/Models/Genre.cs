using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElasticsearchMovie.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //The below attribute excludes the Movie
        //property from JSON serialization, preventing
        //circular references when serializing Movie.
        [JsonIgnore]
        public ICollection<Movie> Movies { get; set; }
    }
}