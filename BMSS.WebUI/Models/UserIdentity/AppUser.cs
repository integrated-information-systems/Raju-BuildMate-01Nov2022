using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace BMSS.WebUI.Models.UserIdentity
{
    public static class IdentityExtensions
    {
        public static string GetpreferredLocation(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("PreferredLocation");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;


        }
    }
    public class AppUser : IdentityUser
    {
        // additional properties will go here
        public string preferredLocation { get; set; }
        public string preferredSalesPerson { get; set; }
        public string preferredCashSalesCustomer{ get; set; }

        public async  Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("preferredLocation", this.preferredLocation));
            userIdentity.AddClaim(new Claim("preferredCashSalesCustomer", this.preferredCashSalesCustomer));
            userIdentity.AddClaim(new Claim("preferredSalesPerson", this.preferredSalesPerson));
            return userIdentity;
        }
    }

    
}