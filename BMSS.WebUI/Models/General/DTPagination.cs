using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSS.WebUI.Models.General
{
    public class DTPagination
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }

        public search search { get; set; }

        public Order[] order { get; set; }
    }
    public class search
    {
        public string value { get; set; }
    }
    public class Order
    {
        public string column { get; set; }
        public string dir { get; set; }
    }
}