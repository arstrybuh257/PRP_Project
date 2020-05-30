using AutoMapper;
using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;
using GainBargain.WEB.Models;
using Hangfire;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace GainBargain.WEB.Controllers
{
    public class AdminController : Controller
    {
        private GainBargainContext db = new GainBargainContext();
        private ISuperCategoryRepository superCategories;
        private IRepository<Category> categoryRepository;
        private IRepository<Market> marketRepository;
        private IParserSourceRepository parserSourceRepository;
        private IDbLogsRepository dbLogsRepository;


        public AdminController()
        {
            superCategories = new SuperCategoryRepository(db);
            categoryRepository = new Repository<Category>(db);
            marketRepository = new Repository<Market>(db);
            parserSourceRepository = new ParserSourceRepository(db);
            dbLogsRepository = new DbLogsRepository(db);
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
        public ActionResult CreateMarketPartial()
        {
            return PartialView();
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
            catch (DataException)
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


        /// <summary>
        /// Our holy grail method.
        /// </summary>
        [HttpPost]
        public ActionResult StartParsing()
        {
            RecurringJob.Trigger(ConfigurationManager.AppSettings["ParsingJobId"] ?? "parsing");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public JsonResult ParsingState()
        {
            return Json(Models.Parser.ParsingProgress, JsonRequestBehavior.AllowGet);
        }

        // Idk, what it should be and whether it should be
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("AdminPanel");
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

        [HttpPost]
        public ActionResult ChangeParsingTime(int daysPeriod, int hoursTime, int minutesTime)
        {
            // If the model is valid
            if (daysPeriod >= 1 && daysPeriod <= 7
                && hoursTime >= 0 && hoursTime <= 23
                && minutesTime >= 0 && minutesTime < 60)
            {
                // Set CRON
                //ConfigurationManager.AppSettings["ParsingCron"] = $"{minutesTime} {hoursTime} */{daysPeriod} * *";

                ConfigurationManager.AppSettings.Set("ParsingCron",
                    $"{minutesTime} {hoursTime} */{daysPeriod} * *");
            }

            return RedirectToAction("AdminPanel");
        }

        /// <summary>
        /// Current parsing schedule options.
        /// </summary>
        [HttpGet]
        public JsonResult GetParsingTime()
        {
            string cron = ConfigurationManager.AppSettings["ParsingCron"];
            var pattern = new Regex(@"(\d+) (\d+) \*/(\d+) \* \*");
            var match = pattern.Match(cron);
            if (!match.Success)
            {
                // Oh no
                return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Status = "OK",
                MinutesTime = match.Groups[1].Value,
                HoursTime = match.Groups[2].Value,
                DaysPeriod = match.Groups[3].Value

            }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Information about previous parsing process
        /// </summary>
        public JsonResult GetPrevParsingInfo()
        {
            var con = db.Database.Connection;
            con.Open();
            var json = Models.Parser.GetParsingResult(con);
            con.Close();

            // If oh no
            if (json == null)
            {
                // Idk
                return Json(new { Status = "failed" }, JsonRequestBehavior.AllowGet);
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns logs string for the console
        /// </summary>
        public JsonResult GetLog(int page)
        {
            const int pageSize = 10;

            var logs = db.DbLogs
                .OrderByDescending(log => log.Time)
                .Skip(page * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .AsEnumerable()
                .Select(log => new
                {
                    Time = log.Time.ToString("HH:mm dd.MM.yyyy"),
                    log.Info,
                    log.Code
                })
                .ToArray();

            return Json(logs, JsonRequestBehavior.AllowGet);
        }
    }
}