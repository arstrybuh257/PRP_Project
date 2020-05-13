using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.Parser.Parsers;
using GainBargain.Parser.WebAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GainBargain.WEB.Controllers
{
    public class AdminController : Controller
    {
        GainBargainContext db = new GainBargainContext();

        private class ParserInput : ParserSource
        {
            public string SelPrice { set; get; }
            public string SelName { set; get; }
            public string SelImageUrl { set; get; }

            public ParserInput() { }

            public ParserInput(ParserSource source, Market market)
            {
                this.ParserId = source.ParserId;
                this.Url = source.Url;
                this.MarketId = source.MarketId;
                this.CategoryId = source.CategoryId;

                this.SelPrice = market.SelPrice;
                this.SelName = market.SelName;
                this.SelImageUrl = market.SelImageUrl;
            }
        }

        [HttpPost]
        public ActionResult StartParsing()
        {
            var sources = db.ParserSources
                .Include(s => s.Market);

            foreach (ParserSource source in sources)
            {
                // Download web request
                string url = source.Url;
                string responceBody = new HttpDownloader(url, null, null).GetPage();

                // Create an appropriate parser
                IClassParser<ParserInput, Product> parser;
                if (source.ParserId == 0)
                {
                    parser = new HTMLParser<ParserInput, Product>(responceBody);
                }
                else
                {
                    parser = new JsonParser<ParserInput, Product>(responceBody);
                }

                // Create an input
                ParserInput input = new ParserInput(source, source.Market);

                // Use it to get products
                db.Products.AddRange(parser.Parse(input));
            }

            db.SaveChanges();

            // Watch products
            return RedirectToAction("Index", "Home");
        }

        // Idk, what it should be and whether it should be
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Sources()
        {
            return View(db.ParserSources.ToList());
        }

        [HttpPost]
        public ActionResult CreateParsingSource(ParserSource source)
        {
            if (db.Categories.Any(cat => cat.Id == source.CategoryId)
                && db.Markets.Any(m => m.Id == source.MarketId)
                && !db.ParserSources.Any(s => s.Url == source.Url))
            {
                db.ParserSources.Add(source);
                db.SaveChanges();

                return RedirectToAction("Sources");
            }

            ViewBag.ErrorMessage = "You typed wrong ids or URL!";
            return View("Error");
        }

        // This is an evil method! Rework it!
        [HttpGet]
        public ActionResult DeleteParsingSource(string url)
        {
            db.ParserSources
                .RemoveRange(
                    db.ParserSources.Where(source => source.Url == url));

            return RedirectToAction("Sources");
        }
    }
}