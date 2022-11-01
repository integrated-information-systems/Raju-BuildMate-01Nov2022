using BMSS.WebUI.Migrations;
using BMSS.WebUI.Models.UserIdentity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;

namespace BMSS.WebUI.Infrastructure.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext() : base("UserIdentityDb") {
            
    }

        static AppIdentityDbContext()
        {
            Database.SetInitializer<AppIdentityDbContext>(new IdentityDbInit());
        }
        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }
    public class IdentityDbInit :  MigrateDatabaseToLatestVersion<AppIdentityDbContext, Configuration>
    {
        public override void InitializeDatabase(AppIdentityDbContext context)
        {
            base.InitializeDatabase(context);
            PerformInitialSetup(context);
        }
        public void PerformInitialSetup(AppIdentityDbContext context)
        {
           

            AppUserManager userMgr = new AppUserManager(new UserStore<AppUser>(context));
            AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));
            string roleName = "Administrator";
            string userName = "Admin";
            string password = "Admin@123";
            string email = "admin@iis.com";
            List<string> roleList = new List<string>
            {
                "Administrator",
                "Cash Sales",
                "Cash Sales Credit Note",
                "Cash Sales Credit Note-Edit",
                "Cash Sales-Edit",
                "Customers",
                "Change Credit Limit",
                "Delivery Order/Invoice",
                "Delivery Order/Invoice-Edit",
                "Document Numbering",
                "Notes",
                "Payment",
                "Payment-Edit",
                "Print After First Time",
                "Purchase Delivery Note",
                "Purchase Delivery Note-Edit",
                "Purchase Order",
                "Purchase Order-Edit",
                "Purchase Order(SAP)",
                "Purchase Quotation",
                "Purchase Quotation-Edit",
                "Sales Quotation",
                "Sales Quotation-Edit",
                "Stock Issue",
                "Stock Issue-Edit",
                "Stock Loan",
                "Stock Loan-Edit",
                "Stock Receipt",
                "Stock Receipt-Edit",
                "Stock Transfer",
                "Stock Transfer-Edit",
                "Stocks",
                "Submit To SAP",
                "Suppliers",               
            };
            foreach (var item in roleList)
            {
                if (!roleMgr.RoleExists(item))
                {
                    roleMgr.Create(new AppRole(item));
                }
            }

           
            AppUser user = userMgr.FindByName(userName);
            if (user == null)
            {
                userMgr.Create(new AppUser { UserName = userName, Email = email },
                password);
                user = userMgr.FindByName(userName);
            }
            if (!userMgr.IsInRole(user.Id, roleName))
            {
                userMgr.AddToRole(user.Id, roleName);
            }

        }
    }
}