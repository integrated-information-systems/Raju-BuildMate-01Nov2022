using System.Collections.Generic;
using System.Linq;

namespace BMSS.WebUI.Helpers.Functions
{
    public static class HelperFunctions
    {
        //public static Dictionary<string, string> GetErrorListFromModelState(System.Web.Mvc.ModelStateDictionary modelState)
        //{


        //    Dictionary<string, string> ErrList = new Dictionary<string, string>();
        //    foreach (var item in modelState)
        //    {
        //        if (item.Value != null || item.Value.Value != null)
        //        {
        //            if (item.Value.Errors.Any())
        //            {
        //                string ErrMsg = string.Empty;
        //                if (item.Value != null || item.Value.Value != null)
        //                {
        //                    ErrMsg = string.Empty;
        //                }
        //                else
        //                {
        //                    ErrList.Add(item.Key, item.Value.Errors[0].ErrorMessage.ToString());
        //                }
        //            }
        //        }
        //    }
        //    return ErrList;
        //}
        public static IEnumerable<string> GetErrorListFromModelState(System.Web.Mvc.ModelStateDictionary modelState)
        {
          return modelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage));
        }

       
    }
}