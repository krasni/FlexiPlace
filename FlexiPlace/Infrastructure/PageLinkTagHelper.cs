using FlexiPlace.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;

namespace FlexiPlace.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }

        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");
            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                PageUrlValues["zahtjevStrana"] = i;

                // kopiraj PageUrlValues u novi dictionary
                Dictionary<string, object> filterValues = new Dictionary<string, object>(PageUrlValues);

                // removaj datumska polja prije generiranja hrefa za page
                filterValues.Remove("datumOtvaranjaOd");
                filterValues.Remove("datumOtvaranjaDo");
                filterValues.Remove("datumOdsustvaOd");
                filterValues.Remove("datumOdsustvaDo");

                // napravi href zasada bez datumskih filtera
                tag.Attributes["href"] = urlHelper.Action(PageAction, filterValues);

                // dodaj manualno na href datumska polja
                if (PageUrlValues["datumOtvaranjaOd"] != null)
                    tag.Attributes["href"] += $"&DatumOtvaranjaOd={(DateTime)PageUrlValues["datumOtvaranjaOd"]:dd.MM.yyyy}";

                if (PageUrlValues["datumOtvaranjaDo"] != null)
                    tag.Attributes["href"] += $"&DatumOtvaranjaDo={(DateTime)PageUrlValues["datumOtvaranjaDo"]:dd.MM.yyyy}";

                if (PageUrlValues["datumOdsustvaOd"] != null)
                    tag.Attributes["href"] += $"&DatumOdsustvaOd={(DateTime)PageUrlValues["datumOdsustvaOd"]:dd.MM.yyyy}";

                if (PageUrlValues["datumOdsustvaDo"] != null)
                    tag.Attributes["href"] += $"&DatumOdsustvaDo={(DateTime)PageUrlValues["datumOdsustvaDo"]:dd.MM.yyyy}";

                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage
                    ? PageClassSelected : PageClassNormal);
                }

                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
