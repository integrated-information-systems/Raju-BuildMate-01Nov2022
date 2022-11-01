using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.Planner;
using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Concrete;
using BMSS.Domain.Concrete.Planner;
using BMSS.Domain.Concrete.SAP;
using BMSS.WebUI.Models.CashSalesCreditViewModels;
using BMSS.WebUI.Models.CashSalesViewModels;
using BMSS.WebUI.Models.ChangeCRLimitViewModels;
using BMSS.WebUI.Models.Customer;
using BMSS.WebUI.Models.DocNumberingViewModels;
using BMSS.WebUI.Models.DOViewModels;
using BMSS.WebUI.Models.GRPOViewModels;
using BMSS.WebUI.Models.NotesViewModels;
using BMSS.WebUI.Models.PaymentViewModels;
using BMSS.WebUI.Models.POViewModels;
using BMSS.WebUI.Models.PQViewModels;
using BMSS.WebUI.Models.SQViewModels;
using BMSS.WebUI.Models.StockIssueViewModels;
using BMSS.WebUI.Models.StockReceiptViewModels;
using BMSS.WebUI.Models.StockTransViewModels;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSS.WebUI.Infrastructure
{
    public class CustomerNoteAll : Attribute { }
    public class CustomerNoteMngt : Attribute { }
    public class SupplierNoteAll : Attribute { }
    public class SupplierNoteMngt : Attribute { }
    public class ItemNoteAll : Attribute { }
    public class ItemNoteMngt : Attribute { }
    public class NumPQ : Attribute { }
    public class NumSQ : Attribute { }
    public class NumDO : Attribute { }
    public class NumCSale : Attribute { }
    public class NumCrSale : Attribute { }
    public class NumPO : Attribute { }
    public class NumGRN : Attribute { }
    public class NumSI : Attribute { }
    public class NumSL : Attribute { }
    public class NumSR : Attribute { }
    public class NumST : Attribute { }
    public class NumPay : Attribute { }
    public class NumAlPay : Attribute { }

    
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public static DateTime tempDateTime { get; set; }
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private MapperConfiguration CreateConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // Add all profiles in current assembly
                cfg.AddMaps(GetType().Assembly);

                cfg.CreateMap<CNotesAll, NoteViewModel>();
                cfg.CreateMap<AddUpdateNoteViewModel, CNotesAll>();
                cfg.CreateMap<CNotesAll, AddUpdateNoteViewModel>();

                cfg.CreateMap<CNotesMngt, NoteViewModel>();
                cfg.CreateMap<AddUpdateNoteViewModel, CNotesMngt>();
                cfg.CreateMap<CNotesMngt, AddUpdateNoteViewModel>();

                cfg.CreateMap<INotesAll, ItemNoteViewModel>();
                cfg.CreateMap<AddUpdateItemNoteViewModel, INotesAll>();
                cfg.CreateMap<INotesAll, AddUpdateItemNoteViewModel>();

                cfg.CreateMap<INotesMngt, ItemNoteViewModel>();
                cfg.CreateMap<AddUpdateItemNoteViewModel, INotesMngt>();
                cfg.CreateMap<INotesMngt, AddUpdateItemNoteViewModel>();


                cfg.CreateMap<SNotesAll, NoteViewModel>();
                cfg.CreateMap<AddUpdateNoteViewModel, SNotesAll>();
                cfg.CreateMap<SNotesAll, AddUpdateNoteViewModel>();

                cfg.CreateMap<SNotesMngt, NoteViewModel>();
                cfg.CreateMap<AddUpdateNoteViewModel, SNotesMngt>();
                cfg.CreateMap<SNotesMngt, AddUpdateNoteViewModel>();

                cfg.CreateMap<NumberingSQ, DocNumberingViewModel>();
                cfg.CreateMap<AddUpdateDocNumberingViewModel, NumberingSQ>();
                cfg.CreateMap<NumberingSQ, AddUpdateDocNumberingViewModel>();

                cfg.CreateMap<NumberingPQ, DocNumberingViewModel>();
                cfg.CreateMap<AddUpdateDocNumberingViewModel, NumberingPQ>();
                cfg.CreateMap<NumberingPQ, AddUpdateDocNumberingViewModel>();

                cfg.CreateMap<NumberingDO, DocNumberingViewModel>();
                cfg.CreateMap<AddUpdateDocNumberingViewModel, NumberingDO>();
                cfg.CreateMap<NumberingDO, AddUpdateDocNumberingViewModel>();

                cfg.CreateMap<NumberingCSale, DocNumberingViewModel>();
                cfg.CreateMap<AddUpdateDocNumberingViewModel, NumberingCSale>();
                cfg.CreateMap<NumberingCSale, AddUpdateDocNumberingViewModel>();

                cfg.CreateMap<NumberingCrSale, DocNumberingViewModel>();
                cfg.CreateMap<AddUpdateDocNumberingViewModel, NumberingCrSale>();
                cfg.CreateMap<NumberingCrSale, AddUpdateDocNumberingViewModel>();

                cfg.CreateMap<NumberingPO, DocNumberingViewModel>();
                cfg.CreateMap<AddUpdateDocNumberingViewModel, NumberingPO>();
                cfg.CreateMap<NumberingPO, AddUpdateDocNumberingViewModel>();

                cfg.CreateMap<NumberingGRN, DocNumberingViewModel>();
                cfg.CreateMap<AddUpdateDocNumberingViewModel, NumberingGRN>();
                cfg.CreateMap<NumberingGRN, AddUpdateDocNumberingViewModel>();

                cfg.CreateMap<NumberingSI, DocNumberingViewModel>();
                cfg.CreateMap<AddUpdateDocNumberingViewModel, NumberingSI>();
                cfg.CreateMap<NumberingSI, AddUpdateDocNumberingViewModel>();

                cfg.CreateMap<NumberingSL, DocNumberingViewModel>();
                cfg.CreateMap<AddUpdateDocNumberingViewModel, NumberingSL>();
                cfg.CreateMap<NumberingSL, AddUpdateDocNumberingViewModel>();

                cfg.CreateMap<NumberingSR, DocNumberingViewModel>();
                cfg.CreateMap<AddUpdateDocNumberingViewModel, NumberingSR>();
                cfg.CreateMap<NumberingSR, AddUpdateDocNumberingViewModel>();

                cfg.CreateMap<NumberingST, DocNumberingViewModel>();
                cfg.CreateMap<AddUpdateDocNumberingViewModel, NumberingST>();
                cfg.CreateMap<NumberingST, AddUpdateDocNumberingViewModel>();

                cfg.CreateMap<NumberingPay, DocNumberingViewModel>();
                cfg.CreateMap<AddUpdateDocNumberingViewModel, NumberingPay>();
                cfg.CreateMap<NumberingPay, AddUpdateDocNumberingViewModel>();

                cfg.CreateMap<NumberingAlPay, DocNumberingViewModel>();
                cfg.CreateMap<AddUpdateDocNumberingViewModel, NumberingAlPay>();
                cfg.CreateMap<NumberingAlPay, AddUpdateDocNumberingViewModel>();

                cfg.CreateMap<SQViewModel, SQDocH>()
                .ForMember(d => d.DocDate, e => e.MapFrom(s => DateTime.ParseExact(s.DocDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)))
                //.ForMember(d => d.DeliveryDate, e => e.MapFrom(s => DateTime.ParseExact(s.DeliveryDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)))
                .ForMember(d => d.DeliveryDate, e => e.MapFrom(s => (string.IsNullOrEmpty(s.DeliveryDate))? (DateTime?)null: DateTime.ParseExact(s.DeliveryDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)));

                cfg.CreateMap<SQDocH, SQViewModel>();                
                cfg.CreateMap<SQLineViewModel, SQDocLs>();
                cfg.CreateMap<SQDocLs, SQLineViewModel>();


                cfg.CreateMap<SQNoteViewModel, SQDocNotes>();
                cfg.CreateMap<SQDocNotes, SQNoteViewModel>();

                cfg.CreateMap<PQDocH, PQViewModel>();
                cfg.CreateMap<PQLineViewModel, PQDocLs>();
                cfg.CreateMap<PQDocLs, PQLineViewModel>();
                cfg.CreateMap<PQViewModel, PQDocH>()
                .ForMember(d => d.DocDate, e => e.MapFrom(s => DateTime.ParseExact(s.DocDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)))
                .ForMember(d => d.DeliveryDate, e => e.MapFrom(s => (string.IsNullOrEmpty(s.DeliveryDate)) ? (DateTime?)null : DateTime.ParseExact(s.DeliveryDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)));

                cfg.CreateMap<PQNoteViewModel, PQDocNotes>();
                cfg.CreateMap<PQDocNotes, PQNoteViewModel>();

                //SQ to DO copying
                cfg.CreateMap<SQDocH, DODocH>()
                .ForMember(d => d.DocDate, e => e.MapFrom(s => DateTime.Now));
                cfg.CreateMap<SQDocLs, DODocLs>();
                cfg.CreateMap<SQDocNotes, DODocNotes>();

                //PQ to PO copying
                cfg.CreateMap<PQDocH, PODocH>();
                cfg.CreateMap<PQDocLs, PODocLs>()
                .ForMember(x=> x.OpenQty, x=> x.MapFrom(s=> s.Qty));
                cfg.CreateMap<PQDocNotes, PODocNotes>();

                cfg.CreateMap<DOViewModel, DODocH>()
                .ForMember(d => d.DocDate, e => e.MapFrom(s => DateTime.ParseExact(s.DocDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)))
                .ForMember(d => d.DeliveryDate, e => e.MapFrom(s => (string.IsNullOrEmpty(s.DeliveryDate)) ? (DateTime?)null : DateTime.ParseExact(s.DeliveryDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)))
                .ForMember(d => d.DueDate, e => e.MapFrom(s => DateTime.ParseExact(s.DueDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)));
                cfg.CreateMap<DODocH, DOViewModel>();
                cfg.CreateMap<DOLineViewModel, DODocLs>();
                cfg.CreateMap<DODocLs, DOLineViewModel>();

                cfg.CreateMap<DONoteViewModel, DODocNotes>();
                cfg.CreateMap<DODocNotes, DONoteViewModel>();

                cfg.CreateMap<POViewModel, PODocH>()
                .ForMember(d => d.DocDate, e => e.MapFrom(s => DateTime.ParseExact(s.DocDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)))
                .ForMember(d => d.DeliveryDate, e => e.MapFrom(s => (string.IsNullOrEmpty(s.DeliveryDate)) ? (DateTime?)null : DateTime.ParseExact(s.DeliveryDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)));
                //.ForMember(d => d.DueDate, e => e.MapFrom(s => DateTime.ParseExact(s.DueDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)));
                cfg.CreateMap<PODocH, POViewModel>();                
                cfg.CreateMap<POLineViewModel, PODocLs>();
                cfg.CreateMap<PODocLs, POLineViewModel>();

                cfg.CreateMap<PONoteViewModel, PODocNotes>();
                cfg.CreateMap<PODocNotes, PONoteViewModel>();


                //PO to GRPO copying
                cfg.CreateMap<PODocH, GRPODocH>();
                cfg.CreateMap<PODocLs, GRPODocLs>().ForMember(d => d.Qty, e => e.MapFrom(s => s.GRPOQty));
                cfg.CreateMap<PODocNotes, GRPODocNotes>();

                cfg.CreateMap<CashCustomerViewModel, CashSalesCustomerMaster>();
                cfg.CreateMap<CashSalesCustomerMaster, CashCustomerViewModel>();


                cfg.CreateMap<CashSalesViewModel, CashSalesDocH>()
                .ForMember(d => d.DocDate, e => e.MapFrom(s => DateTime.ParseExact(s.DocDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)))
                .ForMember(d => d.DeliveryDate, e => e.MapFrom(s => (string.IsNullOrEmpty(s.DeliveryDate)) ? (DateTime?)null : DateTime.ParseExact(s.DeliveryDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)));
                //.ForMember(d => d.DueDate, e => e.MapFrom(s => DateTime.ParseExact(s.DueDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)));
                cfg.CreateMap<CashSalesDocH, CashSalesViewModel>();
                cfg.CreateMap<CashSalesLineViewModel, CashSalesDocLs>();
                cfg.CreateMap<CashSalesDocLs, CashSalesLineViewModel>();

                cfg.CreateMap<CashSalesNoteViewModel, CashSalesDocNotes>();
                cfg.CreateMap<CashSalesDocNotes, CashSalesNoteViewModel>();
                cfg.CreateMap<CashSalesPayViewModel, CashSalesDocPays>();
                cfg.CreateMap<CashSalesDocPays, CashSalesPayViewModel>();

                cfg.CreateMap<CashSalesCreditViewModel, CashSalesCreditDocH>()
                .ForMember(d => d.DocDate, e => e.MapFrom(s => DateTime.ParseExact(s.DocDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)));
                //.ForMember(d => d.DeliveryDate, e => e.MapFrom(s => DateTime.ParseExact(s.DeliveryDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)))
                //.ForMember(d => d.DueDate, e => e.MapFrom(s => DateTime.ParseExact(s.DueDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)));
                cfg.CreateMap<CashSalesCreditDocH, CashSalesCreditViewModel>();
                cfg.CreateMap<CashSalesCreditLineViewModel, CashSalesCreditDocLs>();
                cfg.CreateMap<CashSalesCreditDocLs, CashSalesCreditLineViewModel>();

                cfg.CreateMap<CashSalesCreditNoteViewModel, CashSalesCreditDocNotes>();
                cfg.CreateMap<CashSalesCreditDocNotes, CashSalesCreditNoteViewModel>();

                cfg.CreateMap<CashSalesCreditPayViewModel, CashSalesCreditDocPays>();
                cfg.CreateMap<CashSalesCreditDocPays, CashSalesCreditPayViewModel>();

                cfg.CreateMap<GRPOViewModel, GRPODocH>()
                .ForMember(d => d.DocDate, e => e.MapFrom(s => DateTime.ParseExact(s.DocDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)))
                .ForMember(d => d.DeliveryDate, e => e.MapFrom(s => (string.IsNullOrEmpty(s.DeliveryDate)) ? (DateTime?)null : DateTime.ParseExact(s.DeliveryDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)))
                .ForMember(d => d.DueDate, e => e.MapFrom(s => DateTime.ParseExact(s.DueDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture))); ;
                cfg.CreateMap<GRPODocH, GRPOViewModel>();
                cfg.CreateMap<GRPOLineViewModel, GRPODocLs>();
                cfg.CreateMap<GRPODocLs, GRPOLineViewModel>();

                cfg.CreateMap<GRPONoteViewModel, GRPODocNotes>();
                cfg.CreateMap<GRPODocNotes, GRPONoteViewModel>();

                //PO to GRPO copying
                cfg.CreateMap<PODocH, GRPODocH>();
                cfg.CreateMap<PODocLs, GRPODocLs>();
                cfg.CreateMap<PODocNotes, GRPODocNotes>();

                cfg.CreateMap<StockIssueViewModel, StockIssueDocH>()
                .ForMember(d => d.DocDate, e => e.MapFrom(s => DateTime.ParseExact(s.DocDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)));
                cfg.CreateMap<StockIssueDocH, StockIssueViewModel>();
                cfg.CreateMap<StockIssueLineViewModel, StockIssueDocLs>();
                cfg.CreateMap<StockIssueDocLs, StockIssueLineViewModel>();

                cfg.CreateMap<StockIssueNoteViewModel, StockIssueDocNotes>();
                cfg.CreateMap<StockIssueDocNotes, StockIssueNoteViewModel>();

                cfg.CreateMap<StockReceiptViewModel, StockReceiptDocH>()
                .ForMember(d => d.DocDate, e => e.MapFrom(s => DateTime.ParseExact(s.DocDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)));
                cfg.CreateMap<StockReceiptDocH, StockReceiptViewModel>();
                cfg.CreateMap<StockReceiptLineViewModel, StockReceiptDocLs>();
                cfg.CreateMap<StockReceiptDocLs, StockReceiptLineViewModel>();

                cfg.CreateMap<StockReceiptNoteViewModel, StockReceiptDocNotes>();
                cfg.CreateMap<StockReceiptDocNotes, StockReceiptNoteViewModel>();

                cfg.CreateMap<StockTransViewModel, StockTransDocH>()
                .ForMember(d => d.DocDate, e => e.MapFrom(s => DateTime.ParseExact(s.DocDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)));
                cfg.CreateMap<StockTransDocH, StockTransViewModel>();
                cfg.CreateMap<StockTransLineViewModel, StockTransDocLs>();
                cfg.CreateMap<StockTransDocLs, StockTransLineViewModel>();

                cfg.CreateMap<StockTransNoteViewModel, StockTransDocNotes>();
                cfg.CreateMap<StockTransDocNotes, StockTransNoteViewModel>();


                cfg.CreateMap<PaymentViewModel, PaymentDocH>().ForMember(d => d.DocDate, e => e.MapFrom(s => DateTime.ParseExact(s.DocDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture)));
                cfg.CreateMap<PaymentDocH, PaymentViewModel>();
                cfg.CreateMap<PaymentLineViewModel, PaymentDocLs>();
                cfg.CreateMap<PaymentDocLs, PaymentLineViewModel>().ForMember(d => d.DocDate, e => e.MapFrom(s => s.DocDate.HasValue ? s.DocDate.Value.ToString("dd'/'MM'/'yyyy") : ""));

                cfg.CreateMap<PaymentNoteViewModel, PaymentDocNotes>();
                cfg.CreateMap<PaymentDocNotes, PaymentNoteViewModel>();

                cfg.CreateMap<ChangeCRLimitViewModel, ChangeCRLimit>();

                cfg.CreateMap<DOPlannerRowViewModel, DeliveryPlanner>().ForMember(d => d.DeliveryDate, e => e.MapFrom(s => DateTime.ParseExact(s.DeliveryDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture))); 

            });

            return config;
        }

        private void AddBindings()
        {

            var mapperConfiguration = CreateConfiguration();
            kernel.Bind<MapperConfiguration>().ToConstant(mapperConfiguration).InSingletonScope();
            kernel.Bind<IMapper>().ToMethod(ctx =>
                 new Mapper(mapperConfiguration, type => ctx.Kernel.Get(type)));

            // put bindings here
            //SAP
            kernel.Bind<I_OITM_Repository>().To<EF_OITM_Repository>();
            kernel.Bind<I_OITW_Repository>().To<EF_OITW_Repository>();
            kernel.Bind<I_INV1_Repository>().To<EF_INV1_Repository>();
            kernel.Bind<I_ITM1_Repository>().To<EF_ITM1_Repository>();
            kernel.Bind<I_SPP2_Repository>().To<EF_SPP2_Repository>();
            kernel.Bind<I_SPP1_Repository>().To<EF_SPP1_Repository>();
            kernel.Bind<I_OCRD_Repository>().To<EF_OCRD_Repository>();
            kernel.Bind<I_OCPR_Repository>().To<EF_OCPR_Repository>();
            kernel.Bind<I_OCTG_Repository>().To<EF_OCTG_Repository>();
            kernel.Bind<I_OSLP_Repository>().To<EF_OSLP_Repository>();
            kernel.Bind<I_ORTT_Repository>().To<EF_ORTT_Repository>();
            kernel.Bind<I_OWHS_Repository>().To<EF_OWHS_Repository>();
            kernel.Bind<I_OVTG_Repository>().To<EF_OVTG_Repository>();
            kernel.Bind<I_CRD1_Repository>().To<EF_CRD1_Repository>();
            kernel.Bind<I_OADM_Repository>().To<EF_OADM_Repository>();
            kernel.Bind<I_OINM_Repository>().To<EF_OINM_Repository>();
            kernel.Bind<I_OACT_Repository>().To<EF_OACT_Repository>();
            kernel.Bind<I_OCRC_Repository>().To<EF_OCRC_Repository>();
            kernel.Bind<I_PDN1_Repository>().To<EF_PDN1_Repository>();
            kernel.Bind<I_POR1_Repository>().To<EF_POR1_Repository>();
            kernel.Bind<I_OPLN_Repository>().To<EF_OPLN_Repository>();
            kernel.Bind<I_ITT1_Repository>().To<EF_ITT1_Repository>();
            kernel.Bind<I_ORCT_Repository>().To<EF_ORCT_Repository>();
            kernel.Bind<I_OPOR_Repository>().To<EF_OPOR_Repository>();

            //Planner
            kernel.Bind<I_DeliveryPlanner_Repository>().To<EF_DeliveryPlanner_Repository>();

            kernel.Bind<I_Notes_Repository>().To<EF_CNotesAll_Repository>().WhenTargetHas<CustomerNoteAll>();
            kernel.Bind<I_Notes_Repository>().To<EF_CNotesMngt_Repository>().WhenTargetHas<CustomerNoteMngt>();
            kernel.Bind<I_Notes_Repository>().To<EF_SNotesAll_Repository>().WhenTargetHas<SupplierNoteAll>();
            kernel.Bind<I_Notes_Repository>().To<EF_SNotesMngt_Repository>().WhenTargetHas<SupplierNoteMngt>();

            kernel.Bind<I_ItmNotes_Repository>().To<EF_ItmNotesAll_Repository>().WhenTargetHas<ItemNoteAll>();
            kernel.Bind<I_ItmNotes_Repository>().To<EF_ItmNotesMngt_Repository>().WhenTargetHas<ItemNoteMngt>();

            kernel.Bind<I_Numbering_Repository>().To<EF_NumberingPQ_Repository>().WhenTargetHas<NumPQ>();
            kernel.Bind<I_Numbering_Repository>().To<EF_NumberingSQ_Repository>().WhenTargetHas<NumSQ>();
            kernel.Bind<I_Numbering_Repository>().To<EF_NumberingDO_Repository>().WhenTargetHas<NumDO>();
            kernel.Bind<I_Numbering_Repository>().To<EF_NumberingCSale_Repository>().WhenTargetHas<NumCSale>();
            kernel.Bind<I_Numbering_Repository>().To<EF_NumberingCrSale_Repository>().WhenTargetHas<NumCrSale>();
            kernel.Bind<I_Numbering_Repository>().To<EF_NumberingPO_Repository>().WhenTargetHas<NumPO>();
            kernel.Bind<I_Numbering_Repository>().To<EF_NumberingGRN_Repository>().WhenTargetHas<NumGRN>();
            kernel.Bind<I_Numbering_Repository>().To<EF_NumberingSI_Repository>().WhenTargetHas<NumSI>();
            kernel.Bind<I_Numbering_Repository>().To<EF_NumberingSL_Repository>().WhenTargetHas<NumSL>();
            kernel.Bind<I_Numbering_Repository>().To<EF_NumberingSR_Repository>().WhenTargetHas<NumSR>();
            kernel.Bind<I_Numbering_Repository>().To<EF_NumberingST_Repository>().WhenTargetHas<NumST>();
            kernel.Bind<I_Numbering_Repository>().To<EF_NumberingPay_Repository>().WhenTargetHas<NumPay>();
            kernel.Bind<I_Numbering_Repository>().To<EF_NumberingAlPay_Repository>().WhenTargetHas<NumAlPay>();

            kernel.Bind<I_SQDocH_Repository>().To<EF_SQDocHeader_Repository>();
            kernel.Bind<I_SQDocLs_Repository>().To<EF_SQDocLine_Repository>();

            kernel.Bind<I_PQDocH_Repository>().To<EF_PQDocHeader_Repository>();
            kernel.Bind<I_PQDocLs_Repository>().To<EF_PQDocLine_Repository>();

            kernel.Bind<I_DODocH_Repository>().To<EF_DODocHeader_Repository>();
            kernel.Bind<I_DODocLs_Repository>().To<EF_DODocLine_Repository>();

            kernel.Bind<I_PODocH_Repository>().To<EF_PODocHeader_Repository>();
            kernel.Bind<I_PODocLs_Repository>().To<EF_PODocLine_Repository>();

            kernel.Bind<I_CashSalesCustomer_Repository>().To<EF_CashSalesCustomer_Repository>();

            kernel.Bind<I_CashSalesDocH_Repository>().To<EF_CashSalesDocHeader_Repository>();
            kernel.Bind<I_CashSalesDocLs_Repository>().To<EF_CashSalesDocLine_Repository>();

            kernel.Bind<I_CashSalesCreditDocH_Repository>().To<EF_CashSalesCreditDocHeader_Repository>();
            kernel.Bind<I_CashSalesCreditDocLs_Repository>().To<EF_CashSalesCreditDocLine_Repository>();

            kernel.Bind<I_GRPODocH_Repository>().To<EF_GRPODocHeader_Repository>();
            kernel.Bind<I_GRPODocLs_Repository>().To<EF_GRPODocLine_Repository>();

            kernel.Bind<I_StockIssueDocH_Repository>().To<EF_StockIssueDocHeader_Repository>();
            kernel.Bind<I_StockIssueDocLs_Repository>().To<EF_StockIssueDocLine_Repository>();

            kernel.Bind<I_StockReceiptDocH_Repository>().To<EF_StockReceiptDocHeader_Repository>();
            kernel.Bind<I_StockReceiptDocLs_Repository>().To<EF_StockReceiptDocLine_Repository>();

            kernel.Bind<I_StockTransDocH_Repository>().To<EF_StockTransDocHeader_Repository>();
            kernel.Bind<I_StockTransDocLs_Repository>().To<EF_StockTransDocLine_Repository>();

            kernel.Bind<I_PaymentDocH_Repository>().To<EF_PaymentDocHeader_Repository>();

            kernel.Bind<I_ChangeCRLimit_Repository>().To<EF_ChangeCRLimit_Repository>();

            kernel.Bind<I_DODocStockLoan_Repository>().To<EF_DODocStockLoan_Repository>();

            kernel.Bind<I_InventoryMovement_Repository>().To<EF_InventoryMovement_Repository>();

           

        }
    }
}