using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetConf2018.Services.Models
{


    public class Paths
    {
       public List<ItemPath> data { get; set; }
    }

    public class ItemPath
    {
        public string Name { get; set; }
        public string Signature { get; set; }
    }
}

