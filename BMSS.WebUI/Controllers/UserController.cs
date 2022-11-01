using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using BMSS.WebUI.Helpers.Functions;
using BMSS.WebUI.Helpers.HtmlHelpers;
using BMSS.WebUI.Infrastructure;
using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.UserIdentity;
using BMSS.WebUI.Models.UserViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BMSS.WebUI.Infrastructure.Identity;
using BMSS.Domain.Abstract.SAP;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : Controller
    {
        I_OCRD_Repository i_OCRD_Repository;
        I_OSLP_Repository i_OSLP_Repository;
        I_OWHS_Repository i_OWHS_Repository;
        public UserController(I_OCRD_Repository i_OCRD_Repository, I_OSLP_Repository i_OSLP_Repository, I_OWHS_Repository i_OWHS_Repository)
        {
            this.i_OCRD_Repository = i_OCRD_Repository;
            this.i_OSLP_Repository = i_OSLP_Repository;
            this.i_OWHS_Repository = i_OWHS_Repository;
        }
        private AjaxFormViewModel ajaxFormViewModel = new AjaxFormViewModel()
        { FormAddAction = "AddUser",
          AddActionTargetId = "UsrListTble",
          AddFormId = "UserAddForm",
          FormDeleteAction = "DeleteUser",
          DeleteActionTargetId = "UsrListTble",
          DeleteFormId = "UserDeleteForm",
          FormEditAction = "EditUser",
          EditActionTargetId = "UserAddUpdateFormContainer",
          EditFormId = "UserEditForm",
          FormUpdateAction ="UpdateUser",
          UpdateActionTargetId = "UsrListTble",
          UpdateFormId = "UpdateUserForm"
        };
        // GET: User
        public ActionResult Index()
        {
        
            ////Edit And Delete Ajax Options
            //AjaxFormViewModel ajaxFormViewModel = new AjaxFormViewModel() { FormAddAction = "AddUpdateUser", AddActionTargetId = "UsrListTble", AddFormId = "UserAddUpdateForm", FormDeleteAction = "DeleteUser", DeleteActionTargetId= "UsrListTble", DeleteFormId= "UserDeleteForm", FormEditAction = "EditUser", EditActionTargetId= "UserAddUpdateFormContainer", EditFormId= "UserEditForm" };

            CreateUserModel inputModel = new CreateUserModel() { AjaxOptions = ajaxFormViewModel };
            UserListModel ListModel = new UserListModel() { UserList = UserManager.Users.OrderBy(x=> x.UserName),  AjaxOptions = ajaxFormViewModel };
            List<SelectListItem> customers = i_OCRD_Repository.ActiveCustomers.Select(e => new SelectListItem
            {
                Text = e.CardName,
                Value = e.CardCode
            }).ToList();
            List<SelectListItem> salesPersons = i_OSLP_Repository.SalesPersons.Where(x => !x.SlpName.Equals("-No Sales Employee-")).Select(e => new SelectListItem
            {
                Text = e.SlpName,
                Value = e.SlpCode.ToString()
            }).ToList();
            List<SelectListItem> Warehouses = i_OWHS_Repository.Warehouses.Select(e => new SelectListItem
            {
                Text = e.WhsName,
                Value = e.WhsCode.ToString()
            }).ToList();
            ViewBag.UserList = ListModel;
            ViewBag.CustomerList = customers;
            ViewBag.WarehouseList = Warehouses;
            ViewBag.SalesPersonList = salesPersons;
            return View(inputModel);

        }
        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<JsonResult> UpdateUser(UpdateUserModel updateUserModel)
        {
            JsonResultViewModel jsonResultViewModel = new JsonResultViewModel() { };
            List<SelectListItem> customers = i_OCRD_Repository.ActiveCustomers.Select(e => new SelectListItem
            {
                Text = e.CardName,
                Value = e.CardCode
            }).ToList();
            List<SelectListItem> salesPersons = i_OSLP_Repository.SalesPersons.Where(x => !x.SlpName.Equals("-No Sales Employee-")).Select(e => new SelectListItem
            {
                Text = e.SlpName,
                Value = e.SlpCode.ToString()
            }).ToList();
            List<SelectListItem> Warehouses = i_OWHS_Repository.Warehouses.Select(e => new SelectListItem
            {
                Text = e.WhsName,
                Value = e.WhsCode.ToString()
            }).ToList();

            ViewBag.CustomerList = customers;
            ViewBag.SalesPersonList = salesPersons;
            ViewBag.WarehouseList = Warehouses;
            //jsonResultViewModel.IsResultDataTableOpertation = true;
            jsonResultViewModel.Opertation = OpertionType.UpdateRow;

            Dictionary<string, string> ModelErrList = new Dictionary<string, string>();
            List<string> ErrList = new List<string>();


            if (ModelState.IsValid)
            {
                AppUser foundAppUser = await UserManager.FindByIdAsync(updateUserModel.Id);
                if (foundAppUser != null)
                {
                    foundAppUser.preferredLocation = updateUserModel.PreferredLocation;
                    foundAppUser.preferredCashSalesCustomer = updateUserModel.PreferredCashCusomter;
                    foundAppUser.preferredSalesPerson = updateUserModel.PreferredSalesPerson;
                    foundAppUser.Email = updateUserModel.UpdatedEmail;

                    IdentityResult validEmail = await UserManager.UserValidator.ValidateAsync(foundAppUser);
                    if (!validEmail.Succeeded)
                    {
                        AddErrorsFromResultToList(validEmail, ref ErrList);
                    }
                    IdentityResult validPass = null;
                    if (updateUserModel.Password != null)
                    {
                        if (updateUserModel.Password != string.Empty)
                        {
                            validPass = await UserManager.PasswordValidator.ValidateAsync(updateUserModel.Password);
                            if (validPass.Succeeded)
                            {
                                foundAppUser.PasswordHash =
                                UserManager.PasswordHasher.HashPassword(updateUserModel.Password);
                            }
                            else
                            {
                                AddErrorsFromResultToList(validPass, ref ErrList);
                            }
                        }
                    }
                    if ((validEmail.Succeeded && validPass == null) || (validEmail.Succeeded
                    && updateUserModel.Password != string.Empty && validPass.Succeeded))
                    {
                        IdentityResult result = await UserManager.UpdateAsync(foundAppUser);
                        if (result.Succeeded)
                        {
                             
                        }
                        else
                        {
                            AddErrorsFromResultToList(result, ref ErrList);
                        }
                    }
                }
                else
                {
                    ErrList.Add("User not exist");
                }
               
            }

            //
            if (ModelErrList.Count > 0 || ErrList.Count > 0)
            {
                jsonResultViewModel.IsOpertationSuccess = false;
                foreach (KeyValuePair<string, string> entry in ModelErrList)
                {
                    ErrList.Add(entry.Value);
                }
                jsonResultViewModel.ErrList = ErrList;
                jsonResultViewModel.IsOpertationSuccess = false;
            }

            if (jsonResultViewModel.IsOpertationSuccess)
            {
                UserListModel ListModel = new UserListModel() { UserList = UserManager.Users.OrderBy(x=> x.UserName), AjaxOptions = ajaxFormViewModel };
                jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_UserList", ListModel));
               
                CreateUserModel inputModel = new CreateUserModel() { AjaxOptions = ajaxFormViewModel };
                jsonResultViewModel.ContentToLoadNewForm = PartialRender.RenderToString(PartialView("_AddUser", inputModel));               
            }
            else
            {
               
            }

            jsonResultViewModel.Ax = ajaxFormViewModel;
            return Json(jsonResultViewModel, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<JsonResult> AddUser(CreateUserModel createUserModel)
        {
            JsonResultViewModel jsonResultViewModel = new JsonResultViewModel() { };

            //jsonResultViewModel.IsResultDataTableOpertation = true;
            jsonResultViewModel.Opertation = OpertionType.AddRow;

            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();

            if (ModelState.IsValid)
            {
                AppUser newUser = new AppUser { UserName = createUserModel.Name, Email = createUserModel.Email, preferredLocation= createUserModel.PreferredLocation, preferredCashSalesCustomer = createUserModel.PreferredCashCusomter, preferredSalesPerson = createUserModel.PreferredSalesPerson };
                IdentityResult result = await UserManager.CreateAsync(newUser,
                       createUserModel.Password);
                if (result.Succeeded)
                {


                    //jsonResultViewModel.DataTableOpertation = DataTableOpertionType.AddRow;
                    //jsonResultViewModel.rowValues= new List<string>(new string[] { createUserModel.Name, createUserModel.Email, "","" });
                   
                }
               else
                {                   
                    AddErrorsFromResultToList(result, ref ErrList);                  
                }
            }
            else
            {
                
                ModelErrList = HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
               
            }

            //
            if (ModelErrList.Count()>0 || ErrList.Count>0)
            {
                jsonResultViewModel.IsOpertationSuccess = false;               
                foreach (var entry in ModelErrList)
                {
                    ErrList.Add(entry);
                }
                jsonResultViewModel.ErrList = ErrList;
                jsonResultViewModel.IsOpertationSuccess = false;
            }


            if (jsonResultViewModel.IsOpertationSuccess)
            {
                UserListModel ListModel = new UserListModel() { UserList = UserManager.Users.OrderBy(x=> x.UserName), AjaxOptions = ajaxFormViewModel };
                jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_UserList", ListModel));
            }          
            jsonResultViewModel.Ax = ajaxFormViewModel;


            return Json(jsonResultViewModel, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<JsonResult> DeleteUser(string UserID)
        {
            JsonResultViewModel jsonResultViewModel = new JsonResultViewModel() { };
            jsonResultViewModel.Opertation = OpertionType.DeleteRow;

           

            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();

            if (ModelState.IsValid)
            {
                AppUser user = await UserManager.FindByIdAsync(UserID);
                if (user != null)
                {
                    IdentityResult result = await UserManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {                       
                        
                    }
                    else
                    {                        
                        AddErrorsFromResultToList(result, ref ErrList);
                    }

                }
                else
                {
                    ErrList.Add("User not exist");
                }
            }
            else
            {               
                ErrList.Add("User not exist");
            }
            //
            if (ModelErrList.Count() > 0 || ErrList.Count > 0)
            {
                jsonResultViewModel.IsOpertationSuccess = false;
                foreach (var entry in ModelErrList)
                {
                    ErrList.Add(entry);
                }
                jsonResultViewModel.ErrList = ErrList;
                jsonResultViewModel.IsOpertationSuccess = false;
            }


            UserListModel ListModel = new UserListModel() { UserList = UserManager.Users.OrderBy(x=> x.UserName), AjaxOptions = ajaxFormViewModel };
            jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_UserList", ListModel));
            jsonResultViewModel.Ax = ajaxFormViewModel;


            return Json(jsonResultViewModel, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<JsonResult> EditUser(string UserID)
        {
            JsonResultViewModel jsonResultViewModel = new JsonResultViewModel() { };
            jsonResultViewModel.Opertation = OpertionType.EditRow;


            List<SelectListItem> customers = i_OCRD_Repository.ActiveCustomers.Select(e => new SelectListItem
            {
                Text = e.CardName,
                Value = e.CardCode
            }).ToList();

            List<SelectListItem> salesPersons = i_OSLP_Repository.SalesPersons.Where(x => !x.SlpName.Equals("-No Sales Employee-")).Select(e => new SelectListItem
            {
                Text = e.SlpName,
                Value = e.SlpCode.ToString()
            }).ToList();

            List<SelectListItem> Warehouses = i_OWHS_Repository.Warehouses.Select(e => new SelectListItem
            {
                Text = e.WhsName,
                Value = e.WhsCode.ToString()
            }).ToList();
            ViewBag.WarehouseList = Warehouses;
            ViewBag.CustomerList = customers;
            ViewBag.SalesPersonList = salesPersons;

            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();

            UpdateUserModel updateUser = null;

            if (ModelState.IsValid)
            {
                AppUser user = await UserManager.FindByIdAsync(UserID);
                if (user != null)
                {
                    updateUser = new UpdateUserModel { UpdatedEmail = user.Email, Name = user.UserName, Id = user.Id, PreferredLocation=user.preferredLocation, PreferredCashCusomter = user.preferredCashSalesCustomer, PreferredSalesPerson = user.preferredSalesPerson };

                }
                else
                {
                    ErrList.Add("User not exist");
                }
            }
            else
            {
                ErrList.Add("User not exist");
            }

            if (ModelErrList.Count() > 0 || ErrList.Count > 0)
            {
                jsonResultViewModel.IsOpertationSuccess = false;
                foreach (var entry in ModelErrList)
                {
                    ErrList.Add(entry);
                }
                jsonResultViewModel.ErrList = ErrList;
                jsonResultViewModel.IsOpertationSuccess = false;
            }

            if (jsonResultViewModel.IsOpertationSuccess)
            {
                updateUser.AjaxOptions = ajaxFormViewModel;
                jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_UpdateUser", updateUser));
                jsonResultViewModel.Ax = ajaxFormViewModel;
            }
            else
            {
                UserListModel ListModel = new UserListModel() { UserList = UserManager.Users.OrderBy(x=>x.UserName), AjaxOptions = ajaxFormViewModel };
                jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_UserList", ListModel));
                jsonResultViewModel.Ax = ajaxFormViewModel;
            }
           
            return Json(jsonResultViewModel, JsonRequestBehavior.DenyGet);
        }
        [NonAction]
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        [NonAction]
        private void AddErrorsFromResultToList(IdentityResult result, ref List<string> ErrList)
        {
            foreach (string error in result.Errors)
            {
                ErrList.Add(error);
            }
        }
    }
}