using BMSS.Domain.Entities;
using System.Data.Entity;

namespace BMSS.Domain.Concrete.SAP
{
    class EFSapDbContext : DbContext
    {
        // DB First Approach 
        public EFSapDbContext() : base("EFSapDb") // EFSapDbContext - specified here is a connection string name
        {
            // DB First Approach mentioned this line
            Database.SetInitializer<EFSapDbContext>(null);
        }
        // DB First Approach 

        //DbSet          
        public DbSet<OITM> Items { get; set; }
        public DbSet<OITB> ItemGroups { get; set; }
        public DbSet<OWHS> Warehouses { get; set; }
        public DbSet<OITW> WarehouseStocks { get; set; }
        public DbSet<OINV> InvoiceHeaders { get; set; }
        public DbSet<INV1> InvoiceLines { get; set; }
        public DbSet<OPDN> GRPOHeaders { get; set; }
        public DbSet<PDN1> GRPOLines { get; set; }
        public DbSet<OPOR> POHeaders { get; set; }
        public DbSet<POR1> POLines { get; set; }
        public DbSet<OPCH> APInvoiceHeaders { get; set; }
        public DbSet<PCH1> APInvoiceLines { get; set; }
        public DbSet<OPLN> PriceLists { get; set; }
        public DbSet<ITM1> ItemPrices { get; set; }
        public DbSet<SPP1> ItemSpecialPriceMasters { get; set; }
        public DbSet<SPP2> ItemSpecialPrices { get; set; }
        public DbSet<OCRD> Customers { get; set; }
        public DbSet<CRD1> CustomerAddresses { get; set; }
        public DbSet<OSLP> SalesPersons { get; set; }
        public DbSet<OCRG> CustomerGroups { get; set; }
        public DbSet<OCPR> ContactPersons { get; set; }
        public DbSet<OCTG> PaymentTerms { get; set; }
        public DbSet<ORTT> ExchangeRates { get; set; }
        public DbSet<OVTG> TaxCodes { get; set; }
        public DbSet<OADM> CompanyDefaults { get; set; }
        public DbSet<ORPC> APCreditMemos { get; set; }
        public DbSet<ORIN> CreditMemos { get; set; }
        public DbSet<OINM> Transactions { get; set; }
        public DbSet<OACT> GLAccounts { get; set; }
        public DbSet<OCRY> CountryCodes { get; set; }
        public DbSet<OCRC> CreditCards { get; set; }
        public DbSet<ITT1> ChildItems { get; set; }
        public DbSet<OITT> ProductTrees { get; set; }

        public DbSet<ORCT> IncomingPayments { get; set; }

        public DbSet<NNM1> SeriesPrefixes { get; set; }
        protected override void OnModelCreating(DbModelBuilder model_builder)
        {
            // Fluent API
            base.OnModelCreating(model_builder);
          
        }
        

    
    }
}
