using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetConf2018.Services.Models
{
    public class AzureSearchModel
    {
        static int NextId = 0;
        [IsRetrievable(true), IsSearchable, IsSortable]
        public string Tag { get; set; }

        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; private set; }

        [IsRetrievable(true)]
        public string Scoring { get; set; }

        [IsRetrievable(true)]
        public string Url { get; set; }

        public AzureSearchModel()
        {
            this.Id = NextId++.ToString();
        }

    }
}
