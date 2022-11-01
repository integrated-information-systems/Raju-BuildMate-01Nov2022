using BMSS.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    public class UnAuthorizedController : Controller
    {
        // GET: UnAuthorized     
        public JsonResult UnAuthoziedAjaxRequest()
        {

            var baseViewModel = new
            {
                IsModelValid = false,
                ModelErrList = new List<string>() { "Access Denied" }
            };           
            return Json(baseViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}