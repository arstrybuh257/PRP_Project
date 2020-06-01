using GainBargain.DAL.EF;
using System.Web.Mvc;
using System.Linq;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;

namespace GainBargain.WEB.Controllers
{
    public class HomeController : Controller
    {
        GainBargainContext db = new GainBargainContext();

        public ActionResult Index()
        {
            //var db = new GainBargainContext();

            IProductCacheRepository rep = new ProductsCacheRepository(db);

            var productList = rep.GetTopProducts(12)
                .ToList();

            return View(productList);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}