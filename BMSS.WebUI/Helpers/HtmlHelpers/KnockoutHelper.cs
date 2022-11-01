using Newtonsoft.Json;
using System.Web;
using System.Web.Mvc;

namespace BMSS.WebUI.Helpers.HtmlHelpers
{
    public static class KnockoutHelper
    {
        public static HtmlString ViewModelToJson(this HtmlHelper htmlHelper, object model)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            return new HtmlString(JsonConvert.SerializeObject(model, settings));
        }
    }
}