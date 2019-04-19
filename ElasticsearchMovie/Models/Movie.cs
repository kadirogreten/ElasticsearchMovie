using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElasticsearchMovie.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public int? GenreId { get; set; }
        public Genre Genre { get; set; }

    }
}