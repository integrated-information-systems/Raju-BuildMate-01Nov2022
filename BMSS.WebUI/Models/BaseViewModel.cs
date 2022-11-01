using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSS.WebUI.Models
{
    abstract public class BaseViewModel
    {
        public bool IsModelValid { get; set; } = true;
        public List<string> ModelErrList { get; set; }
    }
}