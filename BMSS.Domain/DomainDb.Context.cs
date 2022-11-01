﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BMSS.Domain
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DomainDb : DbContext
    {
        public DomainDb()
            : base("name=DomainDb")
        {
            Database.SetInitializer<DomainDb>(null);
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CashSalesCreditDocLs> CashSalesCreditDocLs { get; set; }
        public virtual DbSet<CashSalesCreditDocNotes> CashSalesCreditDocNotes { get; set; }
        public virtual DbSet<CashSalesCustomerMaster> CashSalesCustomerMaster { get; set; }
        public virtual DbSet<CashSalesDocLs> CashSalesDocLs { get; set; }
        public virtual DbSet<CashSalesDocNotes> CashSalesDocNotes { get; set; }
        public virtual DbSet<ChangeCRLimit> ChangeCRLimit { get; set; }
        public virtual DbSet<CNotesAll> CNotesAll { get; set; }
        public virtual DbSet<CNotesMngt> CNotesMngt { get; set; }
        public virtual DbSet<DODocLs> DODocLs { get; set; }
        public virtual DbSet<DODocNotes> DODocNotes { get; set; }
        public virtual DbSet<DODocStockLoan> DODocStockLoan { get; set; }
        public virtual DbSet<GRPODocLs> GRPODocLs { get; set; }
        public virtual DbSet<GRPODocNotes> GRPODocNotes { get; set; }
        public virtual DbSet<INotesAll> INotesAll { get; set; }
        public virtual DbSet<INotesMngt> INotesMngt { get; set; }
        public virtual DbSet<NumberingAlPay> NumberingAlPay { get; set; }
        public virtual DbSet<NumberingCrSale> NumberingCrSale { get; set; }
        public virtual DbSet<NumberingCSale> NumberingCSale { get; set; }
        public virtual DbSet<NumberingDO> NumberingDO { get; set; }
        public virtual DbSet<NumberingGRN> NumberingGRN { get; set; }
        public virtual DbSet<NumberingPay> NumberingPay { get; set; }
        public virtual DbSet<NumberingPO> NumberingPO { get; set; }
        public virtual DbSet<NumberingSI> NumberingSI { get; set; }
        public virtual DbSet<NumberingSL> NumberingSL { get; set; }
        public virtual DbSet<NumberingSQ> NumberingSQ { get; set; }
        public virtual DbSet<NumberingSR> NumberingSR { get; set; }
        public virtual DbSet<NumberingST> NumberingST { get; set; }
        public virtual DbSet<PaymentDocNotes> PaymentDocNotes { get; set; }
        public virtual DbSet<SNotesAll> SNotesAll { get; set; }
        public virtual DbSet<SNotesMngt> SNotesMngt { get; set; }
        public virtual DbSet<SQDocNotes> SQDocNotes { get; set; }
        public virtual DbSet<StockIssueDocLs> StockIssueDocLs { get; set; }
        public virtual DbSet<StockIssueDocNotes> StockIssueDocNotes { get; set; }
        public virtual DbSet<StockReceiptDocLs> StockReceiptDocLs { get; set; }
        public virtual DbSet<StockReceiptDocNotes> StockReceiptDocNotes { get; set; }
        public virtual DbSet<StockTransDocLs> StockTransDocLs { get; set; }
        public virtual DbSet<StockTransDocNotes> StockTransDocNotes { get; set; }
        public virtual DbSet<SyncErrLog> SyncErrLog { get; set; }
        public virtual DbSet<SyncLog> SyncLog { get; set; }
        public virtual DbSet<PODocNotes> PODocNotes { get; set; }
        public virtual DbSet<POPDNs> POPDNs { get; set; }
        public virtual DbSet<NumberingPQ> NumberingPQ { get; set; }
        public virtual DbSet<PQDocNotes> PQDocNotes { get; set; }
        public virtual DbSet<PQDocLs> PQDocLs { get; set; }
        public virtual DbSet<StockIssueDocH> StockIssueDocH { get; set; }
        public virtual DbSet<StockTransDocH> StockTransDocH { get; set; }
        public virtual DbSet<CashSalesCreditDocPays> CashSalesCreditDocPays { get; set; }
        public virtual DbSet<CashSalesDocPays> CashSalesDocPays { get; set; }
        public virtual DbSet<GRPODocH> GRPODocH { get; set; }
        public virtual DbSet<PODocH> PODocH { get; set; }
        public virtual DbSet<PQDocH> PQDocH { get; set; }
        public virtual DbSet<SQDocH> SQDocH { get; set; }
        public virtual DbSet<SQDocLs> SQDocLs { get; set; }
        public virtual DbSet<PaymentDocLs> PaymentDocLs { get; set; }
        public virtual DbSet<PODocLs> PODocLs { get; set; }
        public virtual DbSet<InventoryLog> InventoryLog { get; set; }
        public virtual DbSet<StockBalance> StockBalance { get; set; }
        public virtual DbSet<StockBalanceLog> StockBalanceLog { get; set; }
        public virtual DbSet<OpeningBalanceSetup> OpeningBalanceSetup { get; set; }
        public virtual DbSet<CashSalesCreditDocH> CashSalesCreditDocH { get; set; }
        public virtual DbSet<CashSalesDocH> CashSalesDocH { get; set; }
        public virtual DbSet<DODocH> DODocH { get; set; }
        public virtual DbSet<PaymentDocH> PaymentDocH { get; set; }
        public virtual DbSet<StockReceiptDocH> StockReceiptDocH { get; set; }
        public virtual DbSet<InvMovmentView> InvMovmentView { get; set; }
    }
}
