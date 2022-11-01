using BMSS.WebUI.Helpers.Functions;
using BMSS.WebUI.Helpers.Models;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace BMSS.WebUI.Helpers.HtmlHelpers
{
    public static class SiteMenu
    {
        public static MvcHtmlString SiteMenuLinks(this HtmlHelper htmlHelper)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);


            StringBuilder result = new StringBuilder();
            TagBuilder ParentTag = new TagBuilder("ul");
            ParentTag.AddCssClass("sidebar-menu");
            ParentTag.Attributes.Add("data-widget", "tree");

            TagBuilder MenuTitle = new TagBuilder("li");
            MenuTitle.AddCssClass("header");
            MenuTitle.InnerHtml = "Main Navigation";
            result.Append(MenuTitle.ToString());

             

            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/SiteMenu.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode node = doc.DocumentElement;
            string ReturnResult = HandleNodes(node, urlHelper, htmlHelper);
            result.Append(ReturnResult);

            // Administration Lines -Ends Here

            ParentTag.InnerHtml= result.ToString();
            return MvcHtmlString.Create(ParentTag.ToString());
        }

        
        private static string HandleNodes(XmlNode node, UrlHelper urlHelper, HtmlHelper htmlHelper )
        {
            TagBuilder ReturnResult = null;
            string ChildReturnResult = null;
            StringBuilder html = new StringBuilder();
            if (node.HasChildNodes)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    ChildReturnResult = HandleNodes(child, urlHelper,htmlHelper);
                    html.Append(ChildReturnResult);
                }
            }

            ReturnResult = HandleNode(node, urlHelper,htmlHelper);
            if (ReturnResult != null)
            {
                if (ReturnResult.TagName.Equals("ul"))
                {
                    ReturnResult.InnerHtml = html.ToString();
                    html.Clear();
                    html.Append(ReturnResult.ToString());

                }
                else if(ReturnResult.TagName.Equals("li") ) {
                    if (ReturnResult.Attributes.ContainsKey("class"))
                    {
                        if( ReturnResult.Attributes["class"] == "treeview")
                        {
                            if (html.ToString().Contains("active"))
                            {
                                ReturnResult.AddCssClass("active");
                            }
                            ReturnResult.InnerHtml = ReturnResult.InnerHtml + html.ToString();
                            html.Clear();
                            html.Append(ReturnResult.ToString());
                        }
                        else
                        {
                            html.Append(ReturnResult.ToString());
                        }
                    }
                    else
                    {
                        html.Append(ReturnResult.ToString());
                    }
                }
               
            }
            
            return html.ToString();
        }
        private static TagBuilder HandleNode(XmlNode node, UrlHelper urlHelper, HtmlHelper htmlHelper)
        {
            TagBuilder ReturnResult = null;
            if (node.Attributes != null && node.Name.Equals("MenuItem"))
            {
                MenuItemHelperModel mi = new MenuItemHelperModel();
                foreach (XmlAttribute attr in node.Attributes)
                {
                    string AttributeName = attr.Name;
                    switch (AttributeName)
                    {

                        case "CssClass":
                            mi.CssClass = attr.Value;
                            break;
                        case "FawsIcon":
                            mi.FawsIcon = attr.Value;
                            break;
                        case "DisplayText":
                            mi.DisplayText = attr.Value;
                            break;
                        case "Controller":
                            mi.Controller = attr.Value;
                            break;
                        case "Action":
                            mi.Action = attr.Value;
                            break;
                        default:
                            break;
                    }
                }
                switch (mi.CssClass)
                {
                    case "":
                        {
                            if (mi.Action.EndsWith(".aspx"))
                            {
                                TagBuilder FontAwsTag = new TagBuilder("i");
                                FontAwsTag.AddCssClass(mi.FawsIcon);

                                TagBuilder AnchorTag = new TagBuilder("a");
                                AnchorTag.Attributes.Add("href", urlHelper.Action(mi.Action, mi.Controller));
                                AnchorTag.Attributes.Add("target", "_blank");
                                AnchorTag.InnerHtml = FontAwsTag.ToString() + mi.DisplayText;


                                TagBuilder ChildLI = new TagBuilder("li");                                
                                ChildLI.InnerHtml = AnchorTag.ToString();

                                ReturnResult = ChildLI;


                            }
                            else
                            {                            
                                ActionParameterHelperModel actionParameterHelperModel = new ActionParameterHelperModel { Action = mi.Action, Controller = mi.Controller };
                                if (actionParameterHelperModel.Controller.Equals(string.Empty))
                                {
                                    string CurrentController = htmlHelper.ViewContext.RouteData.Values["Controller"].ToString();
                                    string CurrentAction = htmlHelper.ViewContext.RouteData.Values["Action"].ToString();

                                    TagBuilder FontAwsTag = new TagBuilder("i");
                                    FontAwsTag.AddCssClass(mi.FawsIcon);

                                    TagBuilder AnchorTag = new TagBuilder("a");
                                    AnchorTag.Attributes.Add("href", "#");
                                    AnchorTag.InnerHtml = FontAwsTag.ToString() + mi.DisplayText;


                                    TagBuilder ChildLI = new TagBuilder("li");
                                    if (CurrentController.ToLower().Equals(mi.Controller.ToLower()) && CurrentAction.ToLower().Equals(mi.Action.ToLower()))
                                    {
                                        ChildLI.AddCssClass("active");
                                    }
                                    else if (CurrentController.ToLower().Equals(mi.Controller.ToLower()))
                                    {
                                        ChildLI.AddCssClass("active");
                                    }
                                    ChildLI.InnerHtml = AnchorTag.ToString();

                                    ReturnResult = ChildLI;
                                }
                                else if (SecurityCheck.ActionIsAuthorized(actionParameterHelperModel))
                                {
                                    string CurrentController = htmlHelper.ViewContext.RouteData.Values["Controller"].ToString();
                                    string CurrentAction = htmlHelper.ViewContext.RouteData.Values["Action"].ToString();

                                    TagBuilder FontAwsTag = new TagBuilder("i");
                                    FontAwsTag.AddCssClass(mi.FawsIcon);                               

                                    TagBuilder AnchorTag = new TagBuilder("a");
                                    AnchorTag.Attributes.Add("href", urlHelper.Action(mi.Action, mi.Controller));
                                    AnchorTag.InnerHtml = FontAwsTag.ToString() + mi.DisplayText;

                              
                                    TagBuilder ChildLI = new TagBuilder("li");
                                    if (CurrentController.ToLower().Equals(mi.Controller.ToLower()) && CurrentAction.ToLower().Equals(mi.Action.ToLower()))
                                    {
                                        ChildLI.AddCssClass("active");
                                    }
                                    else if (CurrentController.ToLower().Equals(mi.Controller.ToLower()))
                                    {
                                        ChildLI.AddCssClass("active");
                                    }
                                    ChildLI.InnerHtml = AnchorTag.ToString();

                                    ReturnResult = ChildLI;
                                }
                            }
                        }
                        break;

                    case "treeview-menu":
                        TagBuilder ChildUL = new TagBuilder("ul");
                        ChildUL.AddCssClass(mi.CssClass);
                        ReturnResult = ChildUL;
                        break;
                    case "treeview":
                        {
                            TagBuilder FontAwsTag = new TagBuilder("i");
                            FontAwsTag.AddCssClass(mi.FawsIcon);


                            TagBuilder AnchorTag = new TagBuilder("a");
                            AnchorTag.Attributes.Add("href", "#");

                            TagBuilder DisplayTextSpanTag = new TagBuilder("span");
                            DisplayTextSpanTag.SetInnerText(mi.DisplayText);

                            TagBuilder ArrowSpanTag = new TagBuilder("span");
                            ArrowSpanTag.AddCssClass("pull-right-container");

                            TagBuilder ArrowFontAwsTag = new TagBuilder("i");
                            ArrowFontAwsTag.AddCssClass("fa fa-angle-left pull-right");
                            ArrowSpanTag.InnerHtml = ArrowFontAwsTag.ToString();

                            AnchorTag.InnerHtml = FontAwsTag.ToString() + DisplayTextSpanTag.ToString() + ArrowSpanTag.ToString();

                            TagBuilder ChildLI = new TagBuilder("li");
                            ChildLI.AddCssClass(mi.CssClass);
                            ChildLI.InnerHtml = AnchorTag.ToString();

                            ReturnResult = ChildLI;
                        }
                        break;
                    default:
                        break;
                }


            }
            return ReturnResult;
        }

    }


    
     
}

