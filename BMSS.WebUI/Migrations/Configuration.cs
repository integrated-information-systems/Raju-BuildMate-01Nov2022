namespace BMSS.WebUI.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<BMSS.WebUI.Infrastructure.Identity.AppIdentityDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "BMSS.WebUI.Infrastructure.Identity.AppIdentityDbContext";
        }

        protected override void Seed(BMSS.WebUI.Infrastructure.Identity.AppIdentityDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
