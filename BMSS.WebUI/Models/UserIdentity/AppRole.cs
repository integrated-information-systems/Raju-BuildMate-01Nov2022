using Microsoft.AspNet.Identity.EntityFramework;

namespace BMSS.WebUI.Models.UserIdentity
{
    public class AppRole : IdentityRole
    {
        public AppRole() : base() { }
        public AppRole(string name) : base(name) { }
    }
}