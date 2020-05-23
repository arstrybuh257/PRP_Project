﻿using AutoMapper;
using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;
using GainBargain.Parser.Parsers;
using GainBargain.Parser.WebAccess;
using GainBargain.WEB.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace GainBargain.WEB.Controllers
{
    public class AdminController : Controller
    {
        GainBargainContext db = new GainBargainContext();
        ISuperCategoryRepository superCategories;
        IRepository<Category> categoryRepository;
        IRepository<Market> marketRepository;
        IParserSourceRepository parserSourceRepository;

        public AdminController()
        {
            superCategories = new SuperCategoryRepository(db);
            categoryRepository = new Repository<Category>(db);
            marketRepository = new Repository<Market>(db);
            parserSourceRepository = new ParserSourceRepository(db);
        }

        public ActionResult AdminPanel()
        {
            return View();
        }


        public ActionResult DataUpdate()
        {
            return PartialView();
        }

        public ActionResult CategoriesManager()
        {
            var model = superCategories.GetAll();
            return PartialView(model);
        }

        public ActionResult Markets()
        {
            var model = marketRepository.GetAll();
            return PartialView(model);
        }

        public ActionResult SourcesManager()
        {
            var parserSources = parserSourceRepository.GetAllParserSources();
            
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ParserSource, ParserSourceManager>()
                .ForMember("CategoryName", opt => opt.MapFrom(ps => ps.Category.Name))
                .ForMember("MarketName", opt => opt.MapFrom(ps => ps.Market.Name)));
            var mapper = new Mapper(config);

            var model = mapper.Map<List<ParserSourceManager>>(parserSources);

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult EditMarket(int? id)
        {
            if (id == null)
                return HttpNotFound();
            var model = marketRepository.Get((int)id);
            if (model == null)
                return HttpNotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMarket(Market market)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    marketRepository.Update(market);
                    marketRepository.Save();
                    return RedirectToAction("AdminPanel");
                }
            }
            catch(DataException)
            {
                //loggggg
            }

            return View(market);
        }

        public ActionResult DeleteMarket(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var market = marketRepository.Get((int)id);

            if (market == null)
                return HttpNotFound();

            marketRepository.Remove(market);
            marketRepository.Save();

            return RedirectToAction("AdminPanel");
        }

        public ActionResult DeleteParserSource(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var parserSrc = parserSourceRepository.Get((int)id);

            if (parserSrc == null)
                return HttpNotFound();

            parserSourceRepository.Remove(parserSrc);
            parserSourceRepository.Save();

            return RedirectToAction("AdminPanel");
        }

        [HttpGet]
        public ActionResult EditParserSource(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var model = parserSourceRepository.Get((int)id);

            if (model == null)
                return HttpNotFound();

            SelectList markets = new SelectList(marketRepository.GetAll(), "Id", "Name");
            SelectList categories = new SelectList(categoryRepository.GetAll(), "Id", "Name");

            ViewBag.Markets = markets;
            ViewBag.Categories = categories;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditParserSource(ParserSource parserSource)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    parserSourceRepository.Update(parserSource);
                    marketRepository.Save();
                    return RedirectToAction("AdminPanel");
                }
            }
            catch (DataException)
            {
                //loggggg
            }

            SelectList markets = new SelectList(marketRepository.GetAll(), "Id", "Name");
            SelectList categories = new SelectList(categoryRepository.GetAll(), "Id", "Name");

            ViewBag.Markets = markets;
            ViewBag.Categories = categories;

            return View(parserSource);
        }

        [HttpGet]
        public ActionResult CreateParserSource()
        {
            SelectList markets = new SelectList(marketRepository.GetAll(), "Id", "Name");
            SelectList categories = new SelectList(categoryRepository.GetAll(), "Id", "Name");

            ViewBag.Markets = markets;
            ViewBag.Categories = categories;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateParserSource(ParserSource parserSource)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    parserSourceRepository.Add(parserSource);
                    marketRepository.Save();
                    return RedirectToAction("AdminPanel");
                }
            }
            catch (DataException)
            {
                //log
            }

            SelectList markets = new SelectList(marketRepository.GetAll(), "Id", "Name");
            SelectList categories = new SelectList(categoryRepository.GetAll(), "Id", "Name");

            ViewBag.Markets = markets;
            ViewBag.Categories = categories;

            return View(parserSource);
        }

        [HttpGet]
        public ActionResult CreateMarket()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMarket(Market market)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    marketRepository.Add(market);
                    marketRepository.Save();
                    return RedirectToAction("AdminPanel");
                }
            }
            catch (DataException)
            {
                //here goes lOG
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(market);
        }

        [HttpGet]
        public ActionResult SubCategoriesPartial(int? id, string text)
        {
            if (id == null)
                return HttpNotFound();

            if (text != null)
            {
                Category category = new Category { Name = text, SuperCategoryId = (int)id };
                categoryRepository.Add(category);
                categoryRepository.Save();
            }

            var model = categoryRepository.Find(c => c.SuperCategoryId == id);
            ViewBag.scId = id;
            return PartialView(model);
        }



        [HttpPost, ValidateInput(false)]
        public EmptyResult SubCategoriesPartial(int? id, string name, string submitButton)
        {
            if (id != null)
            {
                Category category = categoryRepository.Get((int)id);
                if (category != null)
                {
                    ViewBag.scId = category.SuperCategoryId;
                    switch (submitButton)
                    {
                        case "Зберегти":
                            category.Name = name;
                            categoryRepository.Update(category);
                            categoryRepository.Save();
                            break;
                        case "Видалити":
                            categoryRepository.Remove(category);
                            categoryRepository.Save();
                            break;
                    }
                }
            }

            return new EmptyResult();
        }


        //public JsonResult GetSubCategories(int id)
        //{
        //    var categories = categoryRepository.Find(c => c.SuperCategoryId == id).ToList();
        //    var result = JsonConvert.SerializeObject(categories, Formatting.Indented,
        //               new JsonSerializerSettings
        //               {
        //                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //               });
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

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