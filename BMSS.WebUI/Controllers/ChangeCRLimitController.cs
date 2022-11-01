using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.ChangeCRLimitViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Change Credit Limit")]
    public class ChangeCRLimitController : Controller
    {
        //Repositories
        private I_OCRD_Repository i_OCRD_Repository;
        private I_ChangeCRLimit_Repository i_ChangeCRLimit_Repostory;
        private I_DODocH_Repository i_DODocH_Repository;
        IMapper _mapper;
        public ChangeCRLimitController(I_DODocH_Repository i_DODocH_Repository, I_OCRD_Repository i_OCRD_Repository, IMapper _mapper, I_ChangeCRLimit_Repository i_ChangeCRLimit_Repostory)
        {
            this.i_OCRD_Repository = i_OCRD_Repository;
            this._mapper = _mapper;
            this.i_ChangeCRLimit_Repostory = i_ChangeCRLimit_Repostory;
            this.i_DODocH_Repository = i_DODocH_Repository;
        }
        // GET: ChangeCRLimit
        public ActionResult Index()
        {
            return View(new ChangeCRLimitViewModel());
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetCustomerCreditDetails(string CardCode)
        {
            if (CardCode != null)
            {

                var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(CardCode);
                ChangeCRLimit CRL = i_ChangeCRLimit_Repostory.GetChangeCRLimitDetails(CardCode);
                decimal TotalDocBalance = i_DODocH_Repository.GetTotalSystemBalanceByCardCode(CardCode);
                decimal NewLimit = 0;
                if (CRL != null)
                {
                    NewLimit = CRL.NewLimit;
                }
                var ResultObject = new
                {

                    ResultCustomerObject.CardCode,
                    ResultCustomerObject.CardName,
                    Balance = TotalDocBalance,
                    ResultCustomerObject.CreditLine,
                    NewLimit
                };
                return Json(ResultObject, JsonRequestBehavior.DenyGet);
            }
            else
            {
                var ResultObject = new
                {
                    CardCode = "",
                    CardName = "",
                    Balance = 0,
                    CreditLine = 0,
                    NewLimit = 0
                };
                return Json(ResultObject, JsonRequestBehavior.DenyGet);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        public JsonResult Add(ChangeCRLimitViewModel CRLObj)
        {
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();

            if (ModelState.IsValid && ErrList.Count() <= 0)
            {
                ChangeCRLimit ChangeCRL = new ChangeCRLimit();
                ChangeCRL = _mapper.Map<ChangeCRLimitViewModel, ChangeCRLimit>(CRLObj);
                ChangeCRL.CreatedBy = User.Identity.Name;

                if (!i_ChangeCRLimit_Repostory.ChangeCreditLimit(ChangeCRL))
                {
                    ErrList.Add("There is some Problem in Saving, Please contact Web Admin");
                }
            }

            if (ModelState.IsValid == false || ErrList.Count() > 0)
            {
                CRLObj.IsModelValid = ModelState.IsValid;
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
                CRLObj.ModelErrList = new List<string>();
                if (ModelErrList.Count() > 0)
                {

                    foreach (var entry in ModelErrList)
                    {
                        CRLObj.ModelErrList.Add(entry);
                    }
                }
                if (ErrList.Count() > 0)
                {
                    CRLObj.IsModelValid = false;
                    foreach (var entry in ErrList)
                    {
                        CRLObj.ModelErrList.Add(entry);
                    }
                }
            }

            return Json(CRLObj, JsonRequestBehavior.DenyGet);
        }

    }
}