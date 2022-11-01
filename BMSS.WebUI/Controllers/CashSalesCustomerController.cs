using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.Customer;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize]
    public class CashSalesCustomerController : Controller
    {
        //Repositories
        private I_CashSalesCustomer_Repository i_CashSalesCustomer_Repository;
        private I_OSLP_Repository i_OSLP_Repository;
        IMapper _mapper;
        public CashSalesCustomerController(I_CashSalesCustomer_Repository i_CashSalesCustomer_Repository, IMapper _mapper, I_OSLP_Repository i_OSLP_Repository)
        {
            this.i_CashSalesCustomer_Repository = i_CashSalesCustomer_Repository;
            this.i_OSLP_Repository = i_OSLP_Repository;
            this._mapper = _mapper;
        }
        // GET: CashSalesCustomer
        public ActionResult Index()
        {
            return View(new CashCustomerViewModel() { DocEntry=0 });
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetCashSalesCustomerList()
        {
            //Stock qty by location
            IEnumerable<CashSalesCustomerMaster> CashSalesCustomerList = i_CashSalesCustomer_Repository.CashSalesCustomerList;
            IEnumerable<CashCustomerListViewModel> CSCustomerList = CashSalesCustomerList.Select(e => new CashCustomerListViewModel
            {
                DocEntry = e.DocEntry,
                CustomerID = e.CustomerID,
                CustomerName = e.CustomerName,
                SalesPerson = e.SlpName,
                CreatedOn = e.CreatedOn.ToString("dd'/'MM'/'yyyy")
        }).ToList();
            return Json(CSCustomerList, JsonRequestBehavior.DenyGet);
        }
        [HttpGet]
        [AjaxOnly]
        public JsonResult GetCustomers()
        {
             
            var ResultObject = i_CashSalesCustomer_Repository.CashSalesCustomerList.Select(e => new SelectListItem
            {
                Text = e.CustomerName,
                Value = e.CustomerID
            }).ToList();
            return Json(ResultObject, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult Edit(long DocEntry)
        {
            CashSalesCustomerMaster CashSalesCustomerObj = i_CashSalesCustomer_Repository.GetByDocEntry(DocEntry);
            CashCustomerViewModel CashSalesCustomer = new CashCustomerViewModel();
            CashSalesCustomer = _mapper.Map<CashSalesCustomerMaster, CashCustomerViewModel>(CashSalesCustomerObj);
            return Json(CashSalesCustomer, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetCustomerDetails(string CustomerID)
        {

            if (CustomerID != null)
            {

                var ResultCustomerObject = i_CashSalesCustomer_Repository.GetByTelephoneNo(CustomerID);


                var ResultObject = new
                {
                    ResultCustomerObject.CustomerName,
                    ResultCustomerObject.AddressLine1,
                    ResultCustomerObject.AddressLine2,
                    ResultCustomerObject.AddressLine3,
                    ResultCustomerObject.AddressLine4,
                    ResultCustomerObject.SlpCode,

                };
                return Json(ResultObject, JsonRequestBehavior.DenyGet);
            }
            else
            {
                var ResultObject = new
                {
                    CustomerName = "",
                    AddressLine1 = "",
                    AddressLine2 = "",
                    AddressLine3 = "",
                    AddressLine4 = "",
                    SlpCode = "",
                };
                return Json(ResultObject, JsonRequestBehavior.DenyGet);
            }


        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult Delete(long DocEntry)
        {
            if(!i_CashSalesCustomer_Repository.DeleteByDocEntry(DocEntry))
            {

            }
            IEnumerable<CashSalesCustomerMaster> CashSalesCustomerList = i_CashSalesCustomer_Repository.CashSalesCustomerList;
            return Json(CashSalesCustomerList, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        public JsonResult Add(CashCustomerViewModel CSCObj)
        {
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();

            if (ModelState.IsValid && ErrList.Count() <= 0)
            {
                CashSalesCustomerMaster CashSalesCustomer = new CashSalesCustomerMaster();
                CashSalesCustomer = _mapper.Map<CashCustomerViewModel, CashSalesCustomerMaster>(CSCObj);
                CashSalesCustomer.CreatedBy = User.Identity.Name;
                CashSalesCustomer.SlpName = i_OSLP_Repository.GetSalesPersonName(CSCObj.SlpCode);
                var CashCustomerObject = i_CashSalesCustomer_Repository.GetByTelephoneNo(CSCObj.CustomerID);
                if (CashCustomerObject!=null && CSCObj.DocEntry==0)
                {
                    ErrList.Add("Contact No already tagged with a customer, cannot add New");
                }            
                else if (!i_CashSalesCustomer_Repository.AddUpdateCashSalesCustomer(CashSalesCustomer))
                {
                    ErrList.Add("There is some Problem in Saving, Please contact Web Admin");
                }
            }

            if (ModelState.IsValid == false || ErrList.Count() > 0)
            {
                CSCObj.IsModelValid = ModelState.IsValid;
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
                CSCObj.ModelErrList = new List<string>();
                    if (ModelErrList.Count() > 0)
                    {

                        foreach (var entry in ModelErrList)
                        {
                        CSCObj.ModelErrList.Add(entry);
                        }
                    }
                if (ErrList.Count() > 0)
                {
                    CSCObj.IsModelValid = false;
                    foreach (var entry in ErrList)
                    {
                        CSCObj.ModelErrList.Add(entry);
                    }
                }
            }

            return Json(CSCObj, JsonRequestBehavior.DenyGet);
        }
    }
}