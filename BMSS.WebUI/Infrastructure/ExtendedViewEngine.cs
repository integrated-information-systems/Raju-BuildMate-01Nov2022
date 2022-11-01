using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Infrastructure
{
    public class ExtendedViewEngine : RazorViewEngine
    {
        private static readonly string[] NewPartialViewFormats =
        {
            "~/Views/{1}/Partials/{0}.cshtml",
            "~/Views/Shared/Partials/{0}.cshtml"
        };

        public ExtendedViewEngine()
        {
            base.PartialViewLocationFormats = base.PartialViewLocationFormats.Union(NewPartialViewFormats).ToArray();
        }
    }
}