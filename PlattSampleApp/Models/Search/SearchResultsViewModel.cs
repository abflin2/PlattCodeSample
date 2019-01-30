using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlattSampleApp.Models
{
    public class SearchResultsViewModel<T>
    {

        public List<T> SearchResults { get; set; } = new List<T>();

        public string OriginalSearchTerm { get; set; }
    }
}