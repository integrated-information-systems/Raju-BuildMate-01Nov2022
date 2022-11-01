using BMSS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSS.WebUI.Models.POViewModels
{
    public class POListFilterViewModel
    {
        [Required]
        [DisplayName("Status")]
        public POFilter Status { get; set; }
        public IEnumerable<PODocH> POList { get; set; }
        public List<SelectListItem> Statuses { get; set; }
    }
    public enum POFilter
    {
        All,
        Open,
        Closed
    }
}