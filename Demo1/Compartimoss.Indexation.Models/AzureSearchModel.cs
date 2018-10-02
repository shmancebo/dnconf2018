using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;

namespace Compartimoss.Indexation.Models
{
    public class AzureSearchModel
    {
        static int NextId = 0;
        [IsRetrievable(true), IsSearchable, IsSortable]
        public string Category { get; set; }

        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; private set; }


        [IsRetrievable(true), IsSearchable, IsSortable]
        public string Entity { get; set; }

        [IsRetrievable(true)]
        public bool Winner { get; set; }

        [IsRetrievable(true), IsSearchable, IsSortable]
        public string Year { get; set; }

        public AzureSearchModel()
        {
            this.Id = NextId++.ToString();
        }

    }

}
