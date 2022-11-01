using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BMSS.WebUI.Infrastructure;
using BMSS.WebUI.Models.AccountViewModels;
using BMSS.WebUI.Models.UserIdentity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BMSS.WebUI.Infrastructure.Identity;
using System.Collections.Generic;

namespace BMSS.WebUI.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await UserManager.FindAsync(details.Name,
                details.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid name or password.");
                }
                else
                {
                    var claims = new List<Claim>();

                    ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user,
                    DefaultAuthenticationTypes.ApplicationCookie);

                    claims.Add(new Claim("PreferredLocation", user.preferredLocation));
                    claims.Add(new Claim("PreferredCashSalesCustomer", user.preferredCashSalesCustomer));
                    ident.AddClaims(claims);
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, ident);
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(details);
        }
        [Authorize]
        [HttpPost]      
        public ActionResult Logout()
        {

            AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }
    }
}