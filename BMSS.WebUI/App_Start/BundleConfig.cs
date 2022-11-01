using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace BMSS.WebUI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            /******************* Style Bundles ******************/
            //Bootstrap css
            bundles.Add(new StyleBundle("~/Bootstrap-css").Include(
                "~/Content/css/bootstrap-4.3.1.min.css"
                ));
            //Font-awesome css
            bundles.Add(new StyleBundle("~/Font-awesome-css").Include(
               "~/Content/css/font-awesome-4.7.0.min.css"
               ));
            //Ionicons css
            bundles.Add(new StyleBundle("~/Ionicons").Include(
                "~/Content/css/ionicons-2.0.0.min.css"
                ));
            //Admin LTE css and OWN
            bundles.Add(new StyleBundle("~/OwnCss").Include(
               "~/Content/css/adminlte-2.4.7.css",
               "~/Content/css/skin-blue.css",
               "~/Content/css/own.css"
               ));
            //DataTables css
            bundles.Add(new StyleBundle("~/DataTablesCss").Include(
                "~/Content/css/datatables/dataTables.bootstrap4.min.css",
                "~/Content/css/datatables/responsive.bootstrap4.min.css",
                 "~/Content/css/datatables/fixedHeader.bootstrap4.min.css",
                 "~/Content/css/datatables/fixedColumns.bootstrap4.css",
                 "~/Content/css/datatables/jquery.dataTables.dt-control.css"
                ));

            //Select 2 css
            bundles.Add(new StyleBundle("~/Select2Css").Include(
                "~/Content/css/select2.min.css"
                ));
            //Datepicker css
            bundles.Add(new StyleBundle("~/DatePicker").Include(
                "~/Content/css/bootstrap-datepicker.min.css"
                ));
            //Print css
            bundles.Add(new StyleBundle("~/Print").Include(
                "~/Content/css/print.min.css"
                ));


            /******************* Script Bundles ******************/
            // Jquery
            bundles.Add(new ScriptBundle("~/Jquery").Include(
                "~/Scripts/jquery-3.4.1.min.js"
                ));
                        

            // Jquery - Validation and Ajax
            bundles.Add(new ScriptBundle("~/bundles/Jquery-Validation-and-Ajax").Include(
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            //Bootstrap Js
            bundles.Add(new ScriptBundle("~/Bootstrap")
                .Include("~/Scripts/bootstrap.bundle.min.js"));

            //AdminLTE Js
            bundles.Add(new ScriptBundle("~/AdminLTE").Include(
               "~/Scripts/adminlte.js"
               ));
            //Select2 Js
            bundles.Add(new ScriptBundle("~/Select2").Include(
                "~/Scripts/select2.full.min.js"
                ));
            //Select2-Own Js
            bundles.Add(new ScriptBundle("~/Select2-Own").Include(
               "~/Scripts/select2-own.js"
               ));
            //DatePicker Js
            bundles.Add(new ScriptBundle("~/DatePickerJS").Include(
                "~/Scripts/bootstrap-datepicker.min.js"
                ));
            //Own Js
            bundles.Add(new ScriptBundle("~/Own").Include(
                 "~/Scripts/Own.js"
                ));
            //Knockout Js
            bundles.Add(new ScriptBundle("~/Knockout").Include(
                 "~/Scripts/knockout-3.5.0.min.js",
                 "~/Scripts/knockout-Custom.js"
                ));
            //General Theme Js
            bundles.Add(new ScriptBundle("~/GeneralJs").Include(
                 "~/Scripts/general-theme.js"
                ));
            //DataTables
            bundles.Add(new ScriptBundle("~/DataTables").Include(
                "~/Scripts/datatables/jquery.dataTables.min.js",
                "~/Scripts/datatables/dataTables.bootstrap4.min.js",
                "~/Scripts/datatables/dataTables.responsive.min.js",
                "~/Scripts/datatables/dataTables.fixedHeader.min.js",
                "~/Scripts/datatables/dataTables.fixedColumns.min.js"
                ));

            // Only DataTable
            bundles.Add(new ScriptBundle("~/DataTableOnly").Include(
                  "~/Scripts/datatables/jquery.dataTables.min1.js"
                ));

            //DataTables
            bundles.Add(new ScriptBundle("~/DataTablesHeaderFix").Include(
                  "~/Scripts/datatables/dataTables.bootstrap4.min.js",
                "~/Scripts/datatables/dataTables.responsive.min.js",
                "~/Scripts/datatables/dataTables.fixedHeader.min.js",
                "~/Scripts/datatables/dataTables.fixedColumns.min.js"
                ));
            //Print Js
            bundles.Add(new ScriptBundle("~/PrintJs").Include(
                "~/Scripts/print.min.js"
               ));

            BundleTable.EnableOptimizations = true;
        }
    }
}