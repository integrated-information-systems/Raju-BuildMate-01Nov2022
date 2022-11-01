using BMSS.WebUI.Models.General;
using System.Web.Mvc;

namespace BMSS.WebUI.Helpers.HtmlHelpers
{
    public static class StatusHelper
    {
        public static string GetDocStatusName(this HtmlHelper htmlHelper, short inputValue)
        {
            DocumentStatuses StatusValue = (DocumentStatuses)inputValue;
            string ReturnResult = string.Empty;
            ReturnResult = StatusValue.ToString().Replace("Status_", "");
            ReturnResult = ReturnResult.Replace("_", " ");
            return ReturnResult;
        }
        public static string GetPrintedStatusName(this HtmlHelper htmlHelper, short inputValue)
        {
            PrintedStatuses StatusValue = (PrintedStatuses)inputValue;
            string ReturnResult = string.Empty;
            ReturnResult = StatusValue.ToString().Replace("Status_", "");
            ReturnResult = ReturnResult.Replace("_", " ");
            return ReturnResult;
        }
        public static string GetSyncStatusName(this HtmlHelper htmlHelper, short inputValue)
        {
            SyncStatuses StatusValue = (SyncStatuses)inputValue;
            string ReturnResult = string.Empty;
            ReturnResult = StatusValue.ToString().Replace("Status_", "");
            ReturnResult = ReturnResult.Replace("_", " ");
            return ReturnResult;
        }
    }
}