using GainBargain.WEB.Models;
using System.Text;
using System.Web.Mvc;

namespace GainBargain.WEB.Helpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageButtons(this HtmlHelper<CatalogVM> html, PageInfo pageInfo)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= pageInfo.TotalPages; ++i)
            {
                TagBuilder button = new TagBuilder("button");
                button.MergeAttribute("type", "submit");
                button.MergeAttribute("value", i.ToString());
                button.MergeAttribute("name", "page");
                button.InnerHtml = i.ToString();

                if (i == pageInfo.PageNumber)
                {
                    button.AddCssClass("selected");
                    button.AddCssClass("btn-primary");
                }

                button.AddCssClass("btn btn-default");
                result.Append(button.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }


        public static MvcHtmlString PageButtons(this HtmlHelper html, Pager pager)
        {
            StringBuilder result = new StringBuilder();

            if (pager.EndPage > 1)
            {
                TagBuilder u = new TagBuilder("u");
                u.AddCssClass("pagination");
                if (pager.CurrentPage > 1)
                {
                    TagBuilder liPrev = new TagBuilder("li");

                    TagBuilder button = new TagBuilder("button");
                    button.MergeAttribute("type", "submit");
                    button.MergeAttribute("value", (pager.CurrentPage - 1).ToString());
                    button.MergeAttribute("name", "page");
                    button.InnerHtml = "Попередня";
                    u.InnerHtml += liPrev;
                }

                TagBuilder liFirst = new TagBuilder("li");
                liFirst.AddCssClass(pager.CurrentPage == 1 ? "active" : "");

                TagBuilder buttonFirst = new TagBuilder("button");
                buttonFirst.MergeAttribute("type", "submit");
                buttonFirst.MergeAttribute("value", "1");
                buttonFirst.MergeAttribute("name", "page");
                buttonFirst.InnerHtml = "1";
                liFirst.InnerHtml += buttonFirst;
                u.InnerHtml += liFirst;

                if (pager.CurrentPage > 2 &&
                    pager.StartPage != 2 &&
                    pager.StartPage + 1 != 2 &&
                    pager.TotalItems != pager.PagerLength)
                {
                    TagBuilder liDots = new TagBuilder("li");
                    TagBuilder aDots = new TagBuilder("p");
                    aDots.SetInnerText("...");
                    liDots.InnerHtml += aDots;
                    u.InnerHtml += liDots;
                }

                for (var _page = pager.StartPage; _page <= pager.EndPage; ++_page)
                {
                    if (_page != 1)
                    {
                        if (_page != pager.TotalPages)
                        {
                            TagBuilder liPage = new TagBuilder("li");
                            liPage.AddCssClass(_page == pager.CurrentPage ? "active" : "");


                            TagBuilder aButton = new TagBuilder("button");
                            aButton.MergeAttribute("type", "submit");
                            aButton.MergeAttribute("value", _page.ToString());
                            aButton.MergeAttribute("name", "page");
                            aButton.InnerHtml = _page.ToString();
                            liPage.InnerHtml += aButton;
                            u.InnerHtml += liPage;
                        }
                    }
                }

                if (pager.CurrentPage < pager.TotalPages - 1 &&
                    pager.EndPage != pager.TotalPages - 1 &&
                    pager.EndPage != pager.TotalPages &&
                    pager.TotalItems != pager.PagerLength)
                {
                    TagBuilder liDots = new TagBuilder("li");
                    TagBuilder aDots = new TagBuilder("p");
                    aDots.SetInnerText("...");
                    liDots.InnerHtml += aDots;
                    u.InnerHtml += liDots;
                }

                TagBuilder liLast = new TagBuilder("li");
                liLast.AddCssClass(pager.CurrentPage == pager.TotalPages ? "active" : "");
                u.InnerHtml += liLast;

                if (pager.CurrentPage < pager.TotalPages)
                {
                    TagBuilder liNext = new TagBuilder("li");

                    TagBuilder nextButton = new TagBuilder("button");
                    nextButton.MergeAttribute("type", "submit");
                    nextButton.MergeAttribute("value", (pager.CurrentPage + 1).ToString());
                    nextButton.MergeAttribute("name", "page");
                    nextButton.InnerHtml = "Наступна";
                    liNext.InnerHtml += nextButton;
                    u.InnerHtml += liNext;
                }

                result.Append(u);
            }

            return MvcHtmlString.Create(result.ToString());
        }

    }
}