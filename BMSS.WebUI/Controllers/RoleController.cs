using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using BMSS.WebUI.Helpers.Functions;
using BMSS.WebUI.Helpers.HtmlHelpers;
using BMSS.WebUI.Infrastructure;
using BMSS.WebUI.Infrastructure.Identity;
using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.RoleViewModels;
using BMSS.WebUI.Models.UserIdentity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RoleController : Controller
    {
        private AjaxFormViewModel ajaxFormViewModel = new AjaxFormViewModel()
        { FormAddAction = "AddRole",
          AddActionTargetId = "RleListTble",
          AddFormId = "RoleAddForm",
          FormDeleteAction = "DeleteRole",
          DeleteActionTargetId = "RleListTble",
          DeleteFormId = "RoleDeleteForm",
          FormEditAction = "",
          EditActionTargetId = "RoleAddUpdateFormContainer",
          EditFormId = "",
          FormUpdateAction = "",
          UpdateActionTargetId = "RleListTble",
          UpdateFormId = ""
        };

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        private AppRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();
            }
        }
        // GET: Role
        public ActionResult Index()
        {
            CreateRoleModel inputModel = new CreateRoleModel() { AjaxOptions = ajaxFormViewModel };
            RoleListModel ListModel = new RoleListModel() { RoleList = RoleManager.Roles.OrderBy(x => x.Name), AjaxOptions = ajaxFormViewModel };

            ViewBag.RoleList = ListModel;
            return View(inputModel);
        }        
        public async Task<ActionResult> AssignRoles()
        {
            List<UserRoleViewModel> UserRoleMap = new List<UserRoleViewModel>();
            IEnumerable<AppUser> Users = UserManager.Users.OrderBy(x => x.UserName);
            IEnumerable<AppRole> Roles = RoleManager.Roles.OrderBy(x => x.Name);
            foreach (AppUser usr in Users)
            {
                UserRoleViewModel Item = new UserRoleViewModel();
                Item.Username = usr.UserName;
                Item.RolesAssigned = new List<string>();
                foreach (AppRole rle in Roles)
                {

                    if (await UserManager.IsInRoleAsync(usr.Id, rle.Name))
                    {
                        Item.RolesAssigned.Add(rle.Name);
                    }
                }

                UserRoleMap.Add(Item);
            }
            ViewBag.Usernames = Users;
            ViewBag.Rolenames = Roles;
            
            return View(UserRoleMap);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<JsonResult> AssignRoles(AssignRoleViewModel model)
        {

            JsonResultViewModel jsonResultViewModel = new JsonResultViewModel() { };
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();


            IEnumerable<AppUser> Users = UserManager.Users.OrderBy(x => x.UserName);
            IEnumerable<AppRole> Roles = RoleManager.Roles.OrderBy(x => x.Name);

            if (ModelState.IsValid)
            {
                foreach (AppUser usr in Users)
                {
                    foreach (AppRole rle1 in Roles)
                    {
                        using (AppIdentityDbContext db = new AppIdentityDbContext())
                        {
                            AppUserManager UM = new AppUserManager(new UserStore<AppUser>(db));
                            if (await UM.IsInRoleAsync(usr.Id, rle1.Name))
                            {
                                IdentityResult result = await UM.RemoveFromRoleAsync(usr.Id, rle1.Name);
                            }
                        }
                    }
                    foreach (string UserRoleId in model.IdsToAdd ?? new string[] { })
                    {

                        if (UserRoleId.StartsWith(usr.Id))
                        {

                            string roleId = UserRoleId.Replace(usr.Id, "");
                            AppRole rle = Roles.Where(p => p.Id.Equals((object)roleId)).FirstOrDefault();
                            if (rle != null)
                            {
                                using (AppIdentityDbContext db = new AppIdentityDbContext())
                                {
                                    AppUserManager UM = new AppUserManager(new UserStore<AppUser>(db));
                                    if (!await UM.IsInRoleAsync(usr.Id, rle.Name))
                                    {
                                        IdentityResult result = await UM.AddToRoleAsync(usr.Id, rle.Name);
                                        if (result.Succeeded)
                                        {
                                        }
                                        else
                                        {
                                            AddErrorsFromResultToList(result, ref ErrList);
                                        }
                                    }
                                }
                            }
                            
                        }

                    }

                }
            }
            else
            {
                ModelErrList = HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
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

            return Json(jsonResultViewModel, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<JsonResult> AddRole(CreateRoleModel createRoleModel)
        {
            JsonResultViewModel jsonResultViewModel = new JsonResultViewModel() { };
            jsonResultViewModel.Opertation = OpertionType.AddRow;

            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();

            if (ModelState.IsValid)
            {
                IdentityResult result = await RoleManager.CreateAsync(new AppRole(createRoleModel.Name));
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
                ModelErrList = HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
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
                RoleListModel ListModel = new RoleListModel() { RoleList = RoleManager.Roles.OrderBy(x => x.Name), AjaxOptions = ajaxFormViewModel };
                jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_RoleList", ListModel));
            }

            jsonResultViewModel.Ax = ajaxFormViewModel;

            return Json(jsonResultViewModel, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<JsonResult> DeleteRole(string RoleID)
        {
            JsonResultViewModel jsonResultViewModel = new JsonResultViewModel() { };
            jsonResultViewModel.Opertation = OpertionType.DeleteRow;

            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();

            if (ModelState.IsValid)
            {
                AppRole role = await RoleManager.FindByIdAsync(RoleID);
                if (role != null)
                {
                    IdentityResult result = await RoleManager.DeleteAsync(role);
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
                    ErrList.Add("Role not exist");
                }
            }
            else
            {
                ErrList.Add("Role not exist");
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

            RoleListModel ListModel = new RoleListModel() { RoleList = RoleManager.Roles, AjaxOptions = ajaxFormViewModel };
            jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_RoleList", ListModel));

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